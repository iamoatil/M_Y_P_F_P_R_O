using System;
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
        /// 已经完成的大小
        /// </summary>
        public long FinishedSize
        {
            get { return _finishedSize; }
            set
            {
                _finishedSize = value;
                //只有在进度变化1000分之一时才通知
                if (Math.Abs(_lastChangedValue - _finishedSize) > TotalSize / 1000)
                {
                    OnPropertyChanged();
                    _lastChangedValue = _finishedSize;
                }
            }
        }
        private long _finishedSize = 0;

        private long _lastChangedValue = 0;

        /// <summary>
        /// 总共大小
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
            set
            {
                _totalSize = value;
                OnPropertyChanged();
            }
        }
        private long _totalSize = 100;

        /// <summary>
        /// 剩余时间
        /// </summary>
        public string RemainTime
        {
            get
            {
                return _remainTime;
            }
            set
            {
                _remainTime = value;
                OnPropertyChanged();
            }
        }
        private string _remainTime;

        /// <summary>
        /// 已经用的时间
        /// </summary>
        public string UsedTime
        {
            get
            {
                return _usedTime;
            }
            set
            {
                _usedTime = value;
                OnPropertyChanged();
            }
        }
        private string _usedTime;

        /// <summary>
        /// 通过数据量的大小，得出镜像他们所需的时间
        /// </summary>
        /// <param name="size"></param>
        public void OnProgress(long size)
        {
            long estimatedTime = size / (1024 * 1024 * 5);
            RemainTime = TimeSpan.FromSeconds(estimatedTime).ToString();
            TimeSpan timeSpan = DateTime.Now - _startedDateTime;
            UsedTime = timeSpan.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// 开始时设置Progress状态
        /// </summary>
        public void Start()
        {
            _startedDateTime = DateTime.Now;
        }

        /// <summary>
        /// 结束时设置Progress状态
        /// </summary>
        public void Stop()
        {
            _startedDateTime = DateTime.Now;
        }
    }
}
