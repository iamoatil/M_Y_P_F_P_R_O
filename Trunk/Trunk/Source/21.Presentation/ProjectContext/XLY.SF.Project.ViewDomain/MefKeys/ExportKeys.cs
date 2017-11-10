using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.MefKeys
{
    public class ExportKeys
    {
        #region 其他

        #region 单例包装器
        /// <summary>
        /// 单例包装器
        /// </summary>
        public const string OtherSingleWrapper = "ExportKeys_OtherSingleWrapper";

        #endregion

        #region 配置文件Helper
        /// <summary>
        /// 配置文件Helper
        /// </summary>
        public const string OtherSysConfigHelper = "ExportKey_OtherSysConfigHelper";

        #endregion

        #region 消息框
        /// <summary>
        /// 消息框
        /// </summary>
        public const string OtherMessageBox = "ExportKey_OtherMessageBox";

        #endregion

        #region 模块接口
        /// <summary>
        /// 模块接口
        /// </summary>
        public const string OtherLoadModule = "ExportKey_OtherLoadModule";

        #endregion

        #endregion

        #region 模块

        #region 登录

        /// <summary>
        /// 登录View
        /// </summary>
        public const string ModuleLoginView = "ExportKey_ModuleLoginView";
        /// <summary>
        /// 登录ViewModel
        /// </summary>
        public const string ModuleLoginViewModel = "ExportKey_ModuleLoginViewModel";

        #endregion

        #region 加载

        /// <summary>
        /// 加载View
        /// </summary>
        public const string ModuleLoadingView = "ExportKey_ModuleLoadingView";
        /// <summary>
        /// 加载ViewModel
        /// </summary>
        public const string ModuleLoadingViewModel = "ExportKey_ModuleLoadingViewModel";

        #endregion

        #region 主界面

        /// <summary>
        /// 主界面View
        /// </summary>
        public const string ModuleMainUcView = "ExportKey_ModuleMainUcView";
        /// <summary>
        /// 主界面ViewModel
        /// </summary>
        public const string ModuleMainViewModel = "ExportKey_ModuleMainViewModel";

        public const String DeviceListView = "ExportKey_DeviceListView";
        public const String DeviceListViewModel = "ExportKey_DeviceListViewModel";

        /// <summary>
        /// 设备主页
        /// </summary>
        public const string DeviceHomePageView = "ExportKey_DeviceHomePageView";
        public const string DeviceHomePageViewModel = "ExportKey_DeviceHomePageViewModel";

        #endregion

        #region 首页

        /// <summary>
        /// 首页View
        /// </summary>
        public const string HomePageView = "ExportKey_HomePageView";
        /// <summary>
        /// 首页ViewModel
        /// </summary>
        public const string HomePageViewModel = "ExportKey_HomePageViewModel";

        #endregion

        #region 模版

        /// <summary>
        /// 模版
        /// </summary>
        public const string ViewTemplateView = "ExportKey_ViewTemplateView";
        /// <summary>
        /// 模版
        /// </summary>
        public const string ViewTemplateViewModel = "ExportKey_ViewTemplateViewModel";

        #endregion

        #region 案例

        public const string CaseCreationView = "ExportKey_CaseCreationView";

        public const string CaseCreationViewModel = "ExportKey_CaseCreationViewModel";

        public const string CaseListView = "ExportKey_CaseListView";

        public const string CaseListViewModel = "ExportKey_CaseListViewModel";

        public const string CaseSelectionView = "ExportKey_CaseSelectionView";

        public const string CaseSelectionViewModel = "ExportKey_CaseSelectionViewModel";

        public const string DeviceWindowContentView = "ExportKey_DeviceWindowContentView";

        public const string DeviceWindowContentViewModel = "ExportKey_DeviceWindowContentViewModel";

        public const string DeviceWindowClosedMsg = "DeviceWindowClosed";

        #endregion

        #region 数据源选择
        public const string DeviceSelectView = "ExportKey_DeviceSelectView";
        public const string DeviceSelectViewModel = "ExportKey_DeviceSelectViewModel";
        public const string DeviceSelectFileView = "DeviceSelectFileView";
        public const string DeviceSelectFileViewModel = "DeviceSelectFileViewModel";
        public const string DeviceAddedMsg = "AddDevice";
        #endregion

        #region 设备提取首页
        public const string DeviceHomeView = "ExportKey_DeviceHomeView";
        public const string DeviceHomeViewModel = "ExportKey_DeviceHomeViewModel";

        /// <summary>
        /// 设备主页
        /// </summary>
        public const string DeviceMainView = "ExportKey_DeviceMainView";
        /// <summary>
        /// 设备主页
        /// </summary>
        public const string DeviceMainViewModel = "ExportKey_DeviceMainViewModel";
        #endregion

        #region 选择控件（路径，文件，打开）

        public const string SelectControlView = "ExportKey_SelectControlView";
        public const string SelectControlViewModel = "ExportKey_SelectControlViewModel";

        #endregion

        #region 数据展示
        public const string DataDisplayView = "DataDisplayView";
        public const string DataDisplayViewModel = "DataDisplayViewModel";
        #endregion

        #region
        public const string MirrorView= "MirrorView";
        #endregion

        #endregion

        #region 插件
        /// <summary>
        /// 表示导出为插件(如数据解析插件）
        /// </summary>
        public const string PluginKey = "plugin";
        /// <summary>
        /// 表示导出为脚本插件(如python插件）
        /// </summary>
        public const string PluginScriptKey = "PluginScriptKey";
        /// <summary>
        /// 表示导出为插件加载器
        /// </summary>
        public const string PluginLoaderKey = "PluginLoader";
        /// <summary>
        /// 表示导出为插件适配器
        /// </summary>
        public const string PluginAdapterKey = "PluginAdapter";
        /// <summary>
        /// 表示导出为脚本执行上下文
        /// </summary>
        public const string ScriptContextKey = "ScriptContext";
        #endregion
    }
}
