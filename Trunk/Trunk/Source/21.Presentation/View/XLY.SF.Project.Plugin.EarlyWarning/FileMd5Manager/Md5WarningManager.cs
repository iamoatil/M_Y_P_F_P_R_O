/* ==============================================================================
* Description：Md5Warning  
* Author     ：litao
* Create Date：2017/12/2 10:14:57
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class Md5WarningManager
    {
        public FileDownloader FileDownloader { get { return _fileDownloader; } }
        FileDownloader _fileDownloader = new FileDownloader();

        /// <summary>
        /// file的Md5进度报告器
        /// </summary>       
        public IProgress Progress
        {
            get { return FileMd5Reporter; }
        }
        private ProgressReporter FileMd5Reporter=new ProgressReporter();

        /// <summary>
        /// 提取目录
        /// </summary>
        List<ExtractDir> _extractDirs;

        /// <summary>
        /// 敏感数据列表
        /// </summary>
        List<DataNode> _sensitiveList;

        private bool _isStop;

        /// <summary>
        /// 下载的源目录
        /// </summary>
        private readonly List<string> _downloadSourceDirs = new List<string>();

        /// <summary>
        /// 下载的目标目录
        /// </summary>
        private string _downloadTargetDir;

        /// <summary>
        ///  是否已经初始化。
        /// </summary>
        protected bool IsInitialize;

        public Md5WarningManager()
        {
            _downloadTargetDir = Path.GetTempPath();
            _downloadSourceDirs.Add("/splash2");
        }

        /// <summary>
        ///初始化
        /// </summary>
        public bool Intialize(List<ExtractDir> extractDirs, List<DataNode> list)
        {
            IsInitialize = false;

            //获取其子目录
            _extractDirs = extractDirs;

            _sensitiveList = list;

            _fileDownloader.AfterDownloaded -= FileDownloader_AfterDownloaded;
            _fileDownloader.AfterDownloaded += FileDownloader_AfterDownloaded;
            _fileDownloader.Initialize(_downloadTargetDir, _downloadSourceDirs);

            IsInitialize = true;
            return IsInitialize;
        }        

        /// <summary>
        /// 检测文件的Md5。因为下载是异步的，所以此方法很快就执行完毕了
        /// </summary>
        public void Detect()
        {
            if(!IsInitialize)
            {
                FileMd5Reporter.Report(new ProgressStater(ProgressState.IsFinished), 0);
                return ;
            }
            _isStop = false;           
            _fileDownloader.DownloadDirectory();            
        }

        private void FileDownloader_AfterDownloaded(List<string> dirs, string localDir)
        {
            if (!IsInitialize)
            {
                return;
            }
            if (_isStop == true)
            {
                FileMd5Reporter.Report(new ProgressStater(ProgressState.IsFinished), 0);
                return;
            }
            FileMd5Reporter.Report(new ProgressStater(ProgressState.IsProgressing), 0.5);

            int count = dirs.Count;
            foreach (var dir in dirs)
            {
                //把所有的下载文件都生成Md5并且保存到FileMd5对象中
                FileMd5Generator fileMd5Generator = new FileMd5Generator();
                fileMd5Generator.GenerateMd5(dir, localDir);
                List<FileMd5DataItem> fileMd5List = fileMd5Generator.Md5List;
                //把md5敏感的数据的SensitiveId设置成配置中的值
                foreach (var item in fileMd5List)
                {
                    string md5Str = item.FileMd5;
                    foreach (DataNode dataNode in _sensitiveList)
                    {
                        if (dataNode.SensitiveData.Value == md5Str)
                        {
                            item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                        }
                    }
                }
                
                foreach (var extractDir in _extractDirs)
                {
                    //把所有文件的Md5信息写入到数据库和ds文件中
                    FileMd5DbWriter writer = new FileMd5DbWriter();
                    writer.Initialize(extractDir.Dir);
                    writer.GenerateDsFile();
                    writer.WriteDb(fileMd5List);
                }
                FileMd5Reporter.Report(new ProgressStater(ProgressState.IsProgressing), 0.5/ count);                
            }
            FileMd5Reporter.Report(new ProgressStater(ProgressState.IsFinished), 0);
        }

        internal void StopDetect()
        {
            _isStop = true;
            _fileDownloader.Stop();
        }
    }
}
