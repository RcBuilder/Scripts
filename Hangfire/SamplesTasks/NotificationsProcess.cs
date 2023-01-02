using Common;
using DistributionServiceDAL;
using Entities;
using Hangfire;
using Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BulldogBLL;
using TwilioBLL;
using System.Text;

namespace DistributionServiceBLL
{
    public class NotificationsProcess : IProcessAsync
    {
        // TODO ->> TEMP - constant map (need to upgrade to dynamic loading)     
        protected static Dictionary<string, int> AudioSourceQuantitiesMap { get; set; } = new Dictionary<string, int> {
            { "MM", 45 },
            { "MF", 74 },
            { "FM", 29 },
            { "FF", 34 },
            { "AM", 31 },
            { "AF", 31 },
            { "AJM", 32 },
            { "AJF", 65 }
        };

        protected static Dictionary<string, int> AfterWakeupSourceQuantitiesMap { get; set; } = new Dictionary<string, int> {
            { "Mtxt", 49 },
            { "Mpng", 80 },
            { "Ftxt", 74 },
            { "Fpng", 81 }
        };

        protected TwilioManager Twilio { get; set; }
        protected INotificationsDAL DAL { get; set; }
        protected ILogger Logger { get; set; }
        protected ConfigSingleton Config { get; set; }

        public bool IsRunning { protected set; get; }

        public NotificationsProcess() : this(
            new NotificationsDAL(ConfigSingleton.Instance.ConnStr),
            ConfigSingleton.Instance,
            LoggerSingleton.Instance ///new DummyLogger()
        )
        { }

        public NotificationsProcess(INotificationsDAL dal, ConfigSingleton config, ILogger logger)
        {
            this.DAL = dal;
            this.Config = config;
            this.Twilio = new TwilioManager(this.Config.TwilioAccountSid, Config.TwilioAuthToken);  // TODO ->> DI
            this.Logger = logger;
        }

