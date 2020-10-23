using Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Timers;

namespace MonitorApp
{
    public class Test1Timer : BaseTimer
    {
        protected override float IntervalInMin => 0.1F;
        public override void TimeEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("TICK");
        }
    }
}
