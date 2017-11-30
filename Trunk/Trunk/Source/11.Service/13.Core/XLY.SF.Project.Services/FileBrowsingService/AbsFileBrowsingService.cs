/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 17:58:17 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件浏览服务抽象类
    /// </summary>
    public abstract class AbsFileBrowsingService
    {

        #region 文件系统树浏览

        /// <summary>
        /// 获取根节点
        /// </summary>
        public async Task<FileBrowingNode> GetRootNode()
        {
            return await Task.Run(() => DoGetRootNode());
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        protected abstract FileBrowingNode DoGetRootNode();

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public async Task<List<FileBrowingNode>> GetChildNodes(FileBrowingNode parentNode)
        {
            return await Task.Run(() =>
            {
                if (null == parentNode.ChildNodes)
                {
                    var list = DoGetChildNodes(parentNode);

                    list.Sort((l, r) =>
                    {
                        if (!l.IsFile && r.IsFile)
                        {
                            return -1;
                        }
                        else if (l.IsFile && !r.IsFile)
                        {
                            return 1;
                        }

                        return string.Compare(l.Name, r.Name);
                    });

                    list.Sort((l, r) =>
                    {
                        if (!l.IsFile && r.IsFile)
                        {
                            return -1;
                        }
                        else if (l.IsFile && !r.IsFile)
                        {
                            return 1;
                        }

                        return string.Compare(l.Name, r.Name);
                    });

                    parentNode.ChildNodes = list;
                }

                return parentNode.ChildNodes;
            });
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        protected abstract List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode);

        #endregion

        #region 下载文件

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node">下载节点 可以是文件或者文件夹</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="persistRelativePath">是否保留相对路径</param>
        /// <param name="async">异步通知</param>
        public async Task<bool> Download(FileBrowingNode node, string savePath, bool persistRelativePath, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            return await Task.Run(() =>
            {
                DoDownload(node, savePath, persistRelativePath, cancellationTokenSource, async);
                return true;
            });
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node">下载节点 可以是文件或者文件夹</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="persistRelativePath">是否保留相对路径</param>
        /// <param name="async">异步通知</param>
        protected virtual void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            if (null == node)
            {
                return;
            }

            if (node.NodeType == FileBrowingNodeType.File)
            {
                try
                {
                    DownLoadFile(node, savePath, persistRelativePath, cancellationTokenSource, async);

                    async.OnExportFileNodeSuccessHandle(node, true);
                }
                catch
                {
                    async.OnExportFileNodeSuccessHandle(node, false);
                }
            }
            else
            {
                var cSavePath = string.Empty;
                if (persistRelativePath)
                {
                    cSavePath = savePath.Replace('/', '\\');
                }
                else
                {
                    cSavePath = Path.Combine(savePath, node.Name).Replace('/', '\\');
                }
                try
                {
                    FileHelper.CreateDirectory(cSavePath);

                    foreach (var cnode in DoGetChildNodes(node))
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        DoDownload(cnode, cSavePath, persistRelativePath, cancellationTokenSource, async);
                    }
                }
                catch
                {
                    async.OnExportFileNodeSuccessHandle(node, false);
                }
            }

            return;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileNode"></param>
        /// <param name="savePath"></param>
        /// <param name="persistRelativePath"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="async"></param>
        protected abstract void DownLoadFile(FileBrowingNode fileNode, string savePath, bool persistRelativePath,
            CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async);

        #endregion

        #region 条件搜索

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="cancellationToken">异步取消</param>
        /// <param name="async">异步通知</param>
        public async Task<bool> Search(FileBrowingNode node, IEnumerable<FilterArgs> args, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            return await Task.Run(() =>
            {
                if (null == node || node.IsFile)
                {
                    return false;
                }

                if (null == args || !args.Any())
                {
                    return false;
                }

                BeginSearch(node, args, cancellationTokenSource, async);

                return true;
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected virtual void BeginSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
            DoSearch(node, args, cancellationTokenSource, async);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected virtual void DoSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
            if (null == node.ChildNodes)
            {//查找子节点
                node.ChildNodes = DoGetChildNodes(node);
                if (null == node.ChildNodes)
                {
                    return;
                }
            }

            //先搜索文件，再搜索文件夹
            foreach (var file in node.ChildNodes.Where(f => f.IsFile))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                if (Filter(file, args))
                {//满足搜索要求
                    async?.OnSearchFileNodeHande(file);
                }
            }

            foreach (var dir in node.ChildNodes.Where(f => !f.IsFile))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                DoSearch(dir, args, cancellationTokenSource, async);
            }
        }

        /// <summary>
        /// 判断文件节点是否符合搜索要求
        /// </summary>
        /// <param name="filenode">文件节点 即IsFile为true</param>
        /// <param name="args">搜索条件</param>
        /// <returns>符合返回true  不符合返回false</returns>
        protected virtual bool Filter(FileBrowingNode filenode, IEnumerable<FilterArgs> args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case FilterByDateRangeArgs dateRangeArg://日期查询
                        if (!Filter(filenode, dateRangeArg))
                        {
                            return false;
                        }
                        break;
                    case FilterByStringContainsArgs keywordArg://字符串查询
                        if (!Filter(filenode, keywordArg))
                        {
                            return false;
                        }
                        break;
                    case FilterByRegexArgs regexArg://正则查询
                        if (!Filter(filenode, regexArg))
                        {
                            return false;
                        }
                        break;
                    case FilterByEnumStateArgs stateArg://文件状态查询
                        if (!Filter(filenode, stateArg))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        protected virtual bool Filter(FileBrowingNode filenode, FilterByDateRangeArgs arg)
        {
            if (null == filenode.CreateTime)
            {
                return false;
            }

            if (null != arg.StartTime && arg.StartTime > filenode.CreateTime)
            {
                return false;
            }

            if (null != arg.EndTime && arg.EndTime < filenode.CreateTime.Value.Date)
            {
                return false;
            }

            return true;
        }

        protected virtual bool Filter(FileBrowingNode filenode, FilterByStringContainsArgs arg)
        {
            return filenode.Name.Contains(arg.PatternText);
        }

        protected virtual bool Filter(FileBrowingNode filenode, FilterByRegexArgs arg)
        {
            return arg.Regex.IsMatch(filenode.Name);
        }

        protected virtual bool Filter(FileBrowingNode filenode, FilterByEnumStateArgs arg)
        {
            if (arg.State == Domains.EnumDataState.Normal)
            {//搜索正常的
                return !filenode.IsDelete;
            }
            else
            {//搜索删除的
                return filenode.IsDelete;
            }
        }

        #endregion

    }

    /// <summary>
    /// 文件浏览异步通知
    /// </summary>
    public class FileBrowingIAsyncTaskProgress
    {
        /// <summary>
        /// 查找到文件
        /// </summary>
        public event Action<FileBrowingNode> SearchFileNodeHandle;

        /// <summary>
        /// 文件导出通知
        /// </summary>
        public event Action<FileBrowingNode, bool> ExportFileNodeHandle;

        internal void OnSearchFileNodeHande(FileBrowingNode node)
        {
            try
            {
                SearchFileNodeHandle?.Invoke(node);
            }
            catch
            {

            }
        }

        internal void OnExportFileNodeSuccessHandle(FileBrowingNode node, bool isSuccess)
        {
            try
            {
                ExportFileNodeHandle?.Invoke(node, isSuccess);
            }
            catch
            {

            }
        }
    }
}
