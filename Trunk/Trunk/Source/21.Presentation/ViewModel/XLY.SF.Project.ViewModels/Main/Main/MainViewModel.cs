using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.Main;
using System.IO;
using XLY.SF.Framework.Language;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:38:50
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.ModuleMainViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        #region private

        /// <summary>
        /// 数据库服务
        /// </summary>
        private IDatabaseContext _dbService;

        /// <summary>
        /// 消息服务
        /// </summary>
        private IMessageBox _messageBox { get; set; }

        #endregion

        /// <summary>
        /// 主界面导航管理器
        /// </summary>
        public MainNavigationManager MainNavigation { get; set; }

        #region 当前案例名称

        private string _curCaseName;
        /// <summary>
        /// 当前案例名称
        /// </summary>
        public string CurCaseName
        {
            get
            {
                return this._curCaseName;
            }

            set
            {
                this._curCaseName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region Model

        private MainModel _mainInfo;
        /// <summary>
        /// 主界面展示信息
        /// </summary>
        public MainModel MainInfo
        {
            get
            {
                return this._mainInfo;
            }

            set
            {
                this._mainInfo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// 结束程序
        /// </summary>
        public ProxyRelayCommand ShutdownProgramCommand { get; set; }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public ProxyRelayCommand CloseCaseCommand { get; set; }

        #region 菜单功能

        /// <summary>
        /// 用户管理
        /// </summary>
        public ProxyRelayCommand UserManagementCommand { get; set; }
        /// <summary>
        /// 案例管理
        /// </summary>
        public ProxyRelayCommand CaseManagementCommand { get; set; }
        /// <summary>
        /// 系统设置
        /// </summary>
        public ProxyRelayCommand SysSettingCommand { get; set; }
        /// <summary>
        /// 插件管理
        /// </summary>
        public ProxyRelayCommand PluginManagementCommand { get; set; }
        /// <summary>
        /// 系统日志
        /// </summary>
        public ProxyRelayCommand SysLogCommand { get; set; }
        /// <summary>
        /// 系统授权
        /// </summary>
        public ProxyRelayCommand SysEmpowerCommand { get; set; }
        /// <summary>
        /// 用户反馈
        /// </summary>
        public ProxyRelayCommand UserFeedbackCommand { get; set; }
        /// <summary>
        /// 升级
        /// </summary>
        public ProxyRelayCommand SysUpdateCommand { get; set; }
        /// <summary>
        /// 帮助
        /// </summary>
        public ProxyRelayCommand HelpCommand { get; set; }
        /// <summary>
        /// 关于我们
        /// </summary>
        public ProxyRelayCommand AboutCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ProxyRelayCommand LogoutSysCommand { get; set; }

        #endregion

        #endregion

        #endregion

        #region Constructors

        [ImportingConstructor]
        public MainViewModel(IDatabaseContext dbService, IMessageBox messageBox)
        {
            _dbService = dbService;
            _messageBox = messageBox;

            MainNavigation = new MainNavigationManager();
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;

            //事件注册
            ShutdownProgramCommand = new ProxyRelayCommand(ExecuteShutdownProgramCommand);
            CloseCaseCommand = new ProxyRelayCommand(ExecuteCloseCaseCommand);
            UserManagementCommand = new ProxyRelayCommand(ExecuteUserManagementCommand);


        }

        private void Instance_CaseChanged(object sender, PropertyChangedEventArgs<Project.CaseManagement.Case> e)
        {
            CurCaseName = e.NewValue.Name;
        }

        #endregion

        #region 重载

        protected override void Closed()
        {

        }

        protected override void LoadCore(object parameters)
        {
            MainInfo = new MainModel()
            {
                CurUserName = SystemContext.Instance.CurUserInfo?.UserName,
                CurSysTime = DateTime.Now
            };
            //加载首页
            base.NavigationForMainWindow(ExportKeys.HomePageView);
        }

        #endregion

        #region ExecuteCommand

        #region 菜单操作

        //用户管理
        private string ExecuteUserManagementCommand()
        {
            return "打开用户管理";
        }

        #endregion

        //关程序
        private string ExecuteShutdownProgramCommand()
        {
            SysCommonMsgArgs<string> args = new SysCommonMsgArgs<string>(SystemKeys.ShutdownProgram);
            base.MessageAggregation.SendSysMsg(args);
            return LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MainWin_OpLogShutdown);
        }

        //关闭按钮
        private string ExecuteCloseCaseCommand()
        {
            string tmpCaseName = SystemContext.Instance.CurrentCase?.Name;
            SystemContext.Instance.CurrentCase = null;
            base.NavigationForMainWindow(ExportKeys.HomePageView);
            return string.Format("{0}{1}", LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MainWin_ToolTipCloseCase), tmpCaseName);
        }

        #endregion
    }
}
