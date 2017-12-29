using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter.Loader;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 插件控制器
    /// </summary>
    public class PluginAdapter
    {
        public static PluginAdapter Instance => SingleWrapperHelper<PluginAdapter>.Instance;

        /// <summary>
        /// 插件列表
        /// </summary>
        public List<IPlugin> Plugins { get; set; }

        #region 初始化控制器

        private bool _Isloaded = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialization(IAsyncTaskProgress asyn, params string[] pluginPaths)
        {
            if (!_Isloaded)
            {
                PluginContainerAdapter.Instance.Initialization(asyn, pluginPaths);

                _Isloaded = true;
                Plugins = new List<IPlugin>();
                var pluginLoaders = new List<IPluginLoader>() { new NetPluginLoader(), new ZipPluginLoader() };

                foreach (var loader in pluginLoaders)
                {
                    var pls = loader.Load(asyn, pluginPaths);

                    foreach (var pl in pls)
                    {
                        if (null != pl.PluginInfo)
                        {
                            Plugins.Add(pl);
                        }
                    }
                }

                //加载特征匹配服务
                PluginFeatureMathchService.LoadService();
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 通过插件类型和状态获取当前所有的插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">插件类型</param>
        /// <param name="state">插件状态</param>
        /// <returns>满足条件的所有插件</returns>
        public Dictionary<T, IPlugin> GetPluginsByType<T>(PluginType type = PluginType.SpfDataParse, PluginState state = PluginState.Normal) where T : AbstractPluginInfo
        {
            var plugin = state == PluginState.None ?
                                  Plugins.Where(p => p.PluginInfo.PluginType == type && p.PluginInfo is T) :
                                  Plugins.Where(p => p.PluginInfo.PluginType == type && p.PluginInfo is T && state.HasFlag(p.PluginInfo.State));

            return plugin.ToDictionary(x => (T)x.PluginInfo, x => x);
        }

        /// <summary>
        /// 获取数据解析插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IEnumerable<AbstractDataParsePlugin> GetDataParserPluginsByType<T>(PluginType type = PluginType.SpfDataParse, PluginState state = PluginState.Normal) where T : AbstractPluginInfo
        {
            var plugin = state == PluginState.None ?
                      Plugins.Where(p => p.PluginInfo.PluginType == type && p.PluginInfo is T).Select(p => p as AbstractDataParsePlugin) :
                      Plugins.Where(p => p.PluginInfo.PluginType == type && p.PluginInfo is T && state.HasFlag(p.PluginInfo.State)).Select(p => p as AbstractDataParsePlugin);

            //这儿要做深拷贝
            return plugin.Select(p =>
            {
                var ptype = p.GetType();
                var np = ptype.Assembly.CreateInstance(ptype.FullName) as AbstractDataParsePlugin;
                np.PluginInfo = DeepCopyWithBinarySerialize(p.PluginInfo);

                return np;
            });
        }

        /// <summary>
        /// 获取提取项列表
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <returns></returns>
        public List<ExtractItem> GetAllExtractItems(Pump source)
        {
            List<ExtractItem> extracts = new List<ExtractItem>();

            //插件过滤 根据提取方式和操作系统
            //本地提取时，不根据设备操作系统类型匹配，因为此时不知道本地文件的来源
            var filtetResult = GetPluginsByType<DataParsePluginInfo>(PluginType.SpfDataParse).Keys.Where(p => p.Pump.HasFlag(source.Type) &&
                                                        (source.Type == EnumPump.LocalData || p.DeviceOSType.HasFlag(source.OSType))).ToList();

            //排序
            filtetResult.Sort((l, r) =>
                {
                    if (l.GroupIndex > r.GroupIndex)
                    {
                        return 1;
                    }
                    else if (l.GroupIndex < r.GroupIndex)
                    {
                        return -1;
                    }
                    else if (l.OrderIndex > r.OrderIndex)
                    {
                        return 1;
                    }
                    else if (l.OrderIndex < r.OrderIndex)
                    {
                        return -1;
                    }

                    return 0;
                });

            foreach (var plug in filtetResult)
            {
                //if (!extracts.Any(e => e.AppName == plug.AppName && e.Name == plug.Name && e.GroupName == plug.Group))
                if (!extracts.Any(e => e.Name == plug.Name && e.GroupName == plug.Group))
                {
                    ExtractItem item = new ExtractItem();
                    item.Name = plug.Name;
                    item.GroupName = plug.Group;
                    item.AppName = plug.AppName;
                    item.Icon = plug.Icon;

                    extracts.Add(item);
                }
            }

            return extracts;
        }

        public List<ExtractItem> GetExtractItems(DataParsePluginInfo plugin)
        {
            return new List<ExtractItem>() { new ExtractItem() { Name = plugin.Name, AppName = plugin.AppName } };
        }

        /// <summary>
        /// 智能匹配插件 按过滤器条件
        /// </summary>
        /// <param name="pluginList"></param>
        /// <param name="pluginFilters"></param>
        /// <returns></returns>
        public List<DataParsePluginInfo> SmartMatchPlugin(List<DataParsePluginInfo> pluginList, List<PluginMatchFilter> filterList)
        {
            List<DataParsePluginInfo> resultList = new List<DataParsePluginInfo>();

            foreach (var item in filterList)
            {
                var result = pluginList.Where(p => p.AppName == item.AppName);
                if (result.Count() == 0)
                    return resultList;
                result = result.Where(p => p.DeviceOSType.HasFlag(item.EnumOSType));
                if (result.Count() == 0)
                    return resultList;
                result = result.Where(p => p.Pump.HasFlag(item.PumpType));
                if (result.Count() == 0)
                    return resultList;

                if (item.AppVersion.Equals(new Version("0.0")))
                {
                    resultList.AddRange(result);
                    continue;
                }

                if (result.Count() == 1)
                {
                    resultList.AddRange(result);
                    continue;
                }

                if (item.AppName.StartsWith("com.apple") && item.EnumOSType == EnumOSType.IOS)
                {
                    var r = VersionSmartMatch(result, item.AppVersion);
                }

                //安卓QQ微信不进行版本匹配
                if (item.AppName == "com.tencent.mobileqq" || item.AppName == "com.tencent.mm")
                {
                    resultList.AddRange(result);
                    continue;
                }


                if (item.Manufacture != null)
                {
                    result = result.Where(p => p.Manufacture.Equals(item.Manufacture, StringComparison.OrdinalIgnoreCase)
                            || (p.Manufacture.Equals(item.Brand, StringComparison.OrdinalIgnoreCase)));
                }

                var res = VersionSmartMatch(pluginList.Where(p => p.AppName == item.AppName), item.AppVersion);
                if (res != null)
                    resultList.Add(res);
            }

            return resultList;
        }

        /// <summary>
        /// 版本匹配（优先级：相等=>高版本=>低版本）
        /// </summary>
        private DataParsePluginInfo VersionSmartMatch(IEnumerable<DataParsePluginInfo> plugins, Version version)
        {
            try
            {
                if (plugins.IsInvalid())
                {
                    return null;
                }
                //if only one
                if (plugins.Count() == 1)
                {
                    return plugins.First();
                }
                // 相等
                var p = plugins.Where(s => s.Version == version);
                if ((p != null) && (p.Count() > 0))
                {
                    return p.First();
                }

                // 低版本 最近一个
                var less = plugins.Where(s => s.Version < version);
                if ((less != null) && (less.Count() > 0))
                {
                    return less.OrderByDescending(s => s.Version).First();
                }

                // 高版本 最近一个
                var greater = plugins.Where(s => s.Version > version);
                if ((greater != null) && (greater.Count() > 0))
                {
                    return greater.OrderBy(s => s.Version).First();
                }

                //return the latest version plugin
                return plugins.OrderByDescending(s => s.Version).First();
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 匹配插件（根据数据泵，即设备类型、设备操作系统类型、数据提取类型匹配插件）
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <param name="extractItems">选择的提取项</param>
        /// <returns></returns>
        public Dictionary<ExtractItem, List<AbstractDataParsePlugin>> MatchPluginByPump(Pump source, IEnumerable<ExtractItem> extractItems)
        {
            var result = new Dictionary<ExtractItem, List<AbstractDataParsePlugin>>();

            //插件过滤 根据提取方式和操作系统
            //本地提取时，不根据设备操作系统类型匹配，因为此时不知道本地文件的来源
            var filtetResult = GetDataParserPluginsByType<DataParsePluginInfo>(PluginType.SpfDataParse).Where(p => p.DataParsePluginInfo.Pump.HasFlag(source.Type) &&
                                                            (source.Type == EnumPump.LocalData || p.DataParsePluginInfo.DeviceOSType.HasFlag(source.OSType)));

            //插件匹配
            foreach (var extract in extractItems)
            {
                result.Add(extract, filtetResult.Where(p => p.PluginInfo.Name == extract.Name && p.PluginInfo.Group == extract.GroupName).ToList());
            }

            return result;
        }

        /// <summary>
        /// 匹配插件（根据特征库和APP版本信息匹配插件）
        /// </summary>
        /// <param name="pump">数据泵类型</param>
        /// <param name="appSourePath">APP本地数据根目录</param>
        /// <param name="appVersion">APP版本信息</param>
        /// <returns></returns>
        public AbstractDataParsePlugin MatchPluginByApp(IEnumerable<AbstractDataParsePlugin> pluginList, Pump pump, string appSourePath, Version appVersion)
        {
            return PluginFeatureMathchService.FeatureMathch(pluginList, pump, appSourePath, appVersion);
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="plugin">要执行的插件</param>
        /// <param name="asyn">异步通知</param>
        /// <returns>插件执行结果。</returns>
        public IDataSource ExecutePlugin(AbstractDataParsePlugin pl, IAsyncTaskProgress asyn)
        {
            IDataSource ds = null;
            if (null != pl)
            {
                pl.StartTime = DateTime.Now;

                try
                {
                    ds = pl.Execute(null, asyn) as IDataSource;
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, $"执行插件{pl.PluginInfo.Name}出错！");
                }
                finally
                {
                    ds?.BuildParent();
                }

                if (ds != null)
                {
                    ds.PluginInfo = pl.DataParsePluginInfo;
                }
                pl.EndTime = DateTime.Now;
            }
            return ds;
        }

        #endregion

        // 利用二进制序列化和反序列实现
        private static T DeepCopyWithBinarySerialize<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                // 序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                // 反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }

            return (T)retval;
        }

    }
}
