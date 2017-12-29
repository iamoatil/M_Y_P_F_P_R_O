using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Language;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Adapter.Loader
{
    internal class JavascriptPluginLoader : AbstractPluginLoader
    {
        public const string DESKEY = "#s^XLY_DESKEY_1986,11+15";

        public const string JS_EXT = ".js";
        public const string RELEASE_JS_EXT = ".pluginjs";

        protected override void LoadPlugin(IAsyncTaskProgress asyn, params string[] pluginPaths)
        {
            List<IPlugin> pluginList = new List<IPlugin>();

            List<FileInfo> pluginFiles = GetPluginFileList(new[] { JS_EXT, RELEASE_JS_EXT }, pluginPaths);

            var res = System.Threading.Tasks.Parallel.ForEach(pluginFiles, (s) =>
            {
                var plug = this.LoadFile(s.FullName);
                (plug.PluginInfo as DataParsePluginInfo).FileFullPath = s.FullName;
                lock (pluginList)
                {
                    if (plug != null)
                    {
                        pluginList.Add(plug);
                    }
                }
                System.Threading.Thread.Sleep(20);
            });

            Plugins = pluginList;

            //string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "cn");
            //if (!Directory.Exists(dir))
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //string buildDir = @"C:\Projects\SFProject-new\Trunk\Trunk\Source\21-Build";
            //foreach (DataParsePluginInfo pi in pluginList.ConvertAll(p => p.PluginInfo))
            //{
            //    try
            //    {
            //        pi.Guid = Guid.NewGuid().ToString();
            //        string d2 = Path.Combine(dir, Path.GetFileName(pi.FileFullPath));
            //        if (!Directory.Exists(d2))
            //        {
            //            Directory.CreateDirectory(d2);
            //        }
            //        Serializer.SerializeToXML(pi, Path.Combine(d2, "plugin.config"));
            //        File.Copy(pi.FileFullPath, Path.Combine(d2, "main.js"));
            //        if (pi.Icon != null)
            //        {
            //            string iic = pi.Icon.TrimStart('/').TrimStart('\\').Replace("//", "/").Replace(@"\\", @"\");
            //            string icon = Path.Combine(buildDir, iic);
            //            if (File.Exists(icon))
            //            {
            //                var ii = Path.Combine(d2, iic);
            //                if (!Directory.Exists(Path.GetDirectoryName(ii)))
            //                {
            //                    Directory.CreateDirectory(Path.GetDirectoryName(ii));
            //                }
            //                File.Copy(icon, ii);
            //            }
            //        }
            //        string[] script = File.ReadAllLines(pi.FileFullPath, Encoding.UTF8);
            //        foreach (var line in script)
            //        {
            //            if (line.Trim().StartsWith("//"))
            //                continue;
            //            int idnex = line.IndexOf(@"chalib\");
            //            if (idnex > 0)
            //            {
            //                int ed = line.LastIndexOf("\\");
            //                string chalib = line.Substring(idnex, ed - idnex).TrimStart('/').TrimStart('\\').Replace("//", "/").Replace(@"\\", @"\");
            //                if (Directory.Exists(Path.Combine(buildDir, chalib)))
            //                {
            //                    FileHelper.CopyDirectory(Path.Combine(buildDir, chalib), Path.Combine(d2, chalib));
            //                }
            //            }
            //        }
            //        ZipFile.CreateFromDirectory(d2, Path.Combine(dir, Path.GetFileNameWithoutExtension(pi.FileFullPath) + ".zip"));
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}
        }

        private IPlugin LoadFile(string file)
        {
            try
            {
                var plug = TryParseScritpPluginFile(file);
                return plug;
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 解析脚本插件
        /// </summary>
        private DataJSScriptPlugin TryParseScritpPluginFile(string file)
        {
            if (String.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                throw new Exception("No Plugin File");
            }

            string jsContent = file.EndsWith(JS_EXT) ? File.ReadAllText(file) : CryptographyHelper.DecodeDES(File.ReadAllText(file));
            return TryParseScritpPlugin(jsContent);
        }

        /// <summary>
        /// 解析脚本插件
        /// </summary>
        private DataJSScriptPlugin TryParseScritpPlugin(string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                throw new Exception("Plugin content is empty");
            }
            var plug = new DataJSScriptPlugin();
            var reg = new Regex(@"(?<=\[config\]).*(?=\[config\])",
                                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            var config = reg.Match(content).Value;
            var js = content;
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo()
            {
                ScriptObject = js,
                PluginType = PluginType.SpfDataParse
            };
            //解析xml配置，装配
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(config);
            var root = doc.DocumentElement;
            if (root == null || root.Name != "plugin")
            {
                throw new Exception("No Plugin Config");
            }
            this.ReadPluginConfig(pluginInfo, root);
            this.ReadSourceNode(pluginInfo, root);
            this.ReadIncludeFile(pluginInfo, root);
            this.ReadDataNode(pluginInfo, root);
            pluginInfo.AfterReadConfigure();
            plug.PluginInfo = pluginInfo;
            return plug;
        }

        /// <summary>
        /// 读取节点基本配置
        /// </summary>
        private void ReadPluginConfig(DataParsePluginInfo plugin, XmlNode node)
        {
            var n = node.GetSafeAttributeValue("name").Replace("，", ",");
            var g = node.GetSafeAttributeValue("group").Replace("，", ",");
            var ns = n.Split(',');
            var gs = g.Split(',');
            if (ns.Length >= 2)
            {
                plugin.OrderIndex = int.Parse(ns[1]);
            }
            if (gs.Length >= 2)
            {
                plugin.GroupIndex = int.Parse(gs[1]);
            }

            plugin.Name = ns[0];
            plugin.Group = gs[0];
            plugin.DeviceOSType = this.ReadEnum(node, "devicetype", plugin.DeviceOSType);
            plugin.Pump = this.ReadEnum(node, "pump", plugin.Pump);
            plugin.AppName = node.GetSafeAttributeValue("app");
            plugin.VersionStr = node.GetSafeAttributeValue("version").ToSafeString();
            plugin.Description = node.GetSafeAttributeValue("description");
            plugin.Icon = node.GetSafeAttributeValue("icon").Replace('/', '\\');
            plugin.Manufacture = node.GetSafeAttributeValue("manufacture");

        }

        /// <summary>
        /// 读取并转化节点中的枚举属性
        /// </summary>
        private T ReadEnum<T>(XmlNode node, string att, T dv)
        {
            var value = node.GetSafeAttributeValue(att);
            try
            {
                if (value.IsValid())
                {
                    return value.Replace("，", ",").ToEnum<T>();
                }
                return dv;
            }
            catch (Exception)
            {
                throw new ApplicationException("No Data Config");
            }
        }

        #region ReadSourceNode
        /// <summary>
        /// 读取节点的源数据文件路径定义
        /// </summary>
        private void ReadSourceNode(DataParsePluginInfo plugin, XmlNode node)
        {
            var source = node.SelectSingleNode("source");
            if (source == null || !source.HasChildNodes)
            {
                return;
            }
            var values = source.SelectNodes("value");
            if (values == null || values.Count <= 0)
            {
                return;
            }
            List<String> items = new List<string>();
            foreach (XmlNode v in values)
            {
                items.Add(v.InnerText);
            }
            //plugin.SourcePath = new SourceFileItems();
            //plugin.SourcePath.AddItems(items);
            plugin.SourcePathStr = new List<string>(items);
        }
        #endregion

        #region ReadIncludeFile
        /// <summary>
        /// 读取引用的外部文件，该文件路径为相对于exe程序的相对路径
        /// </summary>
        private void ReadIncludeFile(DataParsePluginInfo plugin, XmlNode node)
        {
            var source = node.SelectSingleNode("include");
            if (source == null || !source.HasChildNodes)
            {
                return;
            }
            var values = source.SelectNodes("script");
            if (values == null || values.Count <= 0)
            {
                return;
            }
            foreach (XmlNode v in values)//通过相对路径得到绝对路径
            {
                System.Uri baseUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "main.exe"));
                Uri addr = new Uri(baseUri, v.InnerText);
                if (System.IO.File.Exists(addr.LocalPath))
                {
                    string content = System.IO.File.ReadAllText(addr.LocalPath, Encoding.UTF8);
                    //content = LanguageHelper.SetScriptFileLanguage(content);        //修改脚本中的语言，比如改为英文
                    plugin.ScriptObject += Environment.NewLine + Environment.NewLine + content;      //将引用的文件加到末尾
                }
            }
        }
        #endregion

        #region ReadDataNode
        /// <summary>
        /// 读取节点的自定义数据格式
        /// </summary>
        private void ReadDataNode(DataParsePluginInfo plugin, XmlNode node)
        {
            plugin.DataView = new List<DataView>();

            var datas = node.SelectNodes("data");
            if (datas == null || datas.Count <= 0)
            {
                return;
            }
            foreach (XmlNode data in datas)
            {
                DataView dv = new DataView();
                plugin.DataView.Add(dv);

                dv.Contract = data.GetSafeAttributeValue("contract");
                dv.Type = data.GetSafeAttributeValue("type");
                dv.Items = new List<DataItem>();

                var items = data.SelectNodes("item");
                if (items == null || items.Count <= 0)
                {
                    return;
                }

                foreach (XmlNode v in items)
                {
                    var di = new DataItem()
                    {
                        Name = v.GetSafeAttributeValue("name"),
                        Code = v.GetSafeAttributeValue("code"),
                        Type = this.ReadEnum(v, "type", EnumColumnType.String),
                        Width = v.GetSafeAttributeValue("width").ToSafeInt(),
                        Format = v.GetSafeAttributeValue("format"),
                        Order = this.ReadEnum(v, "order", EnumOrder.None),
                        Alignment = this.ReadEnum(v, "alignment", EnumAlignment.Left),
                        GroupIdex = v.GetSafeAttributeValue("groupindex").ToSafeInt(),
                        Show = v.GetSafeAttributeValue("show").ToSafeBoolean(true),
                    };
                    di.Width = di.Width <= 0 ? 100 : di.Width;
                    di.GroupIdex = di.GroupIdex > 0 ? di.GroupIdex : -1;
                    dv.Items.Add(di);
                }
            }
        }


        /// <summary>
        /// 添加固定数据列MD5
        /// </summary>
        /// <returns></returns>
        private DataItem GetMD5Item()
        {
            DataItem item = new DataItem
            {
                Name = "MD5",
                Code = "MDFString",
                Type = EnumColumnType.String,
                Width = 100,
                Format = string.Empty,
                Order = EnumOrder.None,
                Alignment = EnumAlignment.Left
            };
            return item;
        }

        #endregion
    }
}
