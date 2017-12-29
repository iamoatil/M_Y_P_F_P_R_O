using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataView;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.DataViewPluginAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:06:18
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// 数据展示插件适配器，用于加载插件和适配插件
    /// </summary>
    public class DataViewPluginAdapter
    {
        public DataViewPluginAdapter()
        {
            //Plugins = PluginAdapter.Instance.GetPluginsByType<DataViewPluginInfo>(PluginType.SpfDataView).ToList().ConvertAll(p => (AbstractDataViewPlugin)p.Value);
        }

        public static DataViewPluginAdapter Instance => SingleWrapperHelper<DataViewPluginAdapter>.Instance;

        /// <summary>
        /// 当前支持的所有数据视图插件
        /// </summary>
        public IEnumerable<AbstractDataViewPlugin> Plugins { get; set; }

        /// <summary>
        /// 根据当前选择的数据获取视图列表
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AbstractDataViewPlugin> GetView(string pluginId, object type, DataViewConfigure config = null)
        {
            if (type == null)
            {
                return new List<AbstractDataViewPlugin>();
            }

            config = config ?? DataViewConfigure.Default;

            string typeName = (type is Type) ? ((Type)type).Name : type.ToSafeString();
            var views = (typeName == DataViewConfigure.XLY_LAYOUT_KEY ? 
                Plugins.Where(p =>
               ((DataViewPluginInfo)p.PluginInfo).ViewType.Any(v => (v.PluginId.Equals(pluginId) || v.PluginId == "*") && (v.TypeName.Equals(typeName))))
               //.OrderByDescending(iv => iv.PluginInfo.OrderIndex)
               :
               Plugins.Where(p =>
               ((DataViewPluginInfo)p.PluginInfo).ViewType.Any(v => (v.PluginId.Equals(pluginId) || v.PluginId == "*") && (v.TypeName.Equals(typeName) || v.TypeName == "*")))
               //.OrderByDescending(iv => iv.PluginInfo.OrderIndex)
               )
               .ToList();

            if(type is Type t)      //如果插件支持继承匹配
            {
                views.AddRange(Plugins.Where(p => ((DataViewPluginInfo)p.PluginInfo).ViewType.Any(v => v.Inherit && IsAssignFromClass(t, v.TypeName))));
            }
            
            if (views.Count > 1 && !config.IsDefaultGridViewVisibleWhenMultiviews)  //当存在多个视图时，是否隐藏默认的表格视图
            {
                for (int i = views.Count - 1; i >= 0; i--)
                {
                    if(views[i].PluginInfo.Guid == DataViewConfigure.DEFAULT_GRID_VIEW_ID) //移除默认的表格视图
                    {
                        views.RemoveAt(i);
                    }
                }
            }

            //移除重复插件并排序
            return views.DistinctX(v => v.PluginInfo.Guid).OrderByDescending(iv => iv.PluginInfo.OrderIndex);
        }

        private bool IsAssignFromClass(Type t, string parentName)
        {
            if (t == null)
                return false;
            if (t.Name.Equals(parentName))
            {
                return true;
            }
            return IsAssignFromClass(t.BaseType, parentName) || t.GetInterfaces().Any(i=>IsAssignFromClass(i, parentName));
        }
    }
}
