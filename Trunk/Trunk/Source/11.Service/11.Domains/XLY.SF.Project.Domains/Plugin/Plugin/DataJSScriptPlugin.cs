using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// JavaScript/python脚本插件
    /// </summary>
    [PluginContainer(PluginType.SpfDataParse)]
    public class DataJSScriptPlugin : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            DataParsePluginInfo p = PluginInfo as DataParsePluginInfo;

            EngineArg eg = new EngineArg();
            eg.Language = p.Language.ToString();
            eg.ScriptDir = p.ZipTempDirectory;
            eg.ScriptObject = p.ScriptObject;
            eg.ResultFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", String.Format("script_{0}.jscode", Guid.NewGuid().ToString()));
            if (!Directory.Exists(Path.GetDirectoryName(eg.ResultFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(eg.ResultFile));
            }
            //将参数保存到临时文件中，因为该参数长度超过了命令行最大长度
            var argFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", String.Format("script_{0}.params", Guid.NewGuid().ToString()));
            File.WriteAllText(argFile, Serializer.JsonSerializerIO(eg));

            var files = p.SourcePath?.Select(s => $"\"{s.Local.ToSafeString()}\"").ToArray();      //提取文件对应的绝对路径
            if (p.Language == PluginLanguage.JavaScript)     //如果是js脚本，需要先将替换后的js文件内容写入文件中
            {
                var str = "[]";
                if (files.IsValid())
                {
                    str = "[" + string.Join(",", files).Replace(@"\", @"\\") + "]";
                }
                var js = p.ScriptObject.Replace("$source", str);
                System.IO.File.WriteAllText(eg.ResultFile, js, System.Text.Encoding.UTF8);
            }
            else if (p.Language == PluginLanguage.Python36)   //如果是Python脚本，需要设置传入的脚本参数，暂时没时间加
            {

            }
            var json = ExecuteJs(argFile, eg, progress);
            return ToDataSource(json);
        }

        public override void Dispose()
        {

        }

        private static string _ScriptContextRunName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScriptEngine", "XLY.SF.Project.ScriptEngine.exe");

        [MethodImpl(MethodImplOptions.Synchronized)]
        private string ExecuteJs(string argFile, EngineArg engine, IAsyncTaskProgress progress)
        {
            if (!System.IO.File.Exists(_ScriptContextRunName))
            {
                throw new Exception("Not found Script Executive Program at " + _ScriptContextRunName);
            }

            string res = "";
            try
            {
                Process pro = new Process();
                pro.StartInfo.FileName = _ScriptContextRunName;

                pro.StartInfo.Arguments = string.Format("\"{0}\"", argFile);
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;

                pro.Start();
                pro.WaitForExit();

                if (System.IO.File.Exists(engine.ResultFile))
                {
                    res = System.IO.File.ReadAllText(engine.ResultFile, System.Text.Encoding.UTF8);
                    System.IO.File.Delete(engine.ResultFile);
                }
                if (System.IO.File.Exists(argFile))
                {
                    System.IO.File.Delete(argFile);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("执行脚本失败!", ex);
            }

            return res;
        }

        [Serializable]
        class EngineArg
        {
            /// <summary>
            /// 插件的语言，是js/python/html/C#
            /// </summary>
            public string Language { get; set; }
            /// <summary>
            /// 插件解压后临时存放路径
            /// </summary>
            public string ScriptDir { get; set; }
            /// <summary>
            /// 插件对象，如果是js插件，则表示插件的文本内容；如果是Python/Html插件，则表示为插件的路径；如果是C#插件，则为null
            /// </summary>
            public string ScriptObject { get; set; }
            /// <summary>
            /// 结果文件保存的路径，如果是js，则会先将执行的脚本导入该文件
            /// </summary>
            public string ResultFile { get; set; }
        }

        public override string ToString()
        {
            return $"DataJSScriptPlugin -- 名称:{PluginInfo.Name}, 版本:{PluginInfo.Version}, ID:{PluginInfo.Guid}";
        }

        #region 将json字符串转换为IDataSource
        /// <summary>
        /// 将json字符串转换为IDataSource
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private IDataSource ToDataSource(string json)
        {
            DataParsePluginInfo plugin = PluginInfo as DataParsePluginInfo;
            //bool isTree = (plugin.DataView.Any(dv => dv.Type == "TreeNode") && json.Contains("\"TreeNodes\""));
            bool isTree = json.Contains("\"TreeNodes\"");
            return isTree ? ToTreeDataSource(json, plugin) : ToSimpleDataSource(json, plugin);
        }

        /// <summary>
        /// 转换简单类型数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="plugin"></param>
        /// <returns></returns>
        private IDataSource ToSimpleDataSource(string json, DataParsePluginInfo plugin)
        {
            SimpleDataSource ds = new SimpleDataSource();
            ds.Key = Guid.NewGuid();
            ds.Type = plugin.DataView[0].DynamicType;
            ds.IsChecked = false;
            ds.PluginInfo = plugin;

            Type ditype = typeof(DataItems<ScriptDataItem>).GetGenericTypeX(ds.Type);       //生成类型DataItems<T>
            ds.Items = ditype.CreateInstance(plugin.SaveDbPath, true, null, null) as IDataItems;
            JArray array = JArray.Parse(json);
            foreach (var io in array)
            {
                var iiii = io.ToObject(ds.Type);
                if (iiii != null)
                    ds.Items.Add(iiii);
            }
            return ds;
        }

        /// <summary>
        /// 转换树形数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="plugin"></param>
        /// <returns></returns>
        private IDataSource ToTreeDataSource(string json, DataParsePluginInfo plugin)
        {
            TreeDataSource ds = new TreeDataSource();
            ds.Key = Guid.NewGuid();
            ds.Type = null;
            ds.IsChecked = false;
            ds.PluginInfo = plugin;

            JsonToTreeNodes(plugin, ds.TreeNodes, JArray.Parse(json));
            return ds;
        }

        private void JsonToTreeNodes(DataParsePluginInfo plugin, List<TreeNode> tree, JArray jarray)
        {
            if (jarray == null)
                return;
            foreach (JObject jo in jarray)
            {
                TreeNode node = new TreeNode()
                {
                    Text = TryGetJObjectProperty(jo, "Text", ""),
                    TreeNodes = new List<TreeNode>()
                };
                EnumDataState state;
                if (TryGetJObjectProperty(jo, "DataState", "Normal").TryToEnum<EnumDataState>(out state))
                {
                    node.DataState = state;
                }
                else
                {
                    node.DataState = EnumDataState.Normal;
                }
                string typeStr = TryGetJObjectProperty(jo, "Type", "");
                if (!string.IsNullOrWhiteSpace(typeStr))
                {
                    node.Type = plugin.DataView.FirstOrDefault(v => v.Type == typeStr)?.DynamicType;
                    Type ditype = typeof(DataItems<ScriptDataItem>).GetGenericTypeX(node.Type as Type);
                    node.Items = ditype.CreateInstance(plugin.SaveDbPath, true, null, null) as IDataItems;
                    if (jo.TryGetValue("Items", out JToken itemsObj))
                    {
                        foreach (var io in itemsObj as JArray)
                        {
                            var iiii = io.ToObject(node.Type as Type);
                            if (iiii != null)
                                node.Items.Add(iiii);
                        }
                    }
                }
                tree.Add(node);

                JsonToTreeNodes(plugin, node.TreeNodes, jo.Property("TreeNodes").Value as JArray);
            }
        }

        private T TryGetJObjectProperty<T>(JObject jo, string propertyName, T defaultValue)
        {
            if (jo.TryGetValue(propertyName, out JToken value))
            {
                return value.ToObject<T>();
            }
            return defaultValue;
        }

        private object TryGetJObjectProperty(JObject jo, string propertyName, Type type, object defaultValue)
        {
            if (jo.TryGetValue(propertyName, out JToken value))
            {
                return value.ToObject(type);
            }
            return defaultValue;
        }
        #endregion
    }
}
