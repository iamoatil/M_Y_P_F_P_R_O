/* ==============================================================================
* Description：ResultViewModel  
* Author     ：litao
* Create Date：2017/11/24 15:29:04
* ==============================================================================*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class ResultViewModel:INotifyPropertyChanged
    {
        public ResultViewModel()
        {
            _earlyWarning.Initialize();
            ExtactionCategoryCollectionManager categoryManager = _earlyWarning.ExtactionItemParser.CategoryManager;
        }

        /// <summary>
        /// 智能预警
        /// </summary>
        DetectionManager _earlyWarning { get { return DetectionManager.Instance; } }

        
        
        /// <summary>
        /// 预警的结果
        /// </summary>
        public ObservableCollection<DataExtactionItem> DataList { get { return _dataList; } }
        ObservableCollection<DataExtactionItem> _dataList = new ObservableCollection<DataExtactionItem>();

       

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
