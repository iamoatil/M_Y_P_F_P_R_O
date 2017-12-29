using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 表示为数据泵提供一种处理数据的策略。
    /// </summary>
    public interface IProcessStrategy
    {
        /// <summary>
        /// 执行数据泵。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        void Process(DataPumpExecutionContext context);
    }

    /// <summary>
    /// 表示为数据泵提供一种处理数据的策略。
    /// </summary>
    public interface IProcessControllableStrategy : IProcessStrategy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        void InitExecution(string sourcePath, string destPath);
    }
}
