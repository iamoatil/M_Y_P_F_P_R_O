using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 插件特征匹配服务
    /// </summary>
    public static class PluginFeatureMathchService
    {
        private static IList<IPluginFeatureMathch> Features { get; set; }

        /// <summary>
        /// 加载插件匹配特征库
        /// </summary>
        public static void LoadService()
        {
            Features = new List<IPluginFeatureMathch>();

            #region 读取xml文件获取特征匹配库

            try
            {
                string configStr;

                foreach (var ostype in new string[] { "IOS", "Android" })
                {
                    string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, String.Format(@"Config\PluginFeatureMathchConfig_{0}.xml", ostype));

                    if (Framework.Language.LanguageManager.Current.Type == Framework.Language.LanguageType.En)
                    {
                        filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, String.Format(@"Config\PluginFeatureMathchConfig_{0}_en.xml", ostype));
                    }

                    if (!File.Exists(filepath))
                    {
                        continue;
                    }
                    using (Stream sm = new FileStream(filepath, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(sm))
                        {
                            configStr = sr.ReadToEnd();
                        }
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(configStr);

                    foreach (XmlNode node in xmlDoc.SelectSingleNode("root").SelectNodes("PluginFeatureMathch"))
                    {
                        DefaultPluginFeatureMathch pfm = new DefaultPluginFeatureMathch();
                        pfm.AppName = node.Attributes["app"].Value;
                        pfm.OSType = (EnumOSType)Enum.Parse(typeof(EnumOSType), ostype);
                        if (null != node.Attributes["Manufacture"] && node.Attributes["Manufacture"].Value.IsValid())
                        {
                            pfm.Manufacture = node.Attributes["Manufacture"].Value;
                        }

                        foreach (XmlNode ruleNode in node.SelectNodes("Rule"))
                        {
                            switch (ruleNode.Attributes["Type"].Value)
                            {
                                case "PathExistPluginFeatureRule":
                                    var pathRule = new PathExistPluginFeatureRule();
                                    pathRule.Id = ruleNode.Attributes["ID"].Value;
                                    pathRule.Path = ruleNode.Attributes["Path"].Value;
                                    pathRule.Success = ruleNode.Attributes["Success"].Value;
                                    pathRule.Failure = ruleNode.Attributes["Failure"].Value;
                                    pfm.Rules.Add(pathRule);
                                    break;
                                case "FileExistPluginFeatureRule":
                                    var fileRule = new FileExistPluginFeatureRule();
                                    fileRule.Id = ruleNode.Attributes["ID"].Value;
                                    fileRule.Path = ruleNode.Attributes["Path"].Value;
                                    fileRule.FileName = ruleNode.Attributes["FileName"].Value;
                                    fileRule.Success = ruleNode.Attributes["Success"].Value;
                                    fileRule.Failure = ruleNode.Attributes["Failure"].Value;
                                    pfm.Rules.Add(fileRule);
                                    break;
                                case "TableExistPluginFeatureRule":
                                    var tableRule = new TableExistPluginFeatureRule();
                                    tableRule.Id = ruleNode.Attributes["ID"].Value;
                                    tableRule.Path = ruleNode.Attributes["Path"].Value;
                                    tableRule.DbFileName = ruleNode.Attributes["DbFileName"].Value;
                                    tableRule.TableName = ruleNode.Attributes["TableName"].Value;
                                    tableRule.Decryted = null != ruleNode.Attributes["Decryted"] && "true" == ruleNode.Attributes["Decryted"].Value.ToLower();
                                    tableRule.Success = ruleNode.Attributes["Success"].Value;
                                    tableRule.Failure = ruleNode.Attributes["Failure"].Value;
                                    pfm.Rules.Add(tableRule);
                                    break;
                                case "FieldExistPluginFeatureRule":
                                    var fieldRule = new FieldExistPluginFeatureRule();
                                    fieldRule.Id = ruleNode.Attributes["ID"].Value;
                                    fieldRule.Path = ruleNode.Attributes["Path"].Value;
                                    fieldRule.DbFileName = ruleNode.Attributes["DbFileName"].Value;
                                    fieldRule.TableName = ruleNode.Attributes["TableName"].Value;
                                    fieldRule.FiledName = ruleNode.Attributes["FiledName"].Value;
                                    fieldRule.Decryted = null != ruleNode.Attributes["Decryted"] && "true" == ruleNode.Attributes["Decryted"].Value.ToLower();
                                    fieldRule.Success = ruleNode.Attributes["Success"].Value;
                                    fieldRule.Failure = ruleNode.Attributes["Failure"].Value;
                                    pfm.Rules.Add(fieldRule);
                                    break;
                                default:
                                    break;
                            }
                        }

                        Features.Add(pfm);
                    }
                }

            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("特征匹配库加载失败，错误信息：{0}", ex));
            }

            #endregion

        }

        public static AbstractDataParsePlugin FeatureMathch(IEnumerable<AbstractDataParsePlugin> pluginList, Pump pump, string appSourePath, Version appVersion)
        {
            //只有一个插件的话直接返回
            if (1 == pluginList.Count())
            {
                return pluginList.FirstOrDefault();
            }

            var deault = pluginList.First();

            //根据所有路径无效过滤掉部分插件
            //注意 部分插件没有配置路径
            pluginList = pluginList.Where(f => !(f.PluginInfo as DataParsePluginInfo).SourcePath.Any() || f.DataParsePluginInfo.SourcePath.Any(i => i.Local.IsValid()));

            var countPlugin = pluginList.Count();

            //没有插件
            if (0 == countPlugin)
            {
                return deault;
            }

            //只有一个插件的话直接返回
            if (1 == countPlugin)
            {
                return pluginList.FirstOrDefault();
            }

            //从所有的特征库里面，取出匹配的特征库
            //注意，本地提取时，无法区分是安卓还是IOS的数据，所有匹配的时候忽略操作系统
            var res = TryFeatureMathch(appSourePath, pump.Type == EnumPump.LocalData ? EnumOSType.None : pump.OSType, pluginList.First().DataParsePluginInfo.Name).Where((f) => f.IsSuccessed);
            if (res.IsValid())
            {
                //匹配成功，优先采用厂商插件
                PluginFeatureMathchResult mp = res.FirstOrDefault(f => f.Manufacture.IsValid());
                if (null != mp && pluginList.Any(p => p.DataParsePluginInfo.DeviceOSType == mp.OSType && p.DataParsePluginInfo.Manufacture == mp.Manufacture))
                {
                    pluginList = pluginList.Where(p => p.DataParsePluginInfo.DeviceOSType == mp.OSType && p.DataParsePluginInfo.Manufacture == mp.Manufacture).ToList();
                }
                else
                {
                    mp = res.FirstOrDefault(f => f.Manufacture.IsInvalid());
                    pluginList = pluginList.Where(p => p.DataParsePluginInfo.DeviceOSType == mp.OSType && p.DataParsePluginInfo.Manufacture.IsInvalid()).ToList();
                }
                return VersionSmartMathch(pluginList, new Version(mp.AppVersion));
            }

            //匹配失败，就根据版本号来匹配
            return VersionSmartMathch(pluginList, appVersion);
        }

        #region VersionSmartMathch

        /// <summary>
        /// 版本匹配（优先级：相等=>高版本=>低版本）
        /// </summary>
        private static AbstractDataParsePlugin VersionSmartMathch(IEnumerable<AbstractDataParsePlugin> plugins, Version version)
        {
            try
            {
                //if only one
                if (plugins.Count() <= 1)
                {
                    return plugins.First();
                }

                if (null == version)
                {
                    return plugins.First();
                }

                // 相等
                var p = plugins.Where(s => s.DataParsePluginInfo.Version == version);
                if (p.IsValid())
                {
                    return p.First();
                }

                // 低版本 最近一个
                var less = plugins.Where(s => s.DataParsePluginInfo.Version < version);
                if (less.IsValid())
                {
                    return less.OrderByDescending(s => s.DataParsePluginInfo.Version).First();
                }

                // 高版本 最近一个
                var greater = plugins.Where(s => s.DataParsePluginInfo.Version > version);
                if (greater.IsValid())
                {
                    return greater.OrderBy(s => s.DataParsePluginInfo.Version).First();
                }

                //return the latest version plugin
                return plugins.OrderByDescending(s => s.DataParsePluginInfo.Version).First();
            }
            catch (Exception e)
            {
                LoggerManagerSingle.Instance.Error(e.Message, e);
            }
            return null;
        }

        #endregion

        private static readonly PluginFeatureMathchResult Error = new PluginFeatureMathchResult() { IsSuccessed = false };

        public static IList<PluginFeatureMathchResult> TryFeatureMathch(string path, EnumOSType ostype, string appName)
        {
            try
            {
                return Features.Where(f => (f.OSType == ostype || ostype == EnumOSType.None) && f.AppName == appName).Select((f) => f.TryMathch(path) ?? Error).ToList();
            }
            catch (Exception ex)
            {

                LoggerManagerSingle.Instance.Error(string.Format("特征匹配错误，错误信息:{0}", ex));
                return null;
            }
        }
    }
}
