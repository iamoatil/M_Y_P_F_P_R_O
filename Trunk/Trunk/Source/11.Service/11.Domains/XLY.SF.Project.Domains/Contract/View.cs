using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：View  
* Author     ：Fhjun
* Create Date：2017/3/17 15:57:38
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 视图（数据）类型
    /// </summary>
    public enum EnumViewType
    {
        /// <summary>
        /// xly预定义
        /// </summary>
        XLY = 0,

        /// <summary>
        /// 用户自定义类型
        /// </summary>
        Custom = 1,
    }

    #region DataView（数据视图配置）

    /// <summary>
    /// 数据视图配置
    /// </summary>
    [Serializable]
    [XmlRoot("Data")]
    public class DataView
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// 支持的数据契约
        /// </summary>
        [XmlAttribute("contract")]
        public string Contract { get; set; }

        [XmlElement("Item")]
        public List<DataItem> Items { get; set; }

        /// <summary>
        /// 视图所属的插件
        /// </summary>
        [XmlIgnore]
        public DataParsePluginInfo Plugin { get; set; }

        private Type _dynamicType = null;
        /// <summary>
        /// 使用emit动态生成的类型
        /// </summary>
        [XmlIgnore]
        public Type DynamicType
        {
            get
            {
                if(_dynamicType == null)
                {
                    _dynamicType = GetDynamicType(Plugin, this);
                }
                return _dynamicType;
            }
            set
            {
                _dynamicType = value;
            }
        }

        private static Assembly _dynamicAsm = null;
        private static Type GetDynamicType(DataParsePluginInfo plugin, DataView dv)
        {
            if(_dynamicAsm == null)
            {
                _dynamicAsm = Assembly.LoadFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EmitCreator.DefaultAssemblyName + ".dll"));
            }
            return _dynamicAsm.GetType($"{EmitCreator.DefaultAssemblyName}.Ns{plugin.Guid.RemoveSpecialChar()}.{dv.Type}");
        }

        //private static List<string> _baseColumns = null;        //动态类型的基本列
        //private Type CreateDynamicType(DataParsePluginInfo plugin, DataView dv)
        //{
        //    if (dv == null)
        //    {
        //        throw new Exception("加载脚本时出错！DataView为空");
        //    }
        //    if (string.IsNullOrWhiteSpace(dv.Type))
        //    {
        //        throw new Exception("加载脚本时出错！数据类型名称为空");
        //    }

        //    EmitCreator emit = new EmitCreator();
        //    emit.CreateType(dv.Type, $"{EmitCreator.DefaultAssemblyName}.Ns{plugin.Guid.RemoveSpecialChar()}", typeof(ScriptDataItem), GetInterfacesTypes(dv.Contract));

        //    if (_baseColumns == null)
        //    {
        //        _baseColumns = new List<System.Reflection.PropertyInfo>(typeof(ScriptDataItem).GetProperties()).ConvertAll(p => p.Name);
        //    }

        //    if (dv.Items != null)
        //    {
        //        foreach (var item in dv.Items)
        //        {
        //            if (_baseColumns.Contains(item.Code))       //如果基类中包含了该列，则不需要创建
        //            {
        //                continue;
        //            }
        //            var property = emit.CreateProperty(item.Code, GetColumnType(item.Type, item.Format));
        //            emit.SetPropertyAttribute(property, typeof(DisplayAttribute), null, null);
        //        }
        //    }
        //    return emit.Save();
        //}

        ///// <summary>
        ///// 协议类型转换
        ///// </summary>
        ///// <param name="contract"></param>
        ///// <returns></returns>
        //private Type[] GetInterfacesTypes(string contract)
        //{
        //    return null;
        //    //if (string.IsNullOrWhiteSpace(contract))
        //    //{
        //    //    return null;
        //    //}
        //    //List<Type> lst = new List<Type>();
        //    //foreach (var c in contract.Split(','))
        //    //{
        //    //    Type t;
        //    //    switch (c.ToLower().Trim())
        //    //    {
        //    //        case "conversion":
        //    //            t = typeof(IConversion);
        //    //            break;
        //    //        //case "datastate":
        //    //        //    t = typeof(IDataState);
        //    //        //    break;
        //    //        case "file":
        //    //            t = typeof(IFile);
        //    //            break;
        //    //        case "mail":
        //    //            t = typeof(IMail);
        //    //            break;
        //    //        case "map":
        //    //            t = typeof(IMap);
        //    //            break;
        //    //        case "thumbnail":
        //    //            t = typeof(IThumbnail);
        //    //            break;
        //    //        default:
        //    //            t = null;
        //    //            break;
        //    //    }
        //    //    if (t != null && !lst.Contains(t))
        //    //    {
        //    //        lst.Add(t);
        //    //    }
        //    //}
        //    //return lst.ToArray();
        //}

        ///// <summary>
        ///// 获取列类型
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="format"></param>
        ///// <returns></returns>
        //private Type GetColumnType(EnumColumnType type, string format)
        //{
        //    switch (type)
        //    {
        //        case EnumColumnType.DateTime:
        //            return typeof(DateTime);
        //        case EnumColumnType.Double:
        //            return typeof(double);
        //        case EnumColumnType.Enum:
        //            return type.GetType().Assembly.GetType(string.Format("XLY.SF.Project.Domains.{0}", format));
        //        case EnumColumnType.Int:
        //            return typeof(int);
        //        case EnumColumnType.List:
        //            return typeof(List<string>);
        //        default:
        //            return typeof(string);

        //    }
        //}
    }

    #endregion

    #region DataItem（扩展视图配置项）
    /// <summary>
    ///数据项配置
    /// </summary>
    [Serializable]
    public class DataItem : IEquatable<DataItem>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [XmlAttribute("code")]
        public string Code { get; set; }

        /// <summary>
        /// 视图显示宽度
        /// </summary>
        [XmlAttribute("width")]
        public int Width { get; set; }

        /// <summary>
        /// 显示格式化
        /// </summary>
        [XmlAttribute("format")]
        public string Format { get; set; }

        /// <summary>
        /// 列数据类型
        /// </summary>
        [XmlAttribute("type")]
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 排序方式，默认None不排序
        /// </summary>
        [XmlAttribute("order")]
        public EnumOrder Order { get; set; }

        /// <summary>
        /// 对齐方式，默认左对齐
        /// </summary>
        [XmlAttribute("alignment")]
        public EnumAlignment Alignment { get; set; }

        /// <summary>
        /// 分组序号设置，默认-1不分组
        /// </summary>
        [XmlAttribute("groupidex")]
        public int GroupIdex { get; set; }

        /// <summary>
        /// 是否列表显示
        /// </summary>
        [XmlAttribute("show")]
        public bool Show { get; set; }

        #region ExtendView-构造函数（初始化）

        /// <summary>
        ///  ExtendView-构造函数（初始化）
        /// </summary>
        public DataItem()
        {
            this.Width = 100;
            this.Order = EnumOrder.None;
            this.Alignment = EnumAlignment.Left;
            this.GroupIdex = -1;
            this.Show = true;
        }

        #endregion

        #region IEquatable
        /// <summary>
        /// 视图项配置是否相同
        /// </summary>
        public bool Equals(DataItem obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Name == this.Name && obj.Code == this.Name && obj.Width == this.Width && obj.Format == this.Format)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
    #endregion
}
