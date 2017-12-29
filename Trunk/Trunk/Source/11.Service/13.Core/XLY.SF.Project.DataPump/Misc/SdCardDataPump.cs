using System;
using XLY.SF.Project.DataPump;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// SD卡数据泵。
    /// </summary>
    public class SdCardDataPump : AndroidMirrorDataPump
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.SdCardDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public SdCardDataPump(Pump metadata)
            : base(metadata)
        {
        }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 创建实现了 IFileSystemDevice 接口的类型实例。
        /// </summary>
        /// <returns>实现了 IFileSystemDevice 接口的类型实例。</returns>
        protected override IFileSystemDevice CreateFileSystemDevice()
        {
            IFileSystemDevice device = new SDCardDevice
            {
                Source = PumpDescriptor,
                ScanModel = (Byte)PumpDescriptor.ScanModel
            };
            return device;
        }

        #endregion

        #endregion
    }
}
