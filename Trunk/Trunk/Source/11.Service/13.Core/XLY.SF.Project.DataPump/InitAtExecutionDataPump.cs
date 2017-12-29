using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 在执行时只执行一次初始化操作的数据泵。
    /// </summary>
    public abstract class InitAtExecutionDataPump : DataPumpBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.InitAtExecutionDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        protected InitAtExecutionDataPump(Pump metadata)
            : base(metadata)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否已经在第一次执行时初始化过。
        /// </summary>
        public Boolean IsInitedAtFirstTime { get; private set; }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务，并在第一次执行时初始化相关数据。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected sealed override void ExecuteCore(DataPumpExecutionContext context)
        {
            if (!IsInitedAtFirstTime)
            {
                IsInitedAtFirstTime = InitAtFirstTime(context);
            }
            if (IsInitedAtFirstTime)
            {
                OverrideExecute(context);
            }
        }

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected abstract void OverrideExecute(DataPumpExecutionContext context);

        /// <summary>
        /// 第一次执行时的初始化操作。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected abstract Boolean InitAtFirstTime(DataPumpExecutionContext context);

        #endregion

        #endregion
    }
}
