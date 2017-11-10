using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataExtraction
{
    public class ExtractionItem : NotifyPropertyBase
    {
        #region Constructors

        public ExtractionItem(ExtractItem target)
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

        #region IsChecked

        private Boolean _isChecked;
        /// <summary>
        /// 选择状态
        /// </summary>
        public Boolean IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
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
    }
}
