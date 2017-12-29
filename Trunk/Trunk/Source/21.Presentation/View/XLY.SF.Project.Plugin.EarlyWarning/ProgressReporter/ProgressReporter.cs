using System;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 进度状态
    /// </summary>
    enum ProgressState
    {
        Unknow,

        /// <summary>
        /// 正处于进度状态
        /// </summary>
        IsProgressing,

        /// <summary>
        /// 进度走完
        /// </summary>
        IsFinished
    }

    /// <summary>
    /// 进度状态器
    /// </summary>
    class ProgressStater
    {
        public ProgressState State { get; private set; }
        public ProgressStater(ProgressState state)
        {
            State = state;
        }
    }

    /// <summary>
    ///  一个进度报告器
    /// </summary>
    class ProgressReporter : IProgressReporter
    {
        public event EventHandler<IProgressEventArg> ProgresssChanged;
        public virtual void Report(object parameter, double value)
        {
            if (ProgresssChanged != null)
            {
                ProgresssChanged(this, new ProgressEventArg(parameter, value));
            }
        }
    }

    /// <summary>
    /// 插件进度报告器
    /// </summary>
    class PluginProgressReporter : ProgressReporter
    {
        /// <summary>
        /// 插件个数
        /// </summary>
        private int _pluginCount;

        public void SetPluginCount(int pluginCount)
        {
            _pluginCount = pluginCount;
        }

        public override void Report(object parameter, double value)
        {
            if(_pluginCount ==0)
            {
                base.Report(parameter, 0);
                return;
            }

            if(value <  0)
            {
                value = 1.0 / _pluginCount;
            }
            base.Report(parameter, value);
        }
    }

    /// <summary>
    ///  双进度报告器
    /// </summary>
    class TwoProgressReporter : IProgressReporter
    {
        int isReporterOverCount = 0;
        IProgress _reporter1;
        IProgress _reporter2;

        public event EventHandler<IProgressEventArg> ProgresssChanged;
        public virtual void Report(object parameter, double value)
        {
            if (ProgresssChanged != null)
            {
                ProgresssChanged(this, new ProgressEventArg(parameter, value));
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(IProgress reporter1, IProgress reporter2)
        {
            _reporter1 = reporter1;
            _reporter2 = reporter2;
            _reporter1.ProgresssChanged -= OnProgresssChanged;
            _reporter2.ProgresssChanged -= OnProgresssChanged;
            _reporter1.ProgresssChanged += OnProgresssChanged;
            _reporter2.ProgresssChanged += OnProgresssChanged;
        }

        public void Reset()
        {
            isReporterOverCount = 0;
        }

        private void OnProgresssChanged(object sender, IProgressEventArg e)
        {
            ProgressStater stater = e.Parameter as ProgressStater;
            if (stater.State == ProgressState.IsFinished)
            {
                isReporterOverCount++;
            }
            if (isReporterOverCount == 2)
            {
                this.Report(new ProgressStater(ProgressState.IsFinished), e.ProgressValue * 0.5);
            }
            else
            {
                this.Report(new ProgressStater(ProgressState.IsProgressing), e.ProgressValue * 0.5);
            }
        }
    }
}
