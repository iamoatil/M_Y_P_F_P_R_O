using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Plugin;

/* ==============================================================================
* Description：AbstractEarlyWarningPlugin  
* Author     ：litao
* Create Date：2017/12/2 10:13:45
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    [Plugin]
    public abstract class AbstractEarlyWarningPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        public virtual void Dispose()
        {

        }

        public abstract object Execute(object arg, IAsyncTaskProgress progress);

        public abstract void Execute(object arg);
    }

    /// <summary>
    /// 传递的参数
    /// </summary>
    internal class EarlyWarningPluginArgument
    {
        public EarlyWarningPluginArgument(DeviceDataSource deviceDataSource, List<DataNode> dataNodes, EarlyWarningResult earlyWarningResult)
        {
            DeviceDataSource = deviceDataSource;
            DataNodes = dataNodes;
            EarlyWarningResult = earlyWarningResult;
        }

        public EarlyWarningPluginArgument(DeviceDataSource deviceDataSource)
        {
            DeviceDataSource = deviceDataSource;
        }

        public EarlyWarningResult EarlyWarningResult { get; private set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public DeviceDataSource DeviceDataSource { get; private set; }

        /// <summary>
        /// 敏感数据列表
        /// </summary>
        public List<DataNode> DataNodes { get; private set; }
    }
}
