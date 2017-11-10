using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Linq;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/27 11:14:19
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Language
{
    /// <summary>
    /// 语言管理器。
    /// </summary>
    public class LanguageManager
    {
        #region Event

        /// <summary>
        /// 语言切换事件。
        /// </summary>
        public event EventHandler Switched;

        #endregion

        #region Fields

        private readonly Func<LanguageType, Stream> _xmlStreamCreator;

        private static readonly Dictionary<String, LanguageManager> Managers = new Dictionary<String, LanguageManager>();

        private XmlDocument _doc;

        private static readonly LanguageDescriptorComparer Comparer = new LanguageDescriptorComparer();

        private static readonly LanguageManager Empty = new LanguageManager();

        #endregion

        #region Constructors

        private LanguageManager(Func<LanguageType, Stream> xmlStreamCreator)
        {
            _xmlStreamCreator = xmlStreamCreator ?? throw new ArgumentNullException("xmlStreamCreator");
        }

        private LanguageManager()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 语言XML文档。
        /// </summary>
        public XmlDocument Document => _doc;

        /// <summary>
        /// 获取特定键的文本。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>文本。</returns>
        public String this[String key]
        {
            get
            {
                if (String.IsNullOrWhiteSpace(key) || _doc == null) return null;
                return _doc.SelectSingleNode(key)?.InnerText;
            }
        }

        /// <summary>
        /// 当前选择的语言类型。
        /// </summary>
        public LanguageType Type { get; private set; }

        /// <summary>
        /// 当前已注册的所有语言管理器选择的语言类型。
        /// </summary>
        public static LanguageType GlobalType { get; private set; }

        /// <summary>
        /// 获取当前程序集的语言管理器。
        /// </summary>
        public static LanguageManager Current
        {
            get
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                return GetCallAssemblyManager(assembly);
            }
        }

        /// <summary>
        /// 一个空的语言管理器。
        /// 所有通过Current属获取，但未使用 LanguageDescriptorAttribute 特性标记的程序集都使用该空语言管理器。
        /// </summary>
        public Boolean IsEmpty => _xmlStreamCreator == null;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 注册一个语言管理器。
        /// </summary>
        /// <param name="key">该语言管理器的标识。</param>
        /// <param name="xmlStreamCreator">用于在语言切换时获取语言配置的回调方法。</param>
        /// <returns>LanguageManager 类型实例。</returns>
        public static LanguageManager Register(String key, Func<LanguageType, Stream> xmlStreamCreator)
        {
            if (Managers.ContainsKey(key)) throw new InvalidOperationException("The key is exsited");
            LanguageManager manager = new LanguageManager(xmlStreamCreator);
            Managers.Add(key, manager);
            manager.Switch(GlobalType);
            return manager;
        }

        /// <summary>
        /// 注销一个语言管理器。
        /// </summary>
        /// <param name="key">该语言管理器的标识。</param>
        public static void Unregister(String key)
        {
            Managers.Remove(key);
        }

        /// <summary>
        /// 获取特定的语言管理器。
        /// </summary>
        /// <param name="key">该语言管理器的标识。</param>
        /// <returns>LanguageManager 类型实例。</returns>
        public static LanguageManager GetManager(String key)
        {
            if (Managers.ContainsKey(key))
            {
                return Managers[key];
            }
            return null;
        }

        /// <summary>
        /// 切换所有已注册模块的语言。
        /// </summary>
        /// <param name="type">语言类型。</param>
        public static void SwitchAll(LanguageType type)
        {
            if (type == GlobalType) return;
            GlobalType = type;
            foreach (LanguageManager item in Managers.Values)
            {
                item.Switch(type);
            }
            Empty.Switch(type);
        }

        /// <summary>
        /// 切换语言。
        /// </summary>
        /// <param name="type">语言类型。</param>
        public void Switch(LanguageType type)
        {
            if (type == Type) return;
            Stream stream = null;
            try
            {
                if (IsEmpty) return;
                stream = _xmlStreamCreator(type);
                if (stream == null) return;
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                _doc = doc;
            }
            catch (XmlException)
            {
                _doc = new XmlDocument();
            }
            finally
            {
                stream?.Close();
                Type = type;
                Switched?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private

        private static LanguageManager GetCallAssemblyManager(Assembly assembly)
        {
            if (!assembly.IsDefined(typeof(LanguageDescriptorAttribute)))
            {
                return Empty;
            }
            if (Managers.ContainsKey(assembly.FullName))
            {
                return Managers[assembly.FullName];
            }
            return Register(assembly.FullName, (t) => GetCallAssemblyResourceStream(assembly, t));
        }

        private static Stream GetCallAssemblyResourceStream(Assembly assembly, LanguageType type)
        {
            var attrs = assembly.GetCustomAttributes<LanguageDescriptorAttribute>();
            var found = attrs.FirstOrDefault(x => x.Type == type);
            if (found == null) return null;
            String resourceName = $"{assembly.GetName().Name}.{found.EmbedResourceName}";
            return assembly.GetManifestResourceStream(resourceName);
        }

        #endregion

        #endregion
    }
}
