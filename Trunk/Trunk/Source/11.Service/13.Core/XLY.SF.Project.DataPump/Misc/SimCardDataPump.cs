using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// SIM卡数据泵（这是一个打酱油的数据泵）。
    /// </summary>
    public class SimCardDataPump : DataPumpBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.SimCardDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public SimCardDataPump(Pump metadata)
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
        }

        #endregion

        #endregion
    }
}
