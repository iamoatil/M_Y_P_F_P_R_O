using System;
using System.Configuration;
using System.Reflection;
using XLY.SF.Project.IsolatedTaskEngine.Configuration;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    /// 引擎配置信息。
    /// </summary>
    public class EngineSetup
    {
        #region Fields

        private readonly IsolatedTaskSection _section;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 EngineSetupInfo 实例。
        /// </summary>
        /// <param name="sectionName">节点名称。</param>
        /// <param name="configFile">配置文件。如果为null，则使用应用程序的默认配置文件，否则使用用户指定的配置文件。</param>
        public EngineSetup(String sectionName, String configFile)
        {
            if (String.IsNullOrWhiteSpace(configFile))
            {
                _section = ConfigurationManager.GetSection(sectionName) as IsolatedTaskSection;
            }
            else
            {
                var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
                var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                if (config == null) throw new ArgumentException("fileMap");
                _section = config.GetSection(sectionName) as IsolatedTaskSection;
            }
            if (_section == null) throw new ArgumentException($"Can't find section: '{sectionName}'");
            Init();
        }

        /// <summary>
        /// 初始化类型 EngineSetupInfo 实例。
        /// </summary>
        /// <param name="entryTypeQualifiedName">注入的业务所在类型的限定名称。</param>
        public EngineSetup(String entryTypeQualifiedName)
        {
            if (String.IsNullOrWhiteSpace(entryTypeQualifiedName)) throw new ArgumentNullException("entryTypeQualifiedName");
            EntryTypeQualifiedName = entryTypeQualifiedName;
        }

        #endregion

        #region Properties

        #region EntryType

        private String _entryTypeQualifiedName;
        /// <summary>
        /// 注入的业务所在类型的限定名称。
        /// </summary>
        public String EntryTypeQualifiedName
        {
            get => _entryTypeQualifiedName;
            private set
            {
                _entryTypeQualifiedName = value;
                String[] strs = value.Split(',');
                if (strs.Length != 2) throw new ArgumentException("Invalid type name");
                Assembly assembly = Assembly.Load(strs[1]);
                if (assembly == null) throw new DllNotFoundException(strs[1]);
                Type type = assembly.GetType(strs[0]);
                if (type == null) throw new MissingMemberException(strs[0]);
                if (type.GetInterface("ITaskExecutor") == null)
                {
                    throw new TypeLoadException("This type is not a valid entry point");
                }
                if (type.GetConstructor(new Type[0]) == null)
                {
                    throw new MissingMemberException("Miss non-parameter constructor");
                }
                EntryType = type;
            }
        }

        /// <summary>
        /// 注入的业务所在类型。
        /// </summary>
        public Type EntryType { get; private set; }

        #endregion

        #region TransceiverName

        private String _transceiverName;
        /// <summary>
        /// 消息收发器名称。
        /// </summary>
        public String TransceiverName
        {
            get => _transceiverName;
            set
            {
                CheckValidation();
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("TransceiverName");
                _transceiverName = value;
            }
        }

        #endregion

        #region MaxParallelTask

        private Int32 _maxParallelTask = 1;
        /// <summary>
        /// 最大并行任务数量。
        /// </summary>
        public Int32 MaxParallelTask
        {
            get => _maxParallelTask;
            set
            {
                CheckValidation();
                if (_maxParallelTask < 1) throw new ArgumentException("value must be bigger than 0");
                _maxParallelTask = value;
            }
        }

        #endregion

        #region EnableAppDomainIsolation

        private Boolean _enableAppDomainIsolation;
        /// <summary>
        /// 是否启用AppDomain隔离任务。
        /// </summary>
        public Boolean EnableAppDomainIsolation
        {
            get => _enableAppDomainIsolation;
            set
            {
                CheckValidation();
                _enableAppDomainIsolation = value;
            }
        }

        #endregion

        internal TaskEngine Owner { get; set; }

        #endregion

        #region Methods

        #region Private

        private void Init()
        {
            EntryTypeQualifiedName = _section.EntryType;
            ParamsElement @params = _section.Params;
            if (@params == null) return;
            TransceiverName = @params.TransceiverName;
            MaxParallelTask = @params.MaxParallelTask;
            EnableAppDomainIsolation = @params.EnableAppDomainIsolation;
        }

        private void CheckValidation()
        {
            if (Owner != null && Owner.IsRuning)
            {
                throw new InvalidOperationException("The engine is runing");
            }
        }

        #endregion

        #endregion
    }
}
