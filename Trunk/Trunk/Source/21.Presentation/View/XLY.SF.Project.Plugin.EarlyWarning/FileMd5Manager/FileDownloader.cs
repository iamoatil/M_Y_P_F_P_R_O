using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 文件下载器
    /// </summary>
    public class FileDownloader
    {
        bool _isStop;

        /// <summary>
        ///  是否已经初始化。
        /// </summary>
        protected bool IsInitialize;

        /// <summary>
        /// 本地目录
        /// </summary>
        public string LocalDir { get; private set; }

        /// <summary>
        /// 源目录列表
        /// </summary>
        public List<string> SourceDirs { get; private set; }

        /// <summary>
        /// 从设备中下载指定目录（SourceDirs）的文件到本地目录（BaseDir）
        /// </summary>
        public IDevice Device { get; private set; }

        /// <summary>
        /// 下载完成后事件
        /// </summary>
        public event Action<List<string>, string> AfterDownloaded;

        public bool Initialize(IDevice dev, string localDir, List<string> sourceDirs)
        {
            IsInitialize = false;
            
            LocalDir = localDir.TrimEnd('\\')+"\\";
            //保证目标目录存在
            if (!Directory.Exists(LocalDir))
            {
                Directory.CreateDirectory(LocalDir);
            }
            if(dev == null)
            {
                return false;
            }
            Device = dev;
            SourceDirs = sourceDirs;

            IsInitialize = true;
            return IsInitialize;
        }

        public bool Initialize(string localDir, List<string> sourceDirs)
        {
            return Initialize(Device, localDir, sourceDirs);
        }

        /// <summary>
        /// 设置设备
        /// </summary>
        /// <param name="dev"></param>
        public void SetDevice(IDevice dev)
        {
            Device = dev;
        }

        /// <summary>
        /// 下载设备目录
        /// </summary>
        /// <param name="dev"></param>>
        internal async void DownloadDirectory()
        {
            //获取文件浏览对象和根节点
            AbsFileBrowsingService service = FileBrowsingServiceFactory.GetFileBrowsingService(Device);
            if (service == null)
            {
                return;
            }
            FileBrowingNode rootNode = await service.GetRootNode();

            //下载存在的目录，并且保存这些目录
            List<string> existDirs = new List<string>();
            foreach (var dir in SourceDirs)
            {
                if (_isStop)
                {
                    break;
                }

                //要检测的目录是否存在
                string[] pathNodes = dir.Split('/');
                FileBrowingNode curNode = rootNode;
                foreach (var pathNode in pathNodes)
                {
                    if (string.IsNullOrWhiteSpace(pathNode))
                    {
                        continue;
                    }

                    List<FileBrowingNode> nodes = await service.GetChildNodes(curNode);

                    curNode = null;
                    foreach (var node in nodes)
                    {
                        if (node.Name == pathNode)
                        {
                            curNode = node;
                            break;
                        }
                    }
                    if (curNode == null)
                    {
                        break;
                    }
                }

                //存在才下载
                if (curNode != null)
                {
                    existDirs.Add(dir.Replace('/', '\\'));
                    CancellationTokenSource cts = new CancellationTokenSource();
                    FileBrowingIAsyncTaskProgress fts = new FileBrowingIAsyncTaskProgress();
                    await service.Download(curNode, LocalDir, true, cts, fts);
                }
            }
            //下载完成后，执行完成后事件
            OnAfterDownloaded(existDirs, LocalDir);
        }

        protected void OnAfterDownloaded(List<string> dirs,string localDir)
        {
            if(AfterDownloaded != null)
            {
                AfterDownloaded(dirs, localDir);
            }
        }

        public void Stop()
        {
            _isStop = true;
        }
    }
}
