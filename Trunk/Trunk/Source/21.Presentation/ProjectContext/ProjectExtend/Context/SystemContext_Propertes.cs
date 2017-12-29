using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MessageBase.Navigation;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.Model;
using XLY.SF.Project.ViewDomain.Model.PresentationNavigationElement;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region Event

        public event EventHandler<PropertyChangedEventArgs<Case>> CaseChanged;

        #endregion

        #region 系统时间

        private DateTime _sysStartDateTime;
        /// <summary>
        /// 系统启动时间
        /// </summary>
        public DateTime SysStartDateTime
        {
            get
            {
                return _sysStartDateTime;
            }
            private set
            {
                _sysStartDateTime = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 今日检查手机总数

        private int _todayInspectPhoneCount = 200;
        /// <summary>
        /// 今日检查手机总数
        /// </summary>
        public int TodayInspectPhoneCount
        {
            get
            {
                return _todayInspectPhoneCount;
            }

            private set
            {
                _todayInspectPhoneCount = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 存储路径

        #region SavePath

        private String _savePath;
        /// <summary>
        /// 系统资源的保存路径。
        /// </summary>
        public String SavePath
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_savePath))
                {
                    String path = Settings.GetValue(DefaultPathKey);
                    if (String.IsNullOrWhiteSpace(path))
                    {
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Windows)), "SPF");
                    }
                    CreateDirectory(path);
                    _savePath = path;
                }
                return _savePath;
            }
            set
            {
                if (_savePath != value)
                {
                    _savePath = null;
                    Settings.SetValue(DefaultPathKey, value);
                    OnPropertyChanged();
                    OnPropertyChanged("CasePath");
                    OnPropertyChanged("OperationImagePath");
                    OnPropertyChanged("CachePath");
                }
            }
        }

        #endregion

        /// <summary>
        /// 案例存储路径
        /// </summary>
        public String CasePath
        {
            get
            {
                String path = System.IO.Path.Combine(SavePath, "Cases");
                CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// 保存操作截图的路径
        /// </summary>
        public String OperationImagePath
        {
            get
            {
                String path = System.IO.Path.Combine(SavePath, "OperationImages", SysStartDateTime.Date.ToString("yyyyMMdd"));
                CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// 缓存路径
        /// </summary>
        public String CachePath
        {
            get
            {
                String path = System.IO.Path.Combine(SavePath, "Caches");
                CreateDirectory(path);
                return path;
            }
        }

        #endregion

        #region 当前登录用户信息

        #region CurUserInfo

        private UserInfoModel _user;
        /// <summary>
        /// 当前登录用户。
        /// </summary>
        public UserInfoModel CurUserInfo
        {
            get => _user;
            set
            {
                if (value != null)
                {
                    value.LoginTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _dbService.Update(value.Entity);
                    _user = value.ToReadOnly();
                }
                else
                {
                    _user = null;
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region 当前DPI

        /// <summary>
        /// 当前屏幕的X
        /// </summary>
        public int DpiX { get; private set; }

        /// <summary>
        /// 当前屏幕的Y
        /// </summary>
        public int DpiY { get; private set; }

        #endregion

        #region 案例

        private Case _currentCase;
        /// <summary>
        /// 当前打开的案例。
        /// </summary>
        public Case CurrentCase
        {
            get=> _currentCase;
            set
            {
                if (_currentCase != value)
                {
                    Case oldValue = _currentCase;
                    _currentCase = value;
                    OnPropertyChanged();
                    CaseChanged?.Invoke(this, new PropertyChangedEventArgs<Case>(oldValue, value));
                }
            }
        }

        #endregion

        #region 设备

        private DeviceExtraction _deviceExtraction;
        public DeviceExtraction DeviceExtraction
        {
            get => _deviceExtraction;
            set
            {
                _deviceExtraction = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 可以用于异步操作的对象
        /// <summary>
        /// 可以用于异步操作的对象，主要用于非UI线程中替换dispatcher
        /// </summary>
        public AsyncOperation AsyncOperation { get; private set; }
        /// <summary>
        /// 加载异步操作对象
        /// </summary>
        public void LoadAsyncOperation()
        {
            AsyncOperation = AsyncOperationManager.CreateOperation(this);
        }
        #endregion

        #region 语言

        public static LanguageType Language
        {
            get
            {
                String str = Settings.GetValue(LanguageKey) ?? String.Empty;
                switch (str.ToLower())
                {
                    case "en":
                        return LanguageType.En;
                    default:
                        return LanguageType.Cn;
                }
            }
        }

        public static XmlDataProvider LanguageProvider { get; }

        public static LanguageManager LanguageManager { get; }

        #endregion

        #region 界面缓存

        /// <summary>
        /// 当前界面缓存
        /// </summary>
        public NavigationCacheManager<PreCacheToken> CurCacheViews { get; }

        #endregion

        #region 常量

        public const String EnableInspectionKey = "enableInspection";

        public const String LanguageKey = "language";

        public const String EnableFilterKey = "enableFilter";

        public const String DefaultPathKey = "defaultPath";

        #endregion

        #region private

        #region 推荐方案【目前先全部缓存在内存】

        /// <summary>
        /// 推荐方案
        /// </summary>
        private List<StrategyElement> SolutionProposed { get; set; }

        #endregion

        #endregion
    }
}
