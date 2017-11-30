using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataPump.Android;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// 棒棒鸡Cellbrite的镜像数据泵。
    /// </summary>
    public class CellbriteMirrorDataPump : AndroidMirrorDataPump
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Misc.CellbriteMirrorDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public CellbriteMirrorDataPump(Pump metadata)
            :base(metadata)
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
            IFileSystemDevice device = new CellbriteDevice(Metadata.Source.ToString())
            {
                Source = Metadata.Source,
                ScanModel = (Byte)Metadata.ScanModel
            };
            return device;
        }

        #endregion

        #endregion
    }
}
