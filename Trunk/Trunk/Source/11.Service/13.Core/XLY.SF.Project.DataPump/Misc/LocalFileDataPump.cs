using System;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 本地文件数据泵。
    /// </summary>
    public class LocalFileDataPump : DataPumpBase
    {
        #region Fields

        private IProcessControllableStrategy _strategy;

        private String _destPath;

        private String _sourcePath;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.LocalFileDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public LocalFileDataPump(Pump metadata)
            : base(metadata)
        {

        }

        #endregion

        #region Methods

        #region Protected

        protected override void ExecuteCore(DataPumpExecutionContext context)
        {
            _strategy.Process(context);
        }

        /// <summary>
        /// 初始化执行上下文。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecutionContext(DataPumpExecutionContext context)
        {
            String sourcePath = _sourcePath;
            if (sourcePath == String.Empty || !Directory.Exists(sourcePath)) return false;
            context.SetContextData("sourcePath", sourcePath);
            return true;
        }

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            _sourcePath = PumpDescriptor.Source.ToSafeString();
            if (String.IsNullOrWhiteSpace(_sourcePath)) return false;
            if (!Directory.Exists(_sourcePath)) return false;

            String destPath = PumpDescriptor.SourceStorePath;
            if (String.IsNullOrWhiteSpace(destPath)) return false;

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            _destPath = destPath;

            InitStrategy();

            return true;
        }

        #endregion

        #region Private

        private void InitStrategy()
        {
            string backFilePath = string.Empty;

            if (FileHelper.IsItunsBackupPath(_sourcePath, ref backFilePath))
            {
                _strategy = new ItunsBackFileStategy();
            }
            else if (FileHelper.IsKuPaiBackupPath(_sourcePath, ref backFilePath))
            {
                _strategy = new KuPaiBackFileStategy();
            }
            else
            {
                _strategy = new AppDataStategy();
                backFilePath = _sourcePath;
            }

            _strategy?.InitExecution(backFilePath, _destPath);
        }

        #endregion

        #endregion
    }
}
