using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 超时定时器
    /// </summary>
    public class TimeoutTimer
    {
        public TimeoutTimer(int timeout, Action action, int tick = 100)
        {
            _timer = new System.Timers.Timer(500);    // 参数单位为ms
            _timer.Elapsed += _timer_Elapsed; ;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();

            Callback = action;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockobj)
            {
                Callback();
            }
        }

        private System.Timers.Timer _timer;
        private Action Callback;
        private object _lockobj = new object();
    }
}
