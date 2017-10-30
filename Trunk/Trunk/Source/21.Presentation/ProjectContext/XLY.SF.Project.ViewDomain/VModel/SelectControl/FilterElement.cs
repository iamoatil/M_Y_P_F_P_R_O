using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.ViewDomain.VModel.SelectControl
{
    public class FilterElement: NotifyPropertyBase
    {
        #region 赛选显示内容

        private string _filterName;
        /// <summary>
        /// 赛选显示内容
        /// </summary>
        public string FilterName
        {
            get
            {
                return this._filterName;
            }

            set
            {
                this._filterName = value;
                base.OnPropertyChanged();
            }
        }
        #endregion

        #region 实际过滤内容

        private string _filterValue;
        /// <summary>
        /// 实际过滤内容
        /// </summary>
        public string FilterValue
        {
            get
            {
                return this._filterValue;
            }

            set
            {
                this._filterValue = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
