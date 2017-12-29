using System;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.ViewModels.Extraction
{
    public class ExtractItemAdorner : NotifyPropertyBase
    {
        #region Constructors

        public ExtractItemAdorner(ExtractItem target)
        {
            Target = target;
        }

        #endregion

        #region Properties

        /// <summary>
        /// ExtractItem 实例。
        /// </summary>
        public ExtractItem Target { get; }

        /// <summary>
        /// 组名。
        /// </summary>
        public String Group => Target.GroupName;

        /// <summary>
        /// 项名。
        /// </summary>
        public String Name => Target.Name;

        #region Count

        private Int32 _count;
        /// <summary>
        /// 提取结果数量。
        /// </summary>
        public Int32 Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region State

        private TaskState _state;
        /// <summary>
        /// 任务状态。
        /// </summary>
        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Elapsed

        private TimeSpan _elapsed;
        /// <summary>
        /// 任务耗时。
        /// </summary>
        public TimeSpan Elapsed
        {
            get => _elapsed;
            set
            {
                _elapsed = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Progress

        private Double _progress;
        /// <summary>
        /// 进度
        /// </summary>
        public Double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        public void Reset()
        {
            Progress = 0;
            Count = 0;
            State = TaskState.Idle;
            Elapsed = TimeSpan.Zero;
        }

        #endregion

        #endregion
    }
}
