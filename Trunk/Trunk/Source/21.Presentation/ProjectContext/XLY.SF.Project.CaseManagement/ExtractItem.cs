using System;
using System.IO;
using System.Xml.Linq;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 设备提取项。
    /// </summary>
    public class ExtractItem : IEquatable<ExtractItem>
    {
        #region Event

        /// <summary>
        /// 删除事件。
        /// </summary>
        public event EventHandler Deleted;

        #endregion

        #region Fields

        private readonly XElement _element;

        #endregion

        #region Constructors

        internal ExtractItem(XElement element,DeviceExtraction owner)
        {
            _element = element;
            Owner = owner;
            if (_element.Attribute("Mode") == null)
            {
                _element.Add(new XAttribute("Mode", null));
            }
            if (_element.Attribute("Path") == null)
            {
                _element.Add(new XAttribute("Path", null));
            }
            Case.RegisterPath(Token, Path, ()=>Delete(true));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 该对象对应的配置元素。
        /// </summary>
        internal XElement Element => _element;

        /// <summary>
        /// 提取方式。
        /// </summary>
        public String Mode
        {
            get => _element.Attribute("Mode").Value;
            internal set => _element.Attribute("Mode").Value = value;
        }

        /// <summary>
        /// 保存路径。
        /// </summary>
        public String Path
        {
            get
            {
                String path = _element.Attribute("Path").Value;
                if (System.IO.Path.IsPathRooted(path)) return path;
                return System.IO.Path.Combine(Owner.Path, path);
            }
            internal set => _element.Attribute("Path").Value = value;
        }

        /// <summary>
        /// 是否是一个有效的配置。
        /// </summary>
        public Boolean IsValid => !String.IsNullOrWhiteSpace(Mode) && !String.IsNullOrWhiteSpace(Path);

        /// <summary>
        /// 目录是否存在。
        /// </summary>
        public Boolean Existed => IsValid && Directory.Exists(Path);

        /// <summary>
        /// 拥有此提取项的设备。
        /// </summary>
        internal DeviceExtraction Owner { get; }

        /// <summary>
        /// 标识。用于标识该实例与某个案例关联。同一案例及其设备提取、提取项都具有相同的标识。
        /// </summary>
        internal Guid Token => Owner.Token;

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
            return Mode.GetHashCode() ^ Path.GetHashCode();
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="obj">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as ExtractItem);
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="other">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public Boolean Equals(ExtractItem other)
        {
            if (other == null) return false;
            if (Object.ReferenceEquals(other, this)) return true;
            return other.Mode == this.Mode
                && other.Path == this.Path;
        }

        /// <summary>
        /// 删除提取项。
        /// </summary>
        public void Delete()
        {
            Delete(false);
        }

        /// <summary>
        /// 判断两个实例是否相等。
        /// </summary>
        /// <param name="objA">实例。</param>
        /// <param name="objB">实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public static Boolean operator ==(ExtractItem objA, ExtractItem objB)
        {
            return Object.Equals(objA, objB);
        }

        /// <summary>
        /// 判断两个实例是否不相等。
        /// </summary>
        /// <param name="objA">实例。</param>
        /// <param name="objB">实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public static Boolean operator !=(ExtractItem objA, ExtractItem objB)
        {
            return !(objA == objB);
        }

        #endregion

        #region Internal

        /// <summary>
        /// 创建一个新的 ExtractItem 类型实例。
        /// </summary>
        /// <param name="mode">提取方式。</param>
        /// <param name="path">路径。</param>
        /// <param name="owner">拥有该提取项的设备。</param>
        /// <returns>ExtractItem 类型实例。</returns>
        internal static ExtractItem New(String mode, String path,DeviceExtraction owner)
        {
            XElement element = new XElement("Extract",
                new XAttribute("Mode", mode),
                new XAttribute("Path", path));
            return new ExtractItem(element, owner);
        }

        #endregion

        #region Private

        /// <summary>
        /// 删除提取项。
        /// </summary>
        private void Delete(Boolean isEvent)
        {
            Case.UnregisterPath(Token, Path);
            if (!isEvent)
            {
                if (!Existed) return;
                Directory.Delete(Path, true);
            }
            if (Owner.Configuration.RemoveExtract(this, Owner))
            {
                Owner.Configuration.Save(Owner.ConfigurationFile);
            }
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion
    }
}
