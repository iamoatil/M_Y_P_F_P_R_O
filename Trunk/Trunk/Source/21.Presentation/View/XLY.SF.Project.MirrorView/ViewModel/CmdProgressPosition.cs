using System;
using System.Timers;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Description：CmdProgressPosition  
* Author     ：litao
* Create Date：2017/11/16 9:50:42
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 进度条位置
    /// </summary>
    public class CmdProgressPosition : NotifyPropertyBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime _startedDateTime;

        /// <summary>
        /// 暂停的时间点
        /// </summary>
        private DateTime _puaseDateTime;

        /// <summary>
        /// 已经完成的大小
        /// </summary>
        public long FinishedSize
        {
            get { return _finishedSize; }
            set
            {
                _finishedSize = value;
                if (TotalSize > 0)
                {
                    //只有在进度变化1000分之一时才通知
                    if (Math.Abs(_lastChangedValue - _finishedSize) > TotalSize / 1000)
                    {
                        OnPropertyChanged();
                        _lastChangedValue = _finishedSize;

                        FinishedSizeStr = $"{SizeToString(_finishedSize)}/{_totalSizeStr}";

                        //计算速度
                        Speed = $"{SizeToString(FinishedSize / UsedTime.TotalSeconds)}/s";

                        Progress = _finishedSize / (1.0 * _totalSize);
                    }
                }
                else
                {
                    OnPropertyChanged();
                    _lastChangedValue = _finishedSize;

                    FinishedSizeStr = SizeToString(_finishedSize);

                    //计算速度
                    Speed = $"{SizeToString(FinishedSize / UsedTime.TotalSeconds)}/s";
                }
            }
        }

        private double _Progress;

        /// <summary>
        /// 当前进度
        /// </summary>
        public double Progress
        {
            get => _Progress;
            set
            {
                _Progress = value;
                OnPropertyChanged();
            }
        }

        private long _finishedSize = 0;

        private string _finishedSizeStr;

        public string FinishedSizeStr
        {
            get => _finishedSizeStr;
            set
            {
                _finishedSizeStr = value;
                OnPropertyChanged();
            }
        }

        private long _lastChangedValue = 0;
        private long _lastTimeValue = 0;

        /// <summary>
        /// 总共大小
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
            set
            {
                _totalSize = value;
                _totalSizeStr = SizeToString(_totalSize);
                FinishedSizeStr = $"0KB/{_totalSizeStr}";
                OnPropertyChanged();
            }
        }

        private long _totalSize = 100;

        private string _totalSizeStr;

        /// <summary>
        /// 已经用的时间
        /// </summary>
        public string UsedTimeStr
        {
            get
            {
                return _usedTimeStr;
            }
            set
            {
                _usedTimeStr = value;
                OnPropertyChanged();
            }
        }

        private string _usedTimeStr;

        private TimeSpan UsedTime { get; set; }

        private string _Speed;

        /// <summary>
        /// 镜像速度
        /// </summary>
        public string Speed
        {
            get => _Speed;
            set
            {
                _Speed = value;
                OnPropertyChanged();
            }
        }

        private string _Msg;

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Msg
        {
            get => _Msg;
            set
            {
                _Msg = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private Timer MirrorTimer { get; set; }

        /// <summary>
        /// 获取相对于上一次的间隔
        /// </summary>
        /// <returns></returns>
        public long GetIntervalToLastTime(long curValue)
        {
            if (_lastTimeValue > curValue)
            {
                _lastTimeValue = curValue;
                return curValue;
            }
            long ret = curValue - _lastTimeValue;
            _lastTimeValue = curValue;
            return ret;
        }

        /// <summary>
        /// 开始时设置Progress状态
        /// </summary>
        public void Start()
        {
            _startedDateTime = DateTime.Now;
            Progress = 0;
            FinishedSize = 0;
            UsedTime = new TimeSpan();
            Speed = "0KB/s";
            UsedTimeStr = "00:00:00";
            Msg = string.Empty;

            MirrorTimer = new Timer();
            MirrorTimer.Interval = 1000;
            MirrorTimer.Elapsed += Ti_Elapsed;
            MirrorTimer.Start();
        }

        const int G1 = 1024 * 1024 * 1024;
        const int M1 = 1024 * 1024;
        const int K1 = 1024;

        private void Ti_Elapsed(object sender, ElapsedEventArgs e)
        {
            UsedTime = DateTime.Now - _startedDateTime;
            UsedTimeStr = UsedTime.ToString(@"hh\:mm\:ss");
        }

        private string SizeToString(double size)
        {
            var str = "";

            if (size > G1)
            {
                str = Math.Round(size / G1, 2).ToString() + "GB";
            }
            else if (size > M1)
            {
                str = Math.Round(size / M1, 2).ToString() + "MB";
            }
            else
            {
                str = Math.Round(size / K1, 2).ToString() + "KB";
            }

            return str;
        }

        /// <summary>
        /// 结束时设置Progress状态
        /// </summary>
        public void Stop()
        {
            if (null != MirrorTimer)
            {
                _startedDateTime = DateTime.Now;
                MirrorTimer?.Stop();
                MirrorTimer = null;

                Progress = 0;
                FinishedSize = 0;
                UsedTimeStr = string.Empty;
                Msg = string.Empty;
                Speed = string.Empty;
                FinishedSizeStr = string.Empty;
            }
        }

        /// <summary>
        /// 暂停时记录时间点，继续时修改_startedDateTime。
        /// </summary>
        public void Pause()
        {
            _puaseDateTime = DateTime.Now;
            MirrorTimer.Enabled = false;
        }

        /// <summary>
        /// 暂停时记录时间点，继续时修改_startedDateTime
        /// </summary>
        public void Continue()
        {
            _startedDateTime += (DateTime.Now - _puaseDateTime);
            MirrorTimer.Enabled = true;
        }
    }
}
