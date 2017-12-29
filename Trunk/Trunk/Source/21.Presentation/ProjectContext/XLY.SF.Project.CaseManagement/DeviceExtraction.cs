using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 设备提取。
    /// </summary>
    public class DeviceExtraction
    {
        #region Event

        /// <summary>
        /// 删除事件。
        /// </summary>
        public event EventHandler Deleted;

        /// <summary>
        /// 信息更新事件。
        /// </summary>
        public event EventHandler Updated;

        #endregion

        #region Fields

        private readonly DPConfiguration _configuration;

        private const String DefaultDeviceExtractionConfigFile = "DeviceExtraction";

        #endregion

        #region Constructors

        private DeviceExtraction(DPConfiguration configuration,String configFile, ReferenceItem item, Case owner)
        {
            _configuration = configuration;
            ConfigurationFile = configFile;
            Path = System.IO.Path.GetDirectoryName(configFile);
            Reference = item;
            Owner = owner;
            Case.RegisterPath(Token, Path, () => Delete(true));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 设备提取配置。
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal DPConfiguration Configuration
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return _configuration;
            }
        }

        /// <summary>
        /// 唯一标识。
        /// </summary>
        public String Id
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return Configuration.Id;
            }
        }

        /// <summary>
        /// 设备类型。
        /// </summary>
        public String Type
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return Configuration.Type;
            }
        }

        /// <summary>
        /// 设备提取是否存在。
        /// </summary>
        public Boolean Existed => File.Exists(ConfigurationFile);

        /// <summary>
        /// 获取当前设备所有的提取。
        /// </summary>
        /// <returns>提取列表。</returns>
        public IEnumerable<ExtractItem> ExtractItems
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return GetExtractItems();
            }
        }

        /// <summary>
        /// 设备提取配置文件所在目录。
        /// </summary>
        public String Path { get; }

        /// <summary>
        /// 案例项目文件名称。
        /// </summary>
        public String ConfigurationFile { get; }

        /// <summary>
        /// 获取节点PropertyGroup中定义的元素名称。
        /// </summary>
        public IEnumerable<String> PropertNames => Configuration.GetPropertyNames();

        /// <summary>
        /// 获取配置中指定名称的自定义属性。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>属性值。</returns>
        public String this[String propertyName]
        {
            get => Configuration.GetProperty(propertyName);
            set => Configuration.SetProperty(propertyName, value);
        }

        /// <summary>
        /// 此设备对应的 ReferenceItem 实例。
        /// </summary>
        internal ReferenceItem Reference { get; }

        /// <summary>
        /// 拥有此设备的案例。
        /// </summary>
        public Case Owner { get; }

        /// <summary>
        /// 标识。用于标识该实例与某个案例关联。同一案例及其设备提取、提取项都具有相同的标识。
        /// </summary>
        internal Guid Token => Owner.Token;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Save()
        {
            ThrowExceptionIfNotExisted();
            if (Configuration.Save(ConfigurationFile))
            {
                Updated?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加一个提取。
        /// </summary>
        /// <param name="mode">提取模式。</param>
        /// <param name="directory">目录名称。</param>
        /// <returns>ExtractItem 类型实例。</returns>
        public ExtractItem CreateExtract(String mode, String directory)
        {
            ThrowExceptionIfNotExisted();
            ExtractItem item = Configuration.AddExtract(mode, directory, this);
            if (item == null) return null;
            if (!Directory.Exists(item.Path))
            {
                try
                {
                    Directory.CreateDirectory(item.Path);
                }
                catch (Exception)
                {
                    Configuration.RemoveExtract(item,this);
                    return null;
                }
            }
            if (!Configuration.Save(ConfigurationFile))
            {
                Configuration.RemoveExtract(item, this);
                return null;
            }
            return item;
        }

        /// <summary>
        /// 将一个提取项与当前提取设备关联。
        /// </summary>
        /// <param name="item">提取项。</param>
        public void Attach(ExtractItem item)
        {
            if (Configuration.AddExtract(item, this))
            {
                Configuration.Save(ConfigurationFile);
            }
        }

        /// <summary>
        /// 将一个提取项与当前提取设备分离。
        /// </summary>
        /// <param name="item">提取项。</param>
        public void Detach(ExtractItem item)
        {
            if (Configuration.RemoveExtract(item, this))
            {
                Configuration.Save(ConfigurationFile);
            }
        }

        /// <summary>
        /// 删除设备提取。
        /// </summary>
        public void Delete()
        {
            Delete(false);
        }

        #endregion

        #region Internal

        /// <summary>
        /// 打开已有的设备提取。
        /// </summary>
        /// <param name="owner">所属案例。</param>
        /// <param name="reference">案例引用的设备提取。</param>
        /// <returns>DeviceExtraction 类型实例。</returns>
        internal static DeviceExtraction Open(ReferenceItem reference, Case owner)
        {
            String configFile = reference.Path;
            if (!System.IO.Path.IsPathRooted(configFile))
            {
                configFile = System.IO.Path.Combine(owner.Path, reference.Path);
            }
            DPConfiguration configuration = DPConfiguration.Open(configFile);
            if (configuration == null) return null;
            return new DeviceExtraction(configuration, configFile, reference, owner);
        }

        /// <summary>
        /// 创建新的设备提取。
        /// </summary>
        /// <param name="type">设备类型。</param>
        /// <param name="directory">设备目录的相对或绝对路径。</param>
        /// <param name="fileNameWithoutExtension">配置文件名称（不含扩展名）。</param>
        /// <param name="owner">创建该设备的案例。</param>
        /// <returns>DeviceExtraction 类型实例。</returns>
        internal static DeviceExtraction Create(String directory, String type, String fileNameWithoutExtension, Case owner)
        {
            DPConfiguration configuration = DPConfiguration.Create(type);
            if (configuration == null) return null;
            Boolean isRelativePath = String.IsNullOrWhiteSpace(System.IO.Path.GetPathRoot(directory));
            if (isRelativePath)
            {
                return CreateByRelativePath(directory, fileNameWithoutExtension, owner, configuration);
            }
            else
            {
                return CreateByAbsolutedPath(directory, fileNameWithoutExtension, owner, configuration);
            }
        }

        #endregion

        #region Private

        private static DeviceExtraction CreateByRelativePath(String directory, String fileNameWithoutExtension, Case owner, DPConfiguration configuration)
        {
            //取得相对路径的目录部分用于后面生成新的相对路径
            //因为GetValidDirectory可能会返回不同的目录名称
            //因此需要重新拼接
            String relativeParentDirectory = System.IO.Path.GetDirectoryName(directory);
            directory = InnerHelper.GetValidDirectory(System.IO.Path.Combine(owner.Path, directory));
            String directoryName = System.IO.Path.GetFileName(directory);
            directory = System.IO.Path.Combine(relativeParentDirectory, directoryName);
            String relatvieFile = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension ?? DefaultDeviceExtractionConfigFile}.dp");
            ReferenceItem reference = ReferenceItem.New(relatvieFile);
            String fullPath = System.IO.Path.Combine(owner.Path, relatvieFile);
            //保存配置文件
            if (configuration.Save(fullPath))
            {
                return new DeviceExtraction(configuration, fullPath, reference, owner);
            }
            return null;
        }

        private static DeviceExtraction CreateByAbsolutedPath(String directory, String fileNameWithoutExtension, Case owner, DPConfiguration configuration)
        {
            directory = InnerHelper.GetValidDirectory(directory);
            String file = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension ?? DefaultDeviceExtractionConfigFile}.dp");
            ReferenceItem reference = ReferenceItem.New(file);
            //保存配置文件
            if (configuration.Save(file))
            {
                return new DeviceExtraction(configuration, file, reference, owner);
            }
            return null;
        }

        private void Delete(Boolean isEvent)
        {
            Case.UnregisterPath(Token, Path);
            if (Owner.Existed && Owner.Configuration.RemoveReference(Reference))
            {
                Owner.Configuration.Save(Owner.ProjectFile);
            }
            if (!isEvent && Existed)
            {
                Directory.Delete(Path, true);
            }
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        private void ThrowExceptionIfNotExisted()
        {
            if (Existed) return;
            throw new InvalidOperationException("This device extraction has been deleted.");
        }

        private IEnumerable<ExtractItem> GetExtractItems(Boolean includeInvalid = false)
        {
            if (includeInvalid)
            {
                return Configuration.GetAllExtractItems(this);
            }
            return Configuration.GetAllExtractItems(this).Where(x => x.Existed);
        }

        #endregion

        #endregion
    }
}
