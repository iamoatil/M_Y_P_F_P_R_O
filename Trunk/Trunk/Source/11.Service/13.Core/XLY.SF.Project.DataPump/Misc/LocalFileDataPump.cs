using System;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// 本地文件数据泵。
    /// </summary>
    public class LocalFileDataPump : ControllableDataPumpBase
    {
        #region Fields

        private IProcessControllableStrategy _strategy;

        private String _destPath;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Misc.LocalFileDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public LocalFileDataPump(Pump metadata)
            : base(metadata)
        {

        }

        #endregion

        #region Methods

        #region Protected

        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            InitStrategy(context);
            _strategy.Process(context);
        }

        /// <summary>
        /// 初始化执行上下文。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecutionContext(DataPumpControllableExecutionContext context)
        {
            String sourcePath = context.Source.ToSafeString();
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
            String destPath = Metadata.SourceStorePath;
            if (String.IsNullOrWhiteSpace(destPath)) return false;

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            _destPath = destPath;
            return true;
        }

        #endregion

        #region Private

        private void InitStrategy(DataPumpControllableExecutionContext context)
        {
            if (ItunsBackFileStategy.IsItunsBackFile(context, out String backFilePath))
            {
                if (!(_strategy is ItunsBackFileStategy))
                {
                    _strategy = new ItunsBackFileStategy();
                }
            }
            else if (KuPaiBackFileStategy.IsKuPaiBackFile(context, out backFilePath))
            {
                if (!(_strategy is KuPaiBackFileStategy))
                {
                    _strategy = new KuPaiBackFileStategy();
                }
            }
            else
            {
                if (!(_strategy is AppDataStategy))
                {
                    _strategy = new AppDataStategy();
                }
            }

            _strategy?.InitExecution(context);
        }

        #endregion

        #endregion
    }
}
