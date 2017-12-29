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
        //列更新器
        private DataDbColumnUpdater _dataDbColumnUpdater ;        

        public IPluginInfo PluginInfo { get; set; }

        public abstract object Execute(object arg, IAsyncTaskProgress progress);

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 设置插件中的列更新器
        /// </summary>
        public void SetColumnUpdater(DataDbColumnUpdater dataDbColumnUpdater)
        {
            IsInitialized = false;
            _dataDbColumnUpdater = dataDbColumnUpdater;
            IsInitialized = true;            
        }
        protected void KeyWordColumnUpdate(string tableName)
        {
            if(!IsInitialized)
            {
                return;
            }
            bool isSuc = _dataDbColumnUpdater.CheckTableAndColumn(tableName, ConstDefinition.XLYJson);
            if (isSuc)
            {
                _dataDbColumnUpdater.AttachConfigDataBase();
                _dataDbColumnUpdater.Update();
            }
        }

        protected void ColumnUpdate(string tableName, string keyColumn)
        {
            if (!IsInitialized)
            {
                return;
            }
            bool isSuc=_dataDbColumnUpdater.CheckTableAndColumn(tableName, keyColumn);
            if(isSuc)
            {
                _dataDbColumnUpdater.AttachConfigDataBase();
                _dataDbColumnUpdater.Update();
            }
        }

        public void Dispose()
        {
            
        }
    }

    /// <summary>
    /// 传递的参数
    /// </summary>
    internal class EarlyWarningPluginArgument
    {
        public EarlyWarningPluginArgument(DeviceDataSource deviceDataSource)
        {
            DeviceDataSource = deviceDataSource;
        }        

        /// <summary>
        /// 数据源
        /// </summary>
        public DeviceDataSource DeviceDataSource { get; private set; }
    }
}
