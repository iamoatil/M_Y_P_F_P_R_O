using System;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    ///  进度参数
    /// </summary>
    interface IProgressEventArg
    {
        object Parameter { get; }
        double ProgressValue { get; }
    }

     /// <summary>
     /// 进度
     /// </summary>
    interface IProgress
    {
        event EventHandler<IProgressEventArg> ProgresssChanged;
    }

    /// <summary>
    ///  报告器
    /// </summary>
    interface IReporter
    {
        void Report(object paramter,double value);
    }

    /// <summary>
    /// 进度报告器
    /// </summary>
    interface IProgressReporter:IReporter,IProgress
    {

    }

    /// <summary>
    /// 进度的参数
    /// </summary>
    class ProgressEventArg : IProgressEventArg
    {
        public object Parameter { get; private set; }
        public double ProgressValue { get; private set; }

        public ProgressEventArg(object parameter, double value)
        {
            Parameter = parameter;
            ProgressValue = value;
        }
    }
}
