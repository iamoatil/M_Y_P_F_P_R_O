using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:24:02
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    [Serializable]
    public class NotifyPropertyBase : INotifyPropertyChanged
    {

        #region 选择标识

        [NonSerialized]
        private bool _isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [field: NonSerializedAttribute()]
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
    }
}
