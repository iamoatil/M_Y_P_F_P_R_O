using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.BlackBerry
{
    /// <summary>
    /// 黑莓USB数据泵。
    /// </summary>
    public class BlackBerryUsbDataPump : ControllableDataPumpBase
    {
        #region Fields

        private String _destDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.BlackBerry.BlackBerryUsbDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public BlackBerryUsbDataPump(Pump metadata)
            : base(metadata)
        {
        }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            String destDirectory = context.Source.Local;
            //BlackBerryDeviceManager deviceManager = BlackBerryDeviceManager.Instance;
            //deviceManager.DataPumpType = true;
            //deviceManager.CopyFile((Device)context.Metadata.Source, "", destDirectory, context.Reporter);
        }

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            if (!(Metadata.Source is Device)) return false;
            String destDirectory = FileHelper.ConnectPath(Metadata.SourceStorePath, $"BlackBerry_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");
            if (Directory.Exists(destDirectory))
            {
                Directory.Delete(destDirectory, true);
            }
            Directory.CreateDirectory(destDirectory);
            _destDirectory = destDirectory;
            return true;
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecutionContext(DataPumpControllableExecutionContext context)
        {
            if (context.Source == null) return false;
            context.Source.Local = _destDirectory;
            return true;
        }

        #endregion

        #endregion
    }
}
