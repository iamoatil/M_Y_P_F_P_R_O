/* ==============================================================================
* Description：UrlWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:26
* ==============================================================================*/

using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class UrlWarningPlugin : AbstractEarlyWarningPlugin
    {
        public UrlWarningPlugin()
        {
            var p = new UrlEarlyWarningPluginInfo()
            {
                Guid = "{F8EB7422-6C4E-43A1-9C3B-D5FF04371268}",
                Name = "UrlWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning
            };
            PluginInfo = p;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            EarlyWarningPluginArgument ewArg = (EarlyWarningPluginArgument)arg;
            DeviceDataSource ds = ewArg.DeviceDataSource;
            DbFromConfigData configDbManager = ewArg.ConfigDbManager;

            TreeDataSource treeDataSource = ds.DataSource as TreeDataSource;
            if (treeDataSource == null
                || treeDataSource.TreeNodes.Count < 1)
            {
                return null;
            }
            string keyColumn = "Url";
            foreach (TreeNode treeNode in treeDataSource.TreeNodes)
            {
                ColumnUpdate(treeNode.Items, configDbManager, keyColumn);               
            }
            return null;
        }       
    }
}
