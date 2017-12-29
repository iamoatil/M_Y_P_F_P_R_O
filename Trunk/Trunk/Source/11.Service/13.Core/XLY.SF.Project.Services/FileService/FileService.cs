using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    public class FileService : IFileService
    {
        private FileServiceAbstractX fileServiceX;
        private IAsyncTaskProgress IAsync;
        private FNodeX _systemTree;

        public int ExternalCount { get; private set; }

        public FileService()
        {
            ExternalCount = 4;
        }

        /// <summary>
        /// 停止操作
        /// </summary>
        public void Stop()
        {
            fileServiceX?.Stop();
        }

        /// <summary>
        /// 释放所有资源关闭当前镜像文件的操作!
        /// </summary>
        public void Close()
        {
            fileServiceX?.Stop();
            fileServiceX?.Close();
        }

        /// <summary>
        /// 获取文件节点列表
        /// </summary>
        public IEnumerable<FNodeX> GetAllFile
        {
            get
            {
                return fileServiceX.AllFileNodeX;
            }
        }

        public FNodeX FileSystem
        {
            get { return _systemTree; }
        }

        /// <summary>
        /// 获取文件系统
        /// </summary>
        /// <param name="device"></param>
        /// <param name="iAsync"></param>
        /// <returns></returns>
        public FNodeX GetFileSystem(IFileSystemDevice device, TaskReporterBase iAsync)
        {
            IAsync = iAsync;
            CreateFileServiceAbstractX(device, iAsync);
            if (fileServiceX == null)
            {
                return null;
            }
            _systemTree = fileServiceX.GetFileSystem();
            return _systemTree;
        }

        private void CreateFileServiceAbstractX(IFileSystemDevice device, TaskReporterBase iAsync)
        {
            if (device is MirrorDevice)
            {
                fileServiceX = new MirrorDeviceService(device, iAsync);
            }
            else if (device is SDCardDevice)
            {
                fileServiceX = new SDCardDeviceService(device, iAsync);
            }
            else if (device is CellbriteDevice)
            {
                fileServiceX = new CellbriteDeviceService(device, iAsync);
            }
            else if (device is CottageDevice)
            {
                fileServiceX = new CottageMirrorDeviceService(device, iAsync);
            }
        }

        #region 获取用户数据分区文件列表

        private List<FNodeX> _userPartitionFiles;
        /// <summary>
        /// 获取用户数据分区文件列表
        /// </summary>
        public List<FNodeX> GetUserPartitionFiles
        {
            get
            {
                if (_userPartitionFiles == null || _userPartitionFiles.Count == 0)
                {
                    _userPartitionFiles = fileServiceX.AllFileNodeX;
                }
                return _userPartitionFiles;
            }
        }

        #endregion

        #region APP应用列表导出

        /// <summary>
        /// APP应用列表导出
        /// </summary>
        /// <param name="matchPath"></param>
        /// <param name="isCover"></param>
        public void ExportAppFile(string matchPath, string path, bool isCover = false)
        {
            var files = GetUserPartitionFiles;
            if (_systemTree == null)
            {
                return;
            }

            var source = files.FindAll(o => o.FullPath.StartsWith(matchPath, StringComparison.OrdinalIgnoreCase) && !o.IsDelete);
            foreach (var f in source)
            {
                fileServiceX.ExportFileX(f, path);
            }

            if (matchPath == @"\data\com.tencent.mm\MicroMsg\")
            {//查找微信分身
                var rootPaths = files.Where(n => s_MicroMsgRegex.IsMatch(n.FullPath) && !n.FullPath.StartsWith(matchPath, StringComparison.OrdinalIgnoreCase)).
                                      Select(f => f.FullPath.Substring(0, f.FullPath.IndexOf(@"\MicroMsg\"))).Distinct().ToList();

                foreach (var root in rootPaths)
                {
                    source = files.FindAll(o => o.FullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) && !o.IsDelete);
                    foreach (var f in source)
                    {
                        fileServiceX.ExportFileX(f, path);
                    }
                }
            }
        }

        private static readonly Regex s_MicroMsgRegex = new Regex(@"\\MicroMsg\\[0-9a-f]+\\EnMicroMsg.db");

        #endregion

        #region 获取文件分类

        private Dictionary<string, List<FNodeX>> _getKeyFNodeX;
        /// <summary>
        /// 获取文件分类
        /// </summary>
        public Dictionary<string, List<FNodeX>> GetKeyFNodeX
        {
            get
            {
                if (_getKeyFNodeX == null || _getKeyFNodeX.Count == 0)
                {
                    _getKeyFNodeX = fileServiceX.GetDictionarySuffix();
                }
                return _getKeyFNodeX;
            }
        }

        #endregion

        #region 媒体文件导出

        /// <summary>
        /// 媒体文件导出
        /// </summary>
        /// <param name="item"></param>
        /// <param name="separateChar"></param>
        public void ExportMediaFile(KeyValueItem item, string path, char separateChar)
        {
            // 媒体恢复必须在所有分区中去找寻对应的文件后缀
            var sourceFiles = GetKeyFNodeX;
            var key = item.Key;
            var suffix = item.Value.Split(separateChar);
            foreach (var suf in suffix)
            {
                if (sourceFiles.ContainsKey(suf))
                {
                    ExportFile(sourceFiles[suf], FileHelper.ConnectPath(path, key, suf));
                }
            }
        }

        #endregion

        #region 文件导出

        /// <summary>
        /// 文件导出
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        /// <returns></returns>
        public void ExportFile(FNodeX file, string path, bool isCover = false)
        {
            fileServiceX.ExportFileX(file, path, isCover: isCover);
        }

        /// <summary>
        /// 批量文件导出
        /// </summary>
        /// <param name="fNodeXs"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        public void ExportFile(List<FNodeX> fNodeXs, string path, bool isMedia = false, bool isCover = false)
        {
            foreach (var file in fNodeXs)
            {
                fileServiceX.ExportFileX(file, path, true, isCover, isMedia);
            }
        }

        #endregion

    }
}
