/* ==============================================================================
* Description：AppWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:58
* ==============================================================================*/


using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class AppWarningPlugin : AbstractEarlyWarningPlugin
    {
        public AppWarningPlugin()
        {
            var p = new AppEarlyWarningPluginInfo()
            {
                Guid = "{E3422DF4-B69A-4642-8420-B69ABCB413A4}",
                Name = "AppWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,               
            };
            PluginInfo = p;
        }        

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            EarlyWarningPluginArgument ewArg = (EarlyWarningPluginArgument)arg;
            DeviceDataSource ds = ewArg.DeviceDataSource;

            AbstractDataSource dataSource = ds.DataSource as AbstractDataSource;

            if (dataSource == null
                || dataSource.Items == null)
            {
                return null;
            }
            string keyColumn = "Name";
            ColumnUpdate(dataSource.Items.DbTableName, keyColumn);
            return null;
        }
        
    }
}
