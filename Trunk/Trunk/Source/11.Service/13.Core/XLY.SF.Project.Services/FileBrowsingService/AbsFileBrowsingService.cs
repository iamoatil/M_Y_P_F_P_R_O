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
        public async Task<FileBrowingNode> GetRootNode(IAsyncTaskProgress async = null)
        {
            return await Task.Run(() => DoGetRootNode(async));
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        protected abstract FileBrowingNode DoGetRootNode(IAsyncTaskProgress async);

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public async Task<List<FileBrowingNode>> GetChildNodes(FileBrowingNode parentNode, IAsyncTaskProgress async = null)
        {
            return await Task.Run(() =>
            {
                if (null == parentNode.ChildNodes)
                {
                    var list = DoGetChildNodes(parentNode, async);

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
        protected abstract List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncTaskProgress async);

        #endregion

        #region 下载文件

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node">下载节点 可以是文件或者文件夹</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="persistRelativePath">是否保留相对路径</param>
        /// <param name="async">异步通知</param>
        public async void Download(FileBrowingNode node, string savePath, bool persistRelativePath = false, IAsyncTaskProgress async = null)
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
        protected abstract void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncTaskProgress async);

        #endregion

        #region 条件搜索

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        public async void Search(FileBrowingNode node, IEnumerable<FilterArgs> args, IAsyncTaskProgress async = null)
        {
            await Task.Run(() =>
            {
                if (null == node || node.IsFile)
                {
                    return;
                }

                if (null == args || !args.Any())
                {
                    return;
                }

                BeginSearch(node, args, async);
            });
        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected virtual void BeginSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, IAsyncTaskProgress async)
        {
            DoSearch(node, args, async);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected virtual void DoSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, IAsyncTaskProgress async)
        {
            if (null == node.ChildNodes)
            {//查找子节点
                node.ChildNodes = DoGetChildNodes(node, null);
                if (null == node.ChildNodes)
                {
                    return;
                }
            }

            //先搜索文件，再搜索文件夹
            foreach (var file in node.ChildNodes.Where(f => f.IsFile))
            {
                if (Filter(file, args))
                {//满足搜索要求
                    //TODO 异步通知查找到满足搜索要求的文件
                }
            }

            foreach (var dir in node.ChildNodes.Where(f => !f.IsFile))
            {
                DoSearch(dir, args, async);
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

            if (null != arg.EndTime && arg.EndTime < filenode.CreateTime)
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
}
