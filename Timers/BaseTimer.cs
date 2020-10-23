using Entities;
using System;
using System.Security.Principal;
using System.Timers;

namespace MonitorApp
{
    public abstract class BaseTimer : ITimer
    {
        protected virtual float IntervalInMin { get; set; } = 1;
        protected WindowsIdentity Identity => Helper.GetWindowsIdentity();
        protected Machine Machine => MachineSingleTon.Instance.Machine;
        protected WinUser WinUser { get; set; } = MachineSingleTon.Instance.WinUser;

        public Timer Timer { get; set; }
        public bool IsRunning { get; protected set; } = false;

        public BaseTimer() {
            this.Timer = new Timer();
            this.Timer.Elapsed += TimeEventWrapper;
            this.Timer.Interval = (int)(this.IntervalInMin * 60 * 1000);
        }

        private void TimeEventWrapper(object sender, EventArgs e) {
            try
            {
                if (this.IsRunning) return;
		this.IsRunning = true;
                this.TimeEvent(sender, e);
            }
            catch (Exception ex)
            {
                TraceSingleTon.Instance.Append($"[Exception] Timer: {this.GetType().Name}, ex:{ex.Message}");
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        public abstract void TimeEvent(object sender, EventArgs e);

        public virtual void Start() {
            this.Timer.Start();
            this.Timer.Enabled = true;
        }

        public virtual void Stop() {
            this.Timer.Stop();
            this.Timer.Enabled = false;
        }
    }
}
