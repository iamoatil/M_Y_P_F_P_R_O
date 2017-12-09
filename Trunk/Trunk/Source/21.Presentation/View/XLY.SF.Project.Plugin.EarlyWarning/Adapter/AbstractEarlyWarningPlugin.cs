using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
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
    internal abstract class AbstractEarlyWarningPlugin : IPlugin
    {
        /// <summary>
        /// 列更新器
        /// </summary>
        readonly ColumnUpdater _columnUpdater = new ColumnUpdater();

        public IPluginInfo PluginInfo { get; set; }
        
        public virtual void Dispose()
        {
            _columnUpdater.Dispose();
        }

        public abstract object Execute(object arg, IAsyncTaskProgress progress);

        protected void KeyWordColumnUpdate(IDataItems dataItems, DbFromConfigData configDbManager)
        {
            _columnUpdater.Initialize(dataItems.DbFilePath, dataItems.DbTableName, configDbManager.DbPath, ConstDefinition.XLYJson);
            _columnUpdater.KeyWordColumnUpdate();
        }

        protected void ColumnUpdate(IDataItems dataItems, DbFromConfigData configDbManager, string keyColumn)
        {
            _columnUpdater.Initialize(dataItems.DbFilePath, dataItems.DbTableName, configDbManager.DbPath, keyColumn);
            _columnUpdater.Update();
        }
    }

    /// <summary>
    /// 传递的参数
    /// </summary>
    internal class EarlyWarningPluginArgument
    {
        public EarlyWarningPluginArgument(DeviceDataSource deviceDataSource, DbFromConfigData configDbManager)
        {
            DeviceDataSource = deviceDataSource;
            ConfigDbManager = configDbManager;
        }        

        /// <summary>
        /// 数据源
        /// </summary>
        public DeviceDataSource DeviceDataSource { get; private set; }

        /// <summary>
        /// ConfigDataToDB
        /// </summary>
        public DbFromConfigData ConfigDbManager { get; private set; }
    }
}
