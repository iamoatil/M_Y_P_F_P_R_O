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
        void Process(DataPumpExecutionContext context);
    }

    /// <summary>
    /// 表示为数据泵提供一种处理数据的策略。
    /// </summary>
    public interface IProcessControllableStrategy
    {
        void Process(DataPumpControllableExecutionContext context);
    }
}
