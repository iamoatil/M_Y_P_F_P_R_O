using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataPreview;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataPreview.DataPreviewPluginAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:06:18
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataPreview
{
    /// <summary>
    /// 数据预览插件适配器，用于加载插件和适配插件
    /// </summary>
    public class DataPreviewPluginAdapter
    {
        public DataPreviewPluginAdapter()
        {
        }

        public static DataPreviewPluginAdapter Instance => SingleWrapperHelper<DataPreviewPluginAdapter>.Instance;

        /// <summary>
        /// 当前支持的所有数据视图插件
        /// </summary>
        public IEnumerable<AbstractDataPreviewPlugin> Plugins { get; set; }

        /// <summary>
        /// 根据当前选择的数据获取视图列表
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AbstractDataPreviewPlugin> GetView(DataPreviewPluginArgument arg)
        {
            if (arg.CurrentData == null)
            {
                return new List<AbstractDataPreviewPlugin>();
            }

            if (arg.CurrentData is string file)     //如果是字符串则认为是文件，此时通过文件后缀名判断
            {
                if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
                {
                    return new List<AbstractDataPreviewPlugin>();
                }
                string ext = System.IO.Path.GetExtension(file);
                var views = Plugins.Where(p =>
                   ((DataPreviewPluginInfo)p.PluginInfo).ViewType.Any(v => v.PluginId != "*" && (v.PluginId == "*.*" || v.PluginId.Replace("*", "").Split('|').Any(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))))
                   .OrderByDescending(iv => iv.PluginInfo.OrderIndex)
                   .ToList();
                return views;
            }
            else   //否则认为是数据对象，此时通过类型判断
            {
                if (arg.Type == null || arg.PluginId == null)
                {
                    arg.Type = arg.PluginId = "*";
                    //return new List<AbstractDataPreviewPlugin>();
                }
                string typeName = (arg.Type is Type) ? ((Type)arg.Type).Name : arg.Type.ToSafeString();
                var views = Plugins.Where(p =>
                   ((DataPreviewPluginInfo)p.PluginInfo).ViewType.Any(v => (v.PluginId.Equals(arg.PluginId) || v.PluginId == "*") && (v.TypeName.Equals(typeName) || v.TypeName == "*")))
                   .OrderByDescending(iv => iv.PluginInfo.OrderIndex)
                   .ToList();

                return views;
            }
        }
    }
}
