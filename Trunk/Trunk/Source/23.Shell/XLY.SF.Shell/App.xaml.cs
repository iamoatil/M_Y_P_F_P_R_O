using System.Diagnostics;
using System.Linq;
using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Language;
using XLY.SF.Shell.NavigationManager;
using XLY.SF.Shell.SpfException;
using XLY.SF.Framework.BaseUtility;
using System.Collections.Generic;
using XLY.SF.Project.ViewDomain.MefKeys;
using ProjectExtend.Context;

namespace XLY.SF.Shell
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        #region 属性定义

        /// <summary>
        /// 导航管理器（只针对窗体）
        /// </summary>
        private WindowNavigationHelper _navigationManager;

        /// <summary>
        /// 异常服务
        /// </summary>
        private ExceptionHelper _exceptionHelper;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            Init();
            Load();
        }

        private void Init()
        {
            //加载IOC容器
            IocManagerSingle.Instance.LoadParts(GetType().Assembly);

            //IocManagerSingle.Instance.AddExportModule(this.GetType().Assembly);
            //创建窗体导航服务
            _navigationManager = new WindowNavigationHelper();
            //错误监听
            _exceptionHelper = new ExceptionHelper();
            //创建主窗体
            Current.MainWindow = new Shell()
            {
                CanResize = false
            };
            Current.MainWindow.WindowState = WindowState.Maximized;

            //监听系统消息
            MsgAggregation.Instance.RegisterSysMsg<string>(this, SystemKeys.ShutdownProgram, ShutdownProgramCallback);
        }

        private void Load()
        {
            //初始化异常服务（既：监听未捕获异常）
            _exceptionHelper.Init();
            //注册窗口导航消息
            _navigationManager.RegisterNavigation();
            //确认当前设置的语言
            LoadCurrentLanguage();
            //开始加载程序（初始化）
            _navigationManager.ExecuteProgramInitialise();
            //此加载模块，有需要时可以打开使用
            //var c = IocManagerSingle.Instance.GetPart<IModule>(ExportKeys.LoadModule);
            //c.InitModule();
            ProjectExtend.Context.SystemContext.Instance.LoadAsyncOperation();
        }

        #region 读取当前语言设置

        /// <summary>
        /// 加载当前语言设置
        /// </summary>
        private void LoadCurrentLanguage()
        {
            var configHelper = IocManagerSingle.Instance.GetPart<ISystemConfigService>(CoreExportKeys.SysConfigHelper);
            var curLanguage = configHelper.GetSysConfigValueByKey("Language");
            //转换语言配置，默认为中文
            var curLangType = curLanguage.ToSafeEnum<LanguageType>(LanguageType.Cn);
#if DEBUG
            SystemContext.LanguageManager.Switch(curLangType);
#else

            LanguageManager.SwitchAll(curLangType);
#endif
        }

        #endregion

        #region 确认是否关闭程序

        private void ShutdownProgramCallback(SysCommonMsgArgs<string> args)
        {
            IMessageBox _msgBox = IocManagerSingle.Instance.GetPart<IMessageBox>();
            if (!string.IsNullOrWhiteSpace(args.Parameters))
            {
                if (_msgBox.ShowDialogWarningMsg(args.Parameters))
                    Application.Current.Shutdown();
            }
            else
                Application.Current.Shutdown();
        }

        #endregion
    }
}
