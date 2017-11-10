using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// 本地文件数据泵。
    /// </summary>
    public class LocalFileDataPump : ControllableDataPumpBase
    {
        #region Fields

        private IProcessControllableStrategy _strategy;

        #endregion

        #region Methods

        #region Protected

        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            InitStrategy(context);
            _strategy.Process(context);
        }

        protected override Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            String destPath = context.TargetDirectory;
            if (String.IsNullOrWhiteSpace(destPath)) return false;

            String sourcePath = context.Source.ToSafeString();
            if (sourcePath == String.Empty || !Directory.Exists(sourcePath)) return false;

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            context.SetContextData("destPath", destPath);
            context.SetContextData("sourcePath", sourcePath);
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
