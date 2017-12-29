using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.MefKeys
{
    public class GeneralKeys
    {
        #region 主界面通知

        /// <summary>
        /// 允许显示案例名称行
        /// </summary>
        public const string AllowShowCaseName = "GeneralKeys_AllowShowCaseName";
        /// <summary>
        /// 允许显示设备列表行
        /// </summary>
        public const string AllowShowDeviceList = "GeneralKeys_AllowShowDeviceList";
        /// <summary>
        /// 子界面导航
        /// </summary>
        public const string NavigationForSubView = "GeneralKeys_NavigationForSubView";
        /// <summary>
        /// 设置子界面状态
        /// </summary>
        public const string SetSubViewStatus = "GeneralKeys_SetSubViewStatus";

        /// <summary>
        /// 设置数据提取参数。
        /// </summary>
        public const string SetDataExtractionParamsMsg = "SetDataExtractionParamsMsg";
        /// <summary>
        /// 提取任务完成消息
        /// </summary>
        public const string ExtractTaskCompleteMsg = "GeneralKeys_TaskCompleteMsg";

        /// <summary>
        /// 提取设备状态改变消息。
        /// </summary>
        public const string ExtractDeviceStateMsg = "GeneralKeys_ExtractDeviceStateMsg";

        /// <summary>
        /// 删除缓存视图
        /// </summary>
        public const string DeleteCacheView = "GeneralKeys_DeleteCacheView";

        /// <summary>
        /// 设置变更消息。
        /// </summary>
        public const string SettingsChangedMsg = "SettingsChangedMsg";        

        #endregion
    }
}
