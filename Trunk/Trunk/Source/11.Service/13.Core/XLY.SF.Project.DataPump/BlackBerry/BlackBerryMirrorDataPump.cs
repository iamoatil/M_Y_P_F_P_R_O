using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.BlackBerry
{
    /// <summary>
    /// 黑莓镜像数据泵。
    /// </summary>
    public class BlackBerryMirrorDataPump : DataPumpBase
    {
        #region Fields

        private String _destDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.BlackBerry.BlackBerryMirrorDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public BlackBerryMirrorDataPump(Pump metadata)
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
        protected override void ExecuteCore(DataPumpExecutionContext context)
        {
            String dataSourcePath = context.PumpDescriptor.Source as String;
            String destDirectory = context.Source.Local;
            ZipFile.ExtractToDirectory(dataSourcePath, destDirectory);
        }

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            String dataSourcePath = PumpDescriptor.Source as String;
            if (String.IsNullOrWhiteSpace(dataSourcePath)) return false;
            String destDirectory = FileHelper.ConnectPath(PumpDescriptor.SourceStorePath, $"BlackBerry_{dataSourcePath.GetHashCode()}");
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
        protected override Boolean InitExecutionContext(DataPumpExecutionContext context)
        {
            context.Source.Local = _destDirectory;
            return true;
        }

        #endregion

        #endregion
    }
}
