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
        #region Fields

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly Dictionary<String, String> _caches;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpServiceBase 实例。
        /// </summary>
        /// <param name="pumpDescriptor">与此数据泵关联的元数据信息。</param>
        protected DataPumpBase(Pump pumpDescriptor)
        {
            PumpDescriptor = pumpDescriptor ?? throw new ArgumentNullException(nameof(pumpDescriptor));
            _caches = new Dictionary<String, String>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 与此数据泵关联的元数据信息。
        /// </summary>
        public Pump PumpDescriptor { get; }

        /// <summary>
        /// 设备 可能为NULL
        /// </summary>
        public Device Phone { get; set; }

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
        /// <param name="extractionItems">提取项列表。</param>
        /// <returns>执行上下文。</returns>
        public virtual DataPumpExecutionContext CreateContext(SourceFileItem source, params ExtractItem[] extractionItems)
        {
            return new DataPumpExecutionContext(PumpDescriptor, source, extractionItems) { Owner = this };
        }

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        public void Execute(DataPumpExecutionContext context)
        {
            if (!IsInit) throw new InvalidOperationException("Data pump need initialize");
            if (context.Owner != this) throw new InvalidOperationException("Unrecognize context");
            if (context.Reporter != null && context.Reporter.IsRuning) return;
            if (_caches.Keys.Contains(context.Source.Config))
            {
                //优先从缓存查找，避免同一路径多次提取
                context.Source.Local = _caches[context.Source.Config];
            }
            else
            {
                if (context.IsInit)
                {
                    ExecuteCore(context);
                }
                else if (InitExecutionContext(context))
                {
                    context.IsInit = true;
                    ExecuteCore(context);
                    //缓存路径信息
                    _caches.Add(context.Source.Config, context.Source.Local);
                }
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
