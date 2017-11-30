using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 数据泵基类。
    /// </summary>
    public abstract class DataPumpBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpServiceBase 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        protected DataPumpBase(Pump metadata)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 与此数据泵关联的元数据信息。
        /// </summary>
        public Pump Metadata { get; }

        /// <summary>
        /// 是否已经初始化。
        /// </summary>
        public Boolean IsInit { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        public virtual void Initialize()
        {
            IsInit = InitializeCore();
        }

        /// <summary>
        /// 创建执行数据泵时所需的上下文对象。
        /// </summary>
        /// <param name="source">数据源。</param>
        /// <returns>执行上下文。</returns>
        public virtual DataPumpExecutionContext CreateContext(SourceFileItem source)
        {
            if (!IsInit) throw new InvalidOperationException("Data pump need initialize");
            return new DataPumpExecutionContext(Metadata, source) { Owner = this };
        }

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        public void Execute(DataPumpExecutionContext context)
        {
            if (!IsInit) throw new InvalidOperationException("Data pump need initialize");
            if (context.Owner != this) throw new InvalidOperationException("Unrecognize context");
            if (context.IsInit)
            {
                ExecuteCore(context);
            }
            else if (InitExecutionContext(context))
            {
                context.IsInit = true;
                ExecuteCore(context);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected abstract void ExecuteCore(DataPumpExecutionContext context);

        /// <summary>
        /// 初始化执行上下文。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected virtual Boolean InitExecutionContext(DataPumpExecutionContext context)
        {
            return true;
        }

        /// <summary>
        /// 初始化数据泵。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected virtual Boolean InitializeCore()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
