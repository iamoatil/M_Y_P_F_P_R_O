using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.Model.PresentationNavigationElement;

/*
 * 子界面缓存管理器
 * 由于设备主页中所有界面都是公有的，可重复的
 * 所以在创建这些界面时，需要根据唯一值【设备ID和ExportKey】来做区分
 * 此处用于统一管理和创建这些View
 * 
 * 创建人：Bob
 * 
 */

namespace XLY.SF.Project.ViewModels.Main.DeviceMain.Navigation
{
    /// <summary>
    /// 子界面缓存管理器
    /// </summary>
    public class SubViewCacheManager
    {
        #region Properties

        /// <summary>
        /// 设备ID
        /// </summary>
        private readonly string _devID;

        #endregion

        public SubViewCacheManager(string devID)
        {
            _devID = devID;
        }

        #region Methods

        /// <summary>
        /// 获取或创建视图
        /// </summary>
        /// <param name="exportKey">导出Key</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        public UcViewBase GetOrCreateView(string exportKey, object @params)
        {
            PreCacheToken delToken = new PreCacheToken(_devID, exportKey);
            UcViewBase targetView;
            if (!SystemContext.Instance.CurCacheViews.TryGetFirstView(delToken, out targetView))
            {
                targetView = IocManagerSingle.Instance.GetViewPart(exportKey);
                targetView.DataSource?.LoadViewModel(@params);
                SystemContext.Instance.CurCacheViews.AddViewCache(delToken, targetView);
            }
            return targetView;
        }

        /// <summary>
        /// 释放缓存的View
        /// </summary>
        /// <param name="exportKey"></param>
        public void ReleaseView(string exportKey)
        {
            PreCacheToken delToken = new PreCacheToken(_devID, exportKey);
            SystemContext.Instance.CurCacheViews.RemoveViewCache(delToken);
        }

        #endregion
    }
}
