using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例。
    /// </summary>
    public sealed class Case
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

        private static readonly Dictionary<Guid, CaseWatcher> CaseWatchers = new Dictionary<Guid, CaseWatcher>();

        private readonly CPConfiguration _configuration;

        private readonly RestrictedCaseInfo _caseInfo;

        private const String DefaultProjectFile = "CaseProject";

        #endregion

        #region Constructors

        private Case(RestrictedCaseInfo caseInfo, CPConfiguration configuration,String projectFile)
        {
            Token = Guid.NewGuid();
            _caseInfo = caseInfo;
            ProjectFile = projectFile;
            _configuration = configuration;
            RegisterCaseWatcher(this,Path, ()=> Delete(true));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 案例配置。
        /// </summary>
        internal CPConfiguration Configuration
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return _configuration;
            }
        }

        /// <summary>
        /// 案例信息。
        /// </summary>
        public RestrictedCaseInfo CaseInfo => _caseInfo;

        /// <summary>
        /// 案例所在路径。
        /// </summary>
        public String Path => _caseInfo.Path;

        /// <summary>
        /// 项目文件路径。
        /// </summary>
        public String ProjectFile { get; }

        /// <summary>
        /// 案例名称。
        /// </summary>
        public String Name => CaseInfo.Name;

        /// <summary>
        /// 案例是否存在。
        /// </summary>
        public Boolean Existed { get; private set; } = true;

        /// <summary>
        /// 该案例关联的设备提取列表。
        /// </summary>
        public IEnumerable<DeviceExtraction> DeviceExtractions
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return GetAllDeviceExtractions();
            }
        }

        /// <summary>
        /// 标识。用于标识该实例与某个案例关联。同一案例及其设备提取、提取项都具有相同的标识。
        /// </summary>
        internal Guid Token { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 创建新的案例。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        /// <param name="directory">案例目录的父级路径。</param>
        /// <param name="projectFileNameWithoutExtension">不包含扩展名的项目文件名称。</param>
        /// <returns>新的案例。</returns>
        public static Case New(CaseInfo caseInfo, String directory, String projectFileNameWithoutExtension = null)
        {
            if (caseInfo == null) throw new ArgumentNullException("caseInfo");
            CPConfiguration configuration = CPConfiguration.Create(caseInfo);
            if (configuration == null) return null;
            String path = InnerHelper.GetValidDirectory(System.IO.Path.Combine(directory, caseInfo.Name));
            caseInfo.Path = path;
            String file = System.IO.Path.Combine(path, $"{projectFileNameWithoutExtension ?? DefaultProjectFile}.cp");
            if (!configuration.Save(file)) return null;
            RestrictedCaseInfo rci = configuration.GetCaseInfo(System.IO.Path.GetDirectoryName(file));
            return new Case(rci, configuration, file);
        }

        /// <summary>
        /// 打开案例。
        /// </summary>
        /// <param name="file">案列的项目文件路径。</param>
        /// <returns>案例。</returns>
        public static Case Open(String file)
        {
            CPConfiguration configuration = CPConfiguration.Open(file);
            if (configuration == null) return null;
            RestrictedCaseInfo caseInfo = configuration.GetCaseInfo(System.IO.Path.GetDirectoryName(file));
            return new Case(caseInfo, configuration, file);
        }

        /// <summary>
        /// 删除案例。
        /// </summary>
        public void Delete()
        {
            Delete(false);
        }

        /// <summary>
        /// 更新案例信息。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Update()
        {
            ThrowExceptionIfNotExisted();
            _configuration.SetCaseInfo(CaseInfo);
            if (_configuration.Save(ProjectFile))
            {
                CaseInfo.Commit();
                Updated?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加设备提取。
        /// </summary>
        /// <param name="directory">保存目录的相对或绝对路径。</param>
        /// <param name="type">设备类型。</param>
        /// <param name="fileNameWithoutExtension">配置文件名（不含扩展名）。</param>
        /// <returns></returns>
        public DeviceExtraction CreateDeviceExtraction(String directory, String type, String fileNameWithoutExtension = null)
        {
            ThrowExceptionIfNotExisted();
            DeviceExtraction de = DeviceExtraction.Create(directory, type, fileNameWithoutExtension, this);
            if (de != null)
            {
                Configuration.AddReference(de.Reference);
                if (Configuration.Save(ProjectFile))
                {
                    return de;
                }
                de.Delete();
            }
            return null;
        }

        /// <summary>
        /// 将一个提取设备与当前案例关联。
        /// </summary>
        /// <param name="de">提取设备。</param>
        public void Attach(DeviceExtraction de)
        {
            if (de == null) return;
            if (Configuration.AddReference(de.Reference))
            {
                Configuration.Save(ProjectFile);
            }
        }

        /// <summary>
        /// 将一个提取设备与当前案例分离。
        /// </summary>
        /// <param name="de">提取设备。</param>
        public void Detach(DeviceExtraction de)
        {
            if (de == null) return;
            if (Configuration.RemoveReference(de.Reference))
            {
                Configuration.Save(ProjectFile);
            }
        }

        #endregion

        #region Internal

        internal static void RegisterPath(Guid token, String path, Action callback)
        {
            CaseWatcher watcher = GetCaseWatcher(token);
            watcher?.Register(path, callback);
        }

        internal static void UnregisterPath(Guid token, String path)
        {
            CaseWatcher watcher = GetCaseWatcher(token);
            watcher?.Unregister(path);
        }

        #endregion

        #region Private

        private void Delete(Boolean isEvent)
        {
            if (!Existed) return;
            UnregisterCaseWatcher(this);
            if (!isEvent)
            {
                Directory.Delete(Path, true);
            }
            Existed = false;
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        private void ThrowExceptionIfNotExisted()
        {
            if (Existed) return;
            throw new InvalidOperationException("This case has been deleted.");
        }

        /// <summary>
        /// 获取所有的设备提取。
        /// </summary>
        /// <param name="references">案例引用的设备提取。</param>
        /// <returns>设备提取列表。</returns>
        private IEnumerable<DeviceExtraction> GetAllDeviceExtractions()
        {
            return Configuration.ReferenceItems.Where(x => x.IsValid).Select(x => DeviceExtraction.Open(x, this)).Where(x => x != null);
        }

        private static void RegisterCaseWatcher(Case @case,String initPath,Action initCallback)
        {
            if (CaseWatchers.ContainsKey(@case.Token)) return;
            CaseWatcher watcher = new CaseWatcher(@case);
            watcher.Register(initPath, initCallback);
            CaseWatchers.Add(@case.Token, watcher);
        }

        private static void UnregisterCaseWatcher(Case @case)
        {
            CaseWatcher watcher = GetCaseWatcher(@case.Token);
            if (watcher == null) return;
            watcher.Dispose();
            CaseWatchers.Remove(watcher.Token);
        }

        private static CaseWatcher GetCaseWatcher(Guid token)
        {
            if (CaseWatchers.ContainsKey(token))
            {
                return CaseWatchers[token];
            }
            return null;
        }

        #endregion

        #endregion
    }
}
