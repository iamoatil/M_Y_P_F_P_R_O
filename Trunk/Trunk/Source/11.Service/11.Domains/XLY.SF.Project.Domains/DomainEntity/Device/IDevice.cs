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

    public static class DeviceExternsion
    {
        public const string XLY_BinKey = "device";
        public const string XLY_IdKey = "ID";
        public const string XLY_NameKey = "Name";

        /// <summary>
        /// 加载从配置文件中读取的属性配置，并生成Device设备
        /// </summary>
        /// <param name="dicPropertys"></param>
        /// <returns></returns>
        public static IDevice Load(Dictionary<string, string> dicPropertys)
        {
            if (dicPropertys == null || !dicPropertys.ContainsKey(XLY_IdKey) || !dicPropertys.ContainsKey(XLY_BinKey))
                return null;
            try
            {
                //object obj = Activator.CreateInstance(typeof(IDevice).Assembly.FullName, dicPropertys[XLY_TypeKey]);
                //foreach (var item in dicPropertys)
                //{
                //    if(item.Key != XLY_TypeKey)
                //    {
                //        obj.Setter(item.Key, item.Value.ChangeType(obj.GetType().GetProperty(item.Key).PropertyType));
                //    }
                //}
                //return obj as IDevice;

                return Serializer.DeSerializeFromBinary<IDevice>(dicPropertys[XLY_BinKey].ToByteArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 生成配置文件所需属性配置项
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Save(this IDevice device)
        {
            Dictionary<string, string> dicPropertys = new Dictionary<string, string>();
            //dicPropertys[XLY_TypeKey] = device.GetType().FullName;
            //foreach(var pi in device.GetType().GetProperties())
            //{
            //    var dp = pi.GetCustomAttribute(typeof(DPConfigAttribute));
            //    if(dp != null)
            //    {
            //        dicPropertys[pi.Name] = pi.GetValue(device).ToSafeString();
            //    }
            //}
            dicPropertys[XLY_IdKey] = device.ID;
            dicPropertys[XLY_NameKey] = device.Name;
            dicPropertys[XLY_BinKey] = Serializer.SerializeToBinary(device).ToHex();
            return dicPropertys;
        }
    }
}
