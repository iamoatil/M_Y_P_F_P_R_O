﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.DataPump.Android
{
    /// <summary>
    /// Android镜像数据泵。
    /// </summary>
    public class AndroidMirrorDataPump : ControllableDataPumpBase
    {
        #region Fields

        private IFileSystemDevice _fsd;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Android.AndroidMirrorDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public AndroidMirrorDataPump(Pump metadata)
            : base(metadata)
        {
            FileService = new FileService();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 文件服务。
        /// </summary>
        protected FileService FileService { get; }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected sealed override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            switch (context.Source.ItemType)
            {
                case SourceFileItemType.FileExtension:
                    HandleWithFileExtension(context);
                    break;
                case SourceFileItemType.NormalPath:
                    HandleWithNormalPath(context);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            IFileSystemDevice device = CreateFileSystemDevice();
            if (device == null) return false;
            _fsd = device;
            return FileService.GetFileSystem(device, null) != null;
        }

        /// <summary>
        /// 创建实现了 IFileSystemDevice 接口的类型实例。
        /// </summary>
        /// <returns>实现了 IFileSystemDevice 接口的类型实例。</returns>
        protected virtual IFileSystemDevice CreateFileSystemDevice()
        {
            IFileSystemDevice device = new MirrorDevice
            {
                Source = Metadata,
                ScanModel = (Byte)Metadata.ScanModel
            };
            return device;
        }

        #endregion

        #region Private

        private void HandleWithFileExtension(DataPumpExecutionContext context)
        {
            SourceFileItem source = context.Source;
            String[] configs = source.Config.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (configs.Length == 0) return;
            KeyValueItem kv = new KeyValueItem
            {
                Key = configs[0].TrimStart("$"),
                Value = configs[1]
            };
            source.Local = FileHelper.ConnectPath(context.TargetDirectory, kv.Key);
            FileService.ExportMediaFile(kv, context.TargetDirectory, ';');
        }

        private void HandleWithNormalPath(DataPumpExecutionContext context)
        {
            SourceFileItem source = context.Source;
            String path = source.Config.TrimEnd("#F").Replace("/", @"\");
            if (path.StartsWith(@"\data"))
            {
                path = path.TrimStart(@"\data");
            }
            else if (path.StartsWith(@"\system"))
            {
                path = path.TrimStart(@"\system");
            }
            source.Local = FileHelper.ConnectPath(context.TargetDirectory, path);
            FileService.ExportAppFile(path, context.TargetDirectory);
        }

        #endregion

        #endregion
    }
}
