/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 17:59:32 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 安卓手机文件浏览服务
    /// </summary>
    internal class AndroidDeviceFileBrowsingService : AbsFileBrowsingService
    {
        /// <summary>
        /// 安卓手机
        /// </summary>
        private Device AndroidPhone { get; set; }

        /// <summary>
        /// 根节点
        /// </summary>
        private FileBrowingNode RootNode { get; set; }

        public AndroidDeviceFileBrowsingService(Device device)
        {
            AndroidPhone = device;

            RootNode = new AndroidDeviceFileBrowingNode()
            {
                Name = "根目录",
                NodeType = FileBrowingNodeType.Root
            };
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        protected override FileBrowingNode DoGetRootNode(IAsyncTaskProgress async)
        {
            return RootNode;
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncTaskProgress async)
        {
            return DoGetChildNodes(parentNode as AndroidDeviceFileBrowingNode);
        }

        private List<FileBrowingNode> DoGetChildNodes(AndroidDeviceFileBrowingNode parentNode)
        {
            if (null == parentNode)
            {
                return null;
            }

            if (parentNode.NodeType == FileBrowingNodeType.Root)
            {
                return ConvertFileNode(AndroidHelper.Instance.FindRootFiles(AndroidPhone), parentNode);
            }
            else if (parentNode.NodeType == FileBrowingNodeType.Directory)
            {
                return ConvertFileNode(AndroidHelper.Instance.FindFiles(AndroidPhone, parentNode.SourcePath), parentNode);
            }

            return null;
        }

        private List<FileBrowingNode> ConvertFileNode(List<LSFile> listFiles, FileBrowingNode parentNode)
        {
            if (listFiles.IsInvalid())
            {
                return new List<FileBrowingNode>();
            }

            List<FileBrowingNode> res = new List<FileBrowingNode>();

            foreach (var file in listFiles.Where(f => f.Type == "Directory" || f.Type == "File"))
            {
                res.Add(new AndroidDeviceFileBrowingNode()
                {
                    Name = file.Name,
                    Parent = parentNode,
                    FileSize = (UInt64)file.Size,
                    NodeType = GetNodeType(file.Type),
                    CreateTime = file.CreateDate,
                    LastAccessTime = file.LastAccessDate,
                    LastWriteTime = file.LastWriteData,
                    SourcePath = file.FullPath,
                });
            }

            return res;
        }

        private FileBrowingNodeType GetNodeType(string typeStr)
        {
            switch (typeStr)
            {
                case "Directory":
                    return FileBrowingNodeType.Directory;
                case "File":
                    return FileBrowingNodeType.File;
            }

            return FileBrowingNodeType.Directory;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="node"></param>
        /// <param name="savePath"></param>
        /// <param name="persistRelativePath"></param>
        /// <param name="async"></param>
        protected override void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncTaskProgress async)
        {
            if (node.NodeType == FileBrowingNodeType.File)
            {
                AndroidHelper.Instance.CopyFile(AndroidPhone, (node as AndroidDeviceFileBrowingNode).SourcePath, savePath, async, persistRelativePath);
            }
            else
            {
                AndroidHelper.Instance.CopyFile(AndroidPhone, string.Format("{0}#F", (node as AndroidDeviceFileBrowingNode).SourcePath), savePath, async, persistRelativePath);
            }
        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected override void BeginSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, IAsyncTaskProgress async)
        {
            var stateArg = args.FirstOrDefault(a => a is FilterByEnumStateArgs);
            if (null != stateArg && (stateArg as FilterByEnumStateArgs).State != EnumDataState.Normal)
            {//如果要搜索删除状态的文件，直接返回。因为安卓手机文件浏览不会有删除状态的文件。
                //TODO:通知搜索结束
                return;
            }

            base.BeginSearch(node, args, async);
        }

        /// <summary>
        /// 安卓手机文件节点
        /// </summary>
        private class AndroidDeviceFileBrowingNode : FileBrowingNode
        {
            /// <summary>
            /// 源路径
            /// </summary>
            public string SourcePath { get; internal set; }

            public override string ToString()
            {
                return SourcePath;
            }
        }
    }

}
