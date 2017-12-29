using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 表示执行数据泵的执行上下文。
    /// </summary>
    public class DataPumpExecutionContext
    {
        #region Fields

        private readonly Hashtable _datas = new Hashtable();

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpTaskContext 实例。
        /// </summary>
        /// <param name="pumpDescriptor">与此数据泵关联的元数据信息。</param>
        /// <param name="source">数据源。如果不需要数据源则设置为null。</param>
        /// <param name="extractionItems">提取项列表。</param>
        internal DataPumpExecutionContext(Pump pumpDescriptor, SourceFileItem source,ExtractItem[] extractionItems)
        {
            PumpDescriptor = pumpDescriptor ?? throw new ArgumentNullException("metadata");
            ExtractionItems = extractionItems;
            Source = source;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 数据保存目录。
        /// </summary>
        public String TargetDirectory => PumpDescriptor?.SavePath;

        /// <summary>
        /// 对任务进行描述的元数据。
        /// </summary>
        public Pump PumpDescriptor { get; }

        /// <summary>
        /// 提取项列表。
        /// </summary>
        public IEnumerable<ExtractItem> ExtractionItems { get; }

        /// <summary>
        /// 数据源。
        /// </summary>
        public SourceFileItem Source { get; }

        #region Reporter

        private ITaskProgressReporter _reporter;
        /// <summary>
        /// 用于异步通知的报告器。
        /// </summary>
        public ITaskProgressReporter Reporter
        {
            get => _reporter;
            set
            {
                if (_reporter != value)
                {
                    if (_reporter == null || _reporter.State == Framework.Core.Base.ViewModel.TaskState.Idle)
                    {
                        _reporter = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Reporter is in use");
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 是否已请求取消操作。
        /// </summary>
        public Boolean IsCancellationRequested => CancellationToken.IsCancellationRequested;

        /// <summary>
        /// 取消标记。默认为CancellationToken.None，不支持取消。
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        /// 拥有该任务的服务。
        /// </summary>
        internal DataPumpBase Owner { get; set; }

        /// <summary>
        /// 自定义数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>数据值。</returns>
        internal Object this[String name]
        {
            get => _datas[name];
            set => _datas[name] = value;
        }

        /// <summary>
        /// 是否已经被初始过。
        /// </summary>
        internal Boolean IsInit { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取对象的Hash值。
        /// </summary>
        /// <returns>对象的Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return PumpDescriptor.GetHashCode();
        }

        #endregion

        #endregion
    }
}
