using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Domains;
using ExtractItem = XLY.SF.Project.CaseManagement.ExtractItem;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    /// <summary>
    /// 设备提取装饰器。
    /// </summary>
    public class DeviceExtractionAdorner : NotifyPropertyBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 DeviceExtractionDecorator 实例。
        /// </summary>
        /// <param name="target">要装饰的 DeviceExtraction 类型实例。</param>
        public DeviceExtractionAdorner(DeviceExtraction target)
        {
            Target = target?? throw new ArgumentNullException("target");
        }

        /// <summary>
        /// 初始化类型 DeviceExtractionDecorator 实例。
        /// </summary>
        public DeviceExtractionAdorner()
        {

        }

        #endregion

        #region Properties

        #region Target

        private DeviceExtraction _target;

        /// <summary>
        /// 目标对象。
        /// </summary>
        public DeviceExtraction Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    _target = value;
                    if (value == null)
                    {
                        Id = null;
                        Type = null;
                        _name = null;
                    }
                    else
                    {
                        if (!value.Existed) throw new InvalidOperationException("Target is not existed");
                        Id = value.Id;
                        Type = value.Type;
                        _name = value["Name"];
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 设备Id。
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// 设备类型。
        /// </summary>
        public String Type { get; private set; }

        #region Name

        private String _name;
        /// <summary>
        /// 设备名称。
        /// </summary>
        public String Name
        {
            get => _name;
            set
            {
                if (SetValue("Name", value))
                {
                    _name = value;
                }
            }
        }

        #endregion

        #region Device

        private IDevice _device;
        public IDevice Device
        {
            get
            {
                if (_device == null)
                {
                    Dictionary<String, String> dic = new Dictionary<string, string>();
                    foreach (String name in _target.PropertNames)
                    {
                        dic.Add(name, GetValue(name));
                    }
                    _device = DeviceExternsion.Load(dic);
                }
                return _device;
            }
            set
            {
                if (_device != value)
                {
                    Dictionary<String, String> dic = value.Save();
                    foreach (var item in dic)
                    {
                        SetValue(item.Key, item.Value);
                    }
                    _device = value;
                    _name = dic["Name"];
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取 DeviceExtraction 实例装饰后的对象。
        /// </summary>
        /// <typeparam name="T">装饰类型。</typeparam>
        /// <param name="target">目标对象。</param>
        /// <returns>装饰后的对象。</returns>
        public static T GetAdorner<T>(DeviceExtraction target)
            where T : DeviceExtractionAdorner,new()
        {
            return new T { Target = target };
        }

        /// <summary>
        /// 保存。
        /// </summary>
        /// <returns>成功返回true；失败返回false。</returns>
        public Boolean Save()
        {
            if (CheckValidation())
            {
                return  _target.Save(); 
            }
            return false;
        }

        /// <summary>
        /// 删除。
        /// </summary>
        public void Delete()
        {
            if (CheckValidation())
            {
                _target.Delete();
            }
        }

        /// <summary>
        /// 将一个提取项与当前设备提取关联。
        /// </summary>
        /// <param name="item">提取项。</param>
        public void Attach(ExtractItem item)
        {
            if (CheckValidation())
            {
                _target.Attach(item);
            }
        }

        /// <summary>
        /// 将一个提取项与当前设备提取分离。
        /// </summary>
        /// <param name="item">提取项。</param>
        public void Detach(ExtractItem item)
        {
            if (CheckValidation())
            {
                _target.Detach(item);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 设置属性值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <param name="value">属性值。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected Boolean SetValue(String propertyName, String value)
        {
            if (CheckValidation())
            {
                _target[propertyName] = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>属性值。</returns>
        protected String GetValue(String propertyName)
        {
            if (CheckValidation())
            {
                return _target[propertyName];
            }
            return null;
        }

        #endregion

        #region Private

        private Boolean CheckValidation()
        {
            return _target != null && _target.Existed;
        }

        #endregion

        #endregion
    }
}
