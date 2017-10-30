using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例引用的设备提取项目。
    /// </summary>
    public class ReferenceItem : IEquatable<ReferenceItem>
    {
        #region Fields

        private readonly XElement _element;

        #endregion

        #region Constructors

        internal ReferenceItem(XElement element)
        {
            _element = element;
            if (_element.Attribute("Path") == null)
            {
                _element.Add(new XAttribute("Path", null));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 该对象对应的配置元素。
        /// </summary>
        internal XElement Element => _element;

        /// <summary>
        /// 设备提取项目路径。
        /// </summary>
        public String Path
        {
            get => _element.Attribute("Path").Value;
            internal set => _element.Attribute("Path").Value = value;
        }

        /// <summary>
        /// 是否是一个有效的配置。
        /// </summary>
        public Boolean IsValid => !String.IsNullOrWhiteSpace(Path);

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取对象的hash值。
        /// </summary>
        /// <returns>对象的hash值。</returns>
        public override Int32 GetHashCode()
        {
            if (!IsValid) return 0;
            return Path.GetHashCode();
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="obj">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as ReferenceItem);
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="other">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public Boolean Equals(ReferenceItem other)
        {
            if (other == null) return false;
            if (Object.ReferenceEquals(other, this)) return true;
            return other.Path == this.Path;
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="objA">实例。</param>
        /// <param name="objB">实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public static Boolean operator ==(ReferenceItem objA, ReferenceItem objB)
        {
            return Object.Equals(objA, objB);
        }

        /// <summary>
        /// 判断两个实例是否不相等。
        /// </summary>
        /// <param name="objA">实例。</param>
        /// <param name="objB">实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public static Boolean operator !=(ReferenceItem objA, ReferenceItem objB)
        {
            return !(objA == objB);
        }

        #endregion

        #region Internal

        /// <summary>
        /// 创建一个新的 ReferenceItem 类型实例。
        /// </summary>
        /// <param name="path">设备提取项目路径。</param>
        /// <returns>ReferenceItem 类型实例。</returns>
        internal static ReferenceItem New(String path)
        {
            XElement element = new XElement("Reference",
                new XAttribute("Path", path));
            return new ReferenceItem(element);
        }

        #endregion

        #endregion
    }
}
