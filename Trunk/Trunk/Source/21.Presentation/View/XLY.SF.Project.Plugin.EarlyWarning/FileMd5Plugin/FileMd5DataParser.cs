using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.EarlyWarningView
{
    class FileMd5DataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        private FileDownloader _fileDownloader = new FileDownloader();

        public FileMd5DataParser()
        {
            var p = new Md5EarlyWarningPluginInfo()
            {
                Guid = "{D531E61F-544C-44EB-A499-8BBA86069F45}",
                Name = "Md5WarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,
            };
            p.SourceDirs.Add("/splash2");
            PluginInfo = p;

            _fileDownloader.Initialize(@"d:\Test\");//Path.GetTempPath();
        }
        
        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            FileMd5DataSource ds = null;
            try
            {
                var pi = PluginInfo as Md5EarlyWarningPluginInfo;

                ds = new FileMd5DataSource(pi.SaveDbPath);

                var items = new List<Call>();
                //把指定目录的文件下载到临时目录，并且计算其Md5写入数据库
                _fileDownloader.DownloadDirectory(pi.Phone, pi.SourceDirs);
                //1.从数据库获取
                var contactsPath = pi.SourcePath[0].Local;
                //if (FileHelper.IsValidDictory(contactsPath))
                //{
                //    var contacts2dbFile = Path.Combine(contactsPath, "contacts2.db");
                //    var call2dbFile = Path.Combine(contactsPath, "calls.db");

                //    if (FileHelper.IsValid(contacts2dbFile) || FileHelper.IsValid(call2dbFile))
                //    {
                //        var paser = new AndroidCallDataParseCoreV1_0(contacts2dbFile, call2dbFile);

                //        items.AddRange(paser.BuildData());
                //    }
                //}

                ////2.从APP植入获取
                //var calllog_info = pi.SourcePath[1].Local;
                //if (FileHelper.IsValid(calllog_info))
                //{
                //    BuildData(calllog_info, ref items);
                //}

                ds.Items.AddRange(items);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        /// <summary>
        /// 下载设备目录
        /// </summary>
        internal void DownloadDirectory()
        {
            var pi = PluginInfo as Md5EarlyWarningPluginInfo;
            //把指定目录的文件下载到临时目录，并且计算其Md5写入数据库
            _fileDownloader.DownloadDirectory(pi.Phone, pi.SourceDirs);
        }
    }

    /// <summary>
    /// 文件下载器
    /// </summary>
    internal class FileDownloader
    {
        /// <summary>
        ///  是否已经初始化。
        /// </summary>
        protected bool IsInitialize;

        /// <summary>
        /// BaseDir目录
        /// </summary>
        public string BaseDir { get; private set; }

        /// <summary>
        /// 文件Md5记录的list
        /// </summary>
        public List<FileMd5> Md5List { get; private set; }

        public bool Initialize(string dir)
        {
            IsInitialize = false;
            BaseDir = dir;
            //保证目标目录存在
            if (!Directory.Exists(BaseDir))
            {
                Directory.CreateDirectory(BaseDir);
            }
            //建立Md5List对象
            Md5List = new List<FileMd5>();

            IsInitialize = true;
            return IsInitialize;
        }

        /// <summary>
        /// 下载设备目录
        /// </summary>
        /// <param name="dev"></param>>
        internal async void DownloadDirectory(IDevice dev, List<string> sourceDirs)
        {
            //获取文件浏览对象和根节点
            AbsFileBrowsingService Service = FileBrowsingServiceFactory.GetFileBrowsingService(dev);
            FileBrowingNode rootNode = await Service.GetRootNode();

            foreach (var dir in sourceDirs)
            {
                //要检测的目录是否存在
                string[] pathNodes = dir.Split('/');
                FileBrowingNode curNode = rootNode;
                foreach (var pathNode in pathNodes)
                {
                    if (string.IsNullOrWhiteSpace(pathNode))
                    {
                        continue;
                    }

                    List<FileBrowingNode> nodes = await Service.GetChildNodes(curNode);

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
                    CancellationTokenSource cts = new CancellationTokenSource();
                    FileBrowingIAsyncTaskProgress fts = new FileBrowingIAsyncTaskProgress();
                    await Service.Download(curNode, BaseDir, true, cts, fts);
                    //下载后生成目录中所有文件的MD5
                    List<FileMd5> md5List = GenerateMd5(dir.Replace('/', '\\'), BaseDir);
                    Md5List.AddRange(md5List);
                }
            }
        }

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        private List<FileMd5> GenerateMd5(string dir, string baseDir)
        {
            List<FileMd5> md5List = new List<FileMd5>();
            string[] files = Directory.GetFiles(baseDir + dir, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string md5String = CryptographyHelper.MD5FromFileUpper(file);
                md5List.Add(new FileMd5(baseDir, file.Replace(baseDir, ""), md5String));
            }
            return md5List;
        }
    }
}