        public void Run()
        {
            this.RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            if (!this.Config.ServicesOnOff) return;
            if (this.IsRunning) return;

            try
            {
                this.IsRunning = true;

                var subscriptions = await this.DAL.GetSubscriptions();
                if (subscriptions == null || subscriptions.Count == 0) return;

                /// this.Logger.Info("NotificationsProcess.Run", $"{string.Join(",", subscriptions.Select(x => x.Details.Id))} ({subscriptions.Count})", null);
                subscriptions.ForEach(HandleSubscription);
            }
            catch (Exception ex)
            {
                this.Logger.Error("NotificationsProcess.Run", ex);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private async void HandleSubscription(Subscription subscription)
        {
            var step = 0;
            try
            {
                var NOW = DateTime.Now.AddHours(1); // sync the server and the SQL timezone

                step = 1;

                var lastReceivedMessage = subscription.SystemSettings.LastReceivedMessage.HasValue ? subscription.SystemSettings.LastReceivedMessage.Value : DateTime.MinValue;
                var tsLastReceived = NOW.Date - lastReceivedMessage.Date;  // no time reference - only days!
                SubscriptionWakeupSettings wakeupSettings = subscription.WakeupSettings;
                TimeSpan TodayWakeupTime;
                switch (NOW.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (!wakeupSettings.WakeupActive1) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime1;
                        break;
                    case DayOfWeek.Monday:
                        if (!wakeupSettings.WakeupActive2) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime2;
                        break;
                    case DayOfWeek.Tuesday:
                        if (!wakeupSettings.WakeupActive3) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime3;
                        break;
                    case DayOfWeek.Wednesday:
                        if (!wakeupSettings.WakeupActive4) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime4;
                        break;
                    case DayOfWeek.Thursday:
                        if (!wakeupSettings.WakeupActive5) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime5;
                        break;
                    case DayOfWeek.Friday:
                        if (!wakeupSettings.WakeupActive6) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime6;
                        break;
                    case DayOfWeek.Saturday:
                        if (!wakeupSettings.WakeupActive7) return;
                        TodayWakeupTime = wakeupSettings.WakeupTime7;
                        break;
                    default:
                        throw new Exception("Unknown Day of week!");
                }

                var tsWakeupTime = NOW.TimeOfDay - TodayWakeupTime;

                /// this.Logger.Info("NotificationsProcess.HandleSubscription", $"{subscription.Id}|{NOW.TimeOfDay}|{subscription.WakeupTime}", null);

                var alreadyReceivedToday = tsLastReceived.Days == 0;  // determine whether the user already received messages today or not
                var isWakeupTime = tsWakeupTime.TotalMinutes >= 0 && tsWakeupTime.TotalMinutes < 30;  // tollerance of x minutes delayed

                step = 2;

                ///this.Logger.Info("NotificationsProcess.HandleSubscription", $"[#{subscription.Id}] Days: {tsLastReceived.Days}, Minutes: {(int)tsWakeupTime.TotalMinutes}", null);

                if (alreadyReceivedToday || !isWakeupTime)
                    return; // if the use received messages or its NOT the defined wakeup time - stop processing                

                step = 3;

                /*
                var isWeekend = NOW.DayOfWeek == DayOfWeek.Friday || NOW.DayOfWeek == DayOfWeek.Saturday;
                if (isWeekend && subscription.WakeupSettings.WakeupState == eWakeupState.NoWeekend)
                    return; // we are in the weekend and the user has chosen to NOT get notifications on the weekend                
                */
                step = 4;

                // fix phone numer if necessary (+972 instead of prefix 0)
                if (subscription.Details.Phone.StartsWith("0"))
                    subscription.Details.Phone = $"+972{subscription.Details.Phone.Remove(0, 1)}";

                step = 5;

                var genderLetter = subscription.WakeupSettings.Gender == eGender.Male ? "M" : "F";  // M (Male) or F (Female)

                // turn-on text-message feature only for the specified days of week.
                var textMessagesON = DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday;
                var messageId = string.Empty;

                // TODO ->> TEMPORARY Disabled 
                textMessagesON = false;

                try
                {
                    if (textMessagesON && subscription.WakeupSettings.UseTextMessage)
                    {
                        var messageName = $"{(byte)NOW.DayOfWeek + 1}{genderLetter}.txt";  // e.g: 1F.txt
                        var messagePath = $"{this.Config.MessagesServer}{messageName}";
                        var messageTemplate = Helper.GetFileContentFromWeb(messagePath);
                        messageId = this.Twilio.SendSMS(this.Config.TwilioPhoneNumber, subscription.Details.Phone, messageTemplate.Replace("[0]", subscription.Details.FirstName));
                        Debug.WriteLine(messageId);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Info("NotificationsProcess.HandleSubscription", $"{ex.Message} (step {step})", null);
                }

                step = 6;

                string callId = string.Empty, audioName = string.Empty;
                if (subscription.WakeupSettings.UseAudioMessage)
                {
                    var audioState = string.Empty;
                    var audioIndex = 1;

                    switch (subscription.WakeupSettings.VoiceGender)
                    {
                        case eVoiceType.Male:
                            audioState = $"M{genderLetter}"; // MM, MF
                            subscription.SystemSettings.IndexMale++;
                            audioIndex = subscription.SystemSettings.IndexMale;

                            if (audioIndex > AudioSourceQuantitiesMap[audioState])
                                subscription.SystemSettings.IndexMale = audioIndex = 1;  // we've reached the last audio file - reset index 

                            break;
                        default:
                        case eVoiceType.Female:
                            audioState = $"AJ{genderLetter}"; // AJM, AJF
                            subscription.SystemSettings.IndexFemale++;
                            audioIndex = subscription.SystemSettings.IndexFemale;

                            if (audioIndex > AudioSourceQuantitiesMap[audioState])
                                subscription.SystemSettings.IndexFemale = audioIndex = 1;  // we've reached the last audio file - reset index 

                            break;
                        case eVoiceType.Alexa:
                            audioState = $"A{genderLetter}"; // AM, AF
                            subscription.SystemSettings.IndexAlexa++;
                            audioIndex = subscription.SystemSettings.IndexAlexa;

                            if (audioIndex > AudioSourceQuantitiesMap[audioState])
                                subscription.SystemSettings.IndexAlexa = audioIndex = 1;  // we've reached the last audio file - reset index 

                            break;
                    }

                    audioName = $"{audioIndex}{audioState}.wav";  // e.g: 1MM.wav
                    var audioPath = $"{this.Config.AudioServer}{audioName}";
                    var prefixPath = $"{this.Config.AudioServer}prefix{audioState}.wav";
                    var suffixPath = $"{this.Config.AudioServer}suffix{audioState}.wav";

                    // dedicated prefix                    
                    var firstNameEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(subscription.Details.FirstName?.Trim() ?? ""));
                    var dedicatedPrefixPath = $"{this.Config.AudioServer}prefix{audioState}_{firstNameEncoded}.wav";
                    var isFileExists = Helper.IsHttpFileExists(dedicatedPrefixPath); 
                    if (isFileExists) prefixPath = dedicatedPrefixPath;  // override the default prefix with a dedicated one (by user first name)                    

                    var useMachineDetection = subscription.Details.Id == 388 || subscription.Details.Id == 413;
                    callId = this.Twilio.MakeACall(this.Config.TwilioPhoneNumber, subscription.Details.Phone, new List<string> { prefixPath, audioPath, suffixPath }, useMachineDetection);
                    Debug.WriteLine(callId);
                }

                step = 7;

                this.Logger.Info("NotificationsProcess.HandleSubscription", $"[#{subscription.Details.Id}] messageId: {messageId}, callId: {callId}", null);

                await save_subscriptionConfig(subscription);
                await this.DAL.Save(new Notification
                {
                    SubscriptionId = subscription.Details.Id,
                    MessageId = messageId,
                    CallId = callId,
                    CallFileName = audioName,
                    CallLastStatus = "",
                    CallRedialAttempts = 1
                });

                var jobId = BackgroundJob.Schedule(() => SendAfterWakeupMessage(subscription), TimeSpan.FromMinutes(60));
            }
            catch (Exception ex)
            {
                ex.Data.Add("step", step);                
                this.Logger.Error("NotificationsProcess.HandleSubscription", ex);
            }
        }

        private async Task save_subscriptionConfig(Subscription subscription)
        {
            await this.DAL.SaveConfig(subscription.Details.Id, new SubscriptionSystemSettings
            {
                ///LastReceivedMessage = NOW, 
                IndexFemale = subscription.SystemSettings.IndexFemale,
                IndexMale = subscription.SystemSettings.IndexMale,
                IndexAlexa = subscription.SystemSettings.IndexAlexa,
                IndexDailyMSG = subscription.SystemSettings.IndexDailyMSG,
                IndexDailyIMG = subscription.SystemSettings.IndexDailyIMG,
                IndexDailyTYPE = subscription.SystemSettings.IndexDailyTYPE,
            });
        }

        public async Task SendAfterWakeupMessage(Subscription subscription)
        {
            /// ...
        }
    }
}

