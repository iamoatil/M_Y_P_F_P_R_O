using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.MessageBase.Navigation
{
    /// <summary>
    /// 导航日志记录【Key】
    /// </summary>
    public static class NavigationLogHelper
    {
        #region 导航历史

        /// <summary>
        /// 导航界面历史记录
        /// </summary>
        private static List<NavigationLogStatus> _historyExportKeys = new List<NavigationLogStatus>();

        /// <summary>
        /// 添加导航记录
        /// </summary>
        /// <param name="navigation">导航消息</param>
        public static void AddNavigationLog(NavigationArgs navigation, bool showInNewWindow)
        {
            NavigationLogStatus logEmt = new NavigationLogStatus(showInNewWindow, navigation.MsgToken);
            _historyExportKeys.Add(logEmt);
        }

        #region 所有导航筛选

        /// <summary>
        /// 获取上一个界面Key
        /// </summary>
        /// <returns></returns>
        public static string GetBeforeViewKey()
        {
            return _historyExportKeys.LastOrDefault()?.ExportKey;
        }

        /// <summary>
        /// 跳过指定Key获取最后一个界面Key
        /// </summary>
        /// <returns></returns>
        public static string GetBeforeViewKeyBySkipKey(string skipViewKey)
        {
            return _historyExportKeys.LastOrDefault((t) => t.ExportKey != skipViewKey)?.ExportKey;
        }

        #endregion

        #region 主界面导航筛选

        /// <summary>
        /// 跳过指定Key获取最后一个界面Key，在主界面导航中查找
        /// </summary>
        /// <returns></returns>
        public static string GetBeforeViewKeyBySkipKeyAtMainView(string skipViewKey)
        {
            return _historyExportKeys.LastOrDefault((t) => t.ExportKey != skipViewKey && !t.ShowInNewWindow)?.ExportKey;
        }

        /// <summary>
        /// 获取上一个界面Key，在主界面导航中查找
        /// </summary>
        /// <returns></returns>
        public static string GetBeforeViewKeyAtMainView()
        {
            return _historyExportKeys.LastOrDefault((t) => !t.ShowInNewWindow)?.ExportKey;
        }

        #endregion

        #endregion
    }
}
