using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.VModel.DevHomePage
{
    /// <summary>
    /// 存储卡设备
    /// </summary>
    public class MemoryCardDevModel : DeviceModel
    {
        public MemoryCardDevModel()
        {
            HasDeviceCSize = true;
        }

        #region 存储卡类型

        private string _memoryCardTypeName;
        /// <summary>
        /// 存储卡类型
        /// </summary>
        public string MemoryCardTypeName
        {
            get
            {
                return this._memoryCardTypeName;
            }

            set
            {
                this._memoryCardTypeName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 编号

        private string _number;
        /// <summary>
        /// 编号
        /// </summary>
        public string Number
        {
            get
            {
                return this._number;
            }

            set
            {
                this._number = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
