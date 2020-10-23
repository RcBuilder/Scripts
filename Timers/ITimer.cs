using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MonitorApp
{
    public interface ITimer
    {
        Timer Timer { get; }
        bool IsRunning { get; }
        void TimeEvent(object sender, EventArgs e);
        void Start();
        void Stop();
    }
}
