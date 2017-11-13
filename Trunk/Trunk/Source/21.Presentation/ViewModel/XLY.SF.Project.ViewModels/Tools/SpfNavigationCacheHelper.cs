using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/5/10 15:53:45
 * 类功能说明：
 * 1. 所有导航界面缓存记录
 * 2. 包含主界面导航，弹窗
 * 3. 不包含选择路径，打开文件，保存文件等界面
 * 
 *************************************************/

namespace XLY.SF.Project.ViewModels.Tools
{
    /// <summary>
    /// SPF导航记录器
    /// </summary>
    public static class SpfNavigationCacheHelper
    {
        static SpfNavigationCacheHelper()
        {
            //_historyExportKeys = new List<string>();
            _curCacheViews = new Dictionary<Guid, UcViewBase>();
        }

        //#region 主界面导航记录

        ///// <summary>
        ///// 导航界面历史记录【主界面导航】
        ///// </summary>
        //private static List<string> _historyExportKeys;

        ///// <summary>
        ///// 添加新导航窗体Key【主界面】
        ///// </summary>
        ///// <param name="exportKey"></param>
        //public static void AddViewKeysByMainView(string exportKey)
        //{
        //    _historyExportKeys.Add(exportKey);
        //}

        ///// <summary>
        ///// 获取上一个界面Key【主界面】
        ///// </summary>
        ///// <returns></returns>
        //public static string GetBeforeViewKeyByMainView()
        //{
        //    return _historyExportKeys.LastOrDefault();
        //}

        ///// <summary>
        ///// 跳过指定Key获取最后一个界面Key【主界面】
        ///// </summary>
        ///// <returns></returns>
        //public static string GetBeforeViewKeyBySkipKey(string skipViewKey)
        //{
        //    return _historyExportKeys.LastOrDefault((t) => t != skipViewKey);
        //}

        //#endregion

        #region 设备界面缓存

        #region 当前缓存Views

        /// <summary>
        /// 当前缓存的View
        /// </summary>
        private static Dictionary<Guid, UcViewBase> _curCacheViews;

        #endregion

        #endregion
    }
}
