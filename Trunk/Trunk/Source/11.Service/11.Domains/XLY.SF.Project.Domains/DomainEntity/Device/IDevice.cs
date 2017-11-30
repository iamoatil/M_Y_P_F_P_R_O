/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/8 9:59:29 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        EnumDeviceType DeviceType { get; set; }
        /// <summary>
        /// 当前设备状态
        /// </summary>
        EnumDeviceStatus Status { get; set; }
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        string ID { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 采集信息
        /// </summary>
        ExportCollectionInfo CollectionInfo { get; set; }
    }



    /// <summary>
    /// 设备抽象类
    /// </summary>
    [Serializable]
    public abstract class AbstractDevice : NotifyPropertyBase, IDevice
    {
        #region DeviceType -- 设备类型

        private EnumDeviceType _DeviceType = EnumDeviceType.None;

        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumDeviceType DeviceType
        {
            get { return _DeviceType; }
            set
            {
                _DeviceType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Status -- 设备状态

        [NonSerialized]
        private EnumDeviceStatus _Status = EnumDeviceStatus.None;
        /// <summary>
        /// 设备状态
        /// </summary>
        public EnumDeviceStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ID -- 设备唯一标识符
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public virtual string ID { get; set; }
        #endregion

        #region Name -- 设备显示的名称
        private string _Name;
        /// <summary>
        /// 设备显示的名称
        /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 采集信息
        /// </summary>
        public ExportCollectionInfo CollectionInfo { get ; set ; }
        #endregion

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other != null && other is IDevice && this.ID == (other as IDevice).ID;
        }
    }

    
}
