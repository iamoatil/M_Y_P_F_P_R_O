/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 17:58:17 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件浏览服务抽象类
    /// </summary>
    public abstract class AbsFileBrowsingService
    {
        /// <summary>
        /// 获取根节点
        /// </summary>
        public async Task<FileBrowingNode> GetRootNode(IAsyncProgress async = null)
        {
            return await Task.Run(() => DoGetRootNode(async));
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        protected abstract FileBrowingNode DoGetRootNode(IAsyncProgress async);

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public async Task<List<FileBrowingNode>> GetChildNodes(FileBrowingNode parentNode, IAsyncProgress async = null)
        {
            return await Task.Run(() => DoGetChildNodes(parentNode, async));
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        protected abstract List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncProgress async);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node">下载节点 可以是文件或者文件夹</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="persistRelativePath">是否保留相对路径</param>
        /// <param name="async">异步通知</param>
        public async void Download(FileBrowingNode node, string savePath, bool persistRelativePath = false, IAsyncProgress async = null)
        {
            await Task.Run(() => DoDownload(node, savePath, persistRelativePath, async));
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node">下载节点 可以是文件或者文件夹</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="persistRelativePath">是否保留相对路径</param>
        /// <param name="async">异步通知</param>
        protected abstract void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async);
    }

}
