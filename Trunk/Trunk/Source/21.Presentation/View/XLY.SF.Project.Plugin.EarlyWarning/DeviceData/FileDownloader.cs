/* ==============================================================================
* Description：Md5Warning  
* Author     ：litao
* Create Date：2017/12/8 16:43:04
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// File下载器
    /// </summary>
    class FileDownloader
    {
        /// <summary>
        /// 文件浏览服务
        /// </summary>
        AbsFileBrowsingService Service;

        private bool _isIntialized;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        bool Initialize()
        {
            _isIntialized = false;
            //Service = FileBrowsingServiceFactory.GetFileBrowsingService(dev);
            _isIntialized = true;

            return _isIntialized;
        }

        /// <summary>
        /// 下载文件或文件夹
        /// </summary>
        private async void Download(string path)
        {
            if(!_isIntialized)
            {
                return;
            }

            FileBrowingNode curNode= GetFileBrowingNode(path);

            if (curNode != null)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                FileBrowingIAsyncTaskProgress fts = new FileBrowingIAsyncTaskProgress();
                await Service.Download(curNode, Path.GetTempPath(), true, cts, fts);
            }
        }

        private FileBrowingNode GetFileBrowingNode(string path)
        {
            return null;
            //return LoadingData(async () => 
            //{
            //    FileBrowingNode rootNode = await Service.GetRootNode();

            //    List<FileBrowingNode> nodes = await Service.GetChildNodes(rootNode);
            //    foreach (var node in nodes)
            //    {
            //        if (node.Name == "splash2")
            //        {
            //            //curNode = node;
            //        }
            //    }
            //});
        }

        /// <summary>
        /// 加载节点数据
        /// </summary>
        /// <param name="doSomething"></param>
        public async void LoadingData(Action doSomething)
        {
            await Task.Run(doSomething);
        }
    }
}
