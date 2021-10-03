using Common;
using DistributionServiceDAL;
using Entities;
using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwilioBLL;

namespace DistributionServiceBLL
{
    public class RedialProcess : IProcessAsync {
        protected TwilioManager Twilio { get; set; }
        protected INotificationsDAL DAL { get; set; }
        protected ILogger Logger { get; set; }
        protected ConfigSingleton Config { get; set; }

        public bool IsRunning { protected set; get; }

        public RedialProcess() : this(
            new NotificationsDAL(ConfigSingleton.Instance.ConnStr),
            ConfigSingleton.Instance,
            LoggerSingleton.Instance ///new DummyLogger()
        )
        { }

        public RedialProcess(INotificationsDAL dal, ConfigSingleton config, ILogger logger)
        {
            this.DAL = dal;
            this.Config = config;
            this.Twilio = new TwilioManager(this.Config.TwilioAccountSid, Config.TwilioAuthToken);  // TODO ->> DI
            this.Logger = logger;
        }

        public void Run() {
            this.RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            if (!this.Config.ServicesOnOff) return;
            if (this.IsRunning) return;

            try
            {
                this.IsRunning = true;

                var notifications = await this.DAL.GetIncompleted();
                if (notifications == null || notifications.Count == 0) return;
                
                notifications.ForEach(HandleNotification);
            }
            catch (Exception ex)
            {
                this.Logger.Error("RedialProcess.Run", ex);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private async void HandleNotification(Notification notification)
        {            
            try
            {
                // get call info
                var callInfo = this.Twilio.GetCallInfo(notification.CallId);
                notification.CallLastStatus = callInfo.Status;  // update call status                

                Debug.WriteLine($"call #{callInfo.SId}, status {callInfo.Status}, answeredBy {callInfo.AnsweredBy}");                
                this.Logger.Info("HandleNotification", $"[#{callInfo.SId}] status: {callInfo.Status}, answeredBy: {callInfo.AnsweredBy}", null);

                var NOW = DateTime.Now; //.AddHours(1); // sync the server and the SQL timezone
                var tsRedialTime = NOW - notification.CreatedDate;

                // mark as completed for the following cases:
                var isOldCall = tsRedialTime.TotalHours > 6;  // old calls
                var noFileToPlay = string.IsNullOrEmpty(notification.CallFileName);  // no file to play
                var tooManyAttempts = notification.CallRedialAttempts > 2;  // too many attempts
                var answered = callInfo.Status == "completed" && callInfo.AnsweredBy != "machine_start";  // user has answered the call

                if (isOldCall || noFileToPlay || tooManyAttempts || answered) {
                    notification.IsCompleted = true;
                    await this.DAL.Save(notification);
                    return;
                }

                // each attempt equals to x minutes after the original call (x=30)
                // add tollerance of y minutes delayed (y=30)
                var fromTime = notification.CallRedialAttempts * 30;
                var toTime = fromTime + 20; // tollerance 
                var isRedialTime = tsRedialTime.TotalMinutes >= 0 && tsRedialTime.TotalMinutes >= fromTime && tsRedialTime.TotalMinutes <= toTime;
                ///this.Logger.Info("RedialProcess.HandleNotification", $"redial={isRedialTime}: {tsRedialTime.TotalMinutes} >= {fromTime} && {tsRedialTime.TotalMinutes} <= {toTime}", null);

                // status: queued, ringing, in-progress, canceled, completed, failed, busy, no-answer
                // answeredBy: machine_start, human, fax, unknown
                var statusesToRedial = new List<string> { "canceled", "failed", "busy", "no-answer" };

                // if the call hasn't been answered or the it answered by a voice-mail - redial.                
                var needToRedial = (statusesToRedial.Contains(callInfo.Status) || callInfo.AnsweredBy == "machine_start");                                

                if (needToRedial && isRedialTime)
                {
                    var audioPath = $"{this.Config.AudioServer}{notification.CallFileName}";
                    var newCallId = this.Twilio.MakeACall(callInfo.From, callInfo.To, audioPath);

                    var oldCallId = notification.CallId;
                    notification.CallId = newCallId;    // override the old call id
                    notification.CallRedialAttempts++;

                    this.Logger.Info("RedialProcess.HandleNotification", $"[#{notification.RowId}] callId: {notification.CallId}(new), {oldCallId}(old)", null);
                }                

                await this.DAL.Save(notification);
            }
            catch (Exception ex) {                
                this.Logger.Error("RedialProcess.HandleNotification", ex);
            }
        }
    }
}
