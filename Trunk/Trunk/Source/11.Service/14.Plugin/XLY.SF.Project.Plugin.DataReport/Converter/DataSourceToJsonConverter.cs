using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataReport.DataSourceConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/28 11:38:33
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataReport
{
    /// <summary>
    /// 将DataSource转换为特定的数据格式
    /// </summary>
    public class DataSourceToJsonConverter
    {
        /// <summary>
        /// 当前任务路径
        /// </summary>
        public string CurrentTaskPath { get; set; }
        /// <summary>
        /// 导出路径
        /// </summary>
        public string TargetDirectory { get; set; }
        /// <summary>
        /// 将DataSource转换为Json格式的文件保存
        /// </summary>
        /// <param name="dataPool"></param>
        /// <param name="destPath">目标文件夹，比如C:\data\</param>
        public void ConverterToJsonFile(DataReportPluginArgument arg, string destPath)
        {
            CurrentTaskPath = arg.CurrentPath;
            TargetDirectory = arg.ReportPath;
            CreateDeviceInfo(arg.DeviceInfo, destPath);
            CreateCollectionInfo(arg.CollectionInfo, destPath);
            CreateJson(arg.DataPool, destPath);
        }

        private void CreateJson(IList<IDataSource> dataPool, string destPath)
        {
            string treePath = Path.Combine(destPath, @"tree.js");

            List<JsonExportTree> tree = new List<JsonExportTree>();
            var groups = dataPool.GroupBy(p => p.PluginInfo.Group);

            foreach (var item in groups)
            {
                JsonExportTree t = new JsonExportTree() { text = item.Key, location = "", icon = "",nodes=new List<JsonExportTree>() };
                foreach (var ds in item)
                {
                    if (ds == null)
                    {
                        continue;
                    }

                    JsonExportTree t2 = new JsonExportTree() { text = ds.PluginInfo.Name, location = "", icon = ds.PluginInfo.Icon ?? "", tags = new string[] { ds.Total.ToString() } };

                    if (ds is TreeDataSource td)
                    {
                        CreateTreeNodeJson(td.TreeNodes, destPath, t2);
                    }
                    else if (ds is AbstractDataSource sd)
                    {
                        CreateItemJson(sd.Items, destPath, t2);
                    }
                    t.nodes.Add(t2);
                }
                tree.Add(t);
            }
            //foreach (IDataSource ds in dataPool)
            //{
            //    if (ds == null)
            //    {
            //        continue;
            //    }

            //    JsonExportTree t = new JsonExportTree() { text = ds.PluginInfo.Name, location = "", icon = ds.PluginInfo.Icon ?? "", tags = new string[] { ds.Total.ToString() } };

            //    if (ds is TreeDataSource td)
            //    {
            //        CreateTreeNodeJson(td.TreeNodes, destPath, t);
            //    }
            //    else if (ds is SimpleDataSource sd)
            //    {
            //        CreateItemJson(sd.Items, destPath, t);
            //    }
            //    tree.Add(t);
            //}
            System.IO.File.WriteAllText(treePath, $"var __data = {Serializer.JsonFastSerilize(tree)};");
        }

        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="destPath"></param>
        private void CreateDeviceInfo(ExportDeviceInfo deviceInfo, string destPath)
        {
            string path = Path.Combine(destPath, @"device.js");
            System.IO.File.WriteAllText(path, $"var __device = {Serializer.JsonFastSerilize(deviceInfo ?? new object())};");
        }

        /// <summary>
        /// 保存采集信息
        /// </summary>
        /// <param name="collectionInfo"></param>
        /// <param name="destPath"></param>
        private void CreateCollectionInfo(ExportCollectionInfo collectionInfo, string destPath)
        {
            string path = Path.Combine(destPath, @"collect.js");
            System.IO.File.WriteAllText(path, $"var __collect = {Serializer.JsonFastSerilize(collectionInfo ?? new object())};");
        }

        private void CreateTreeNodeJson(List<TreeNode> nodes, string path, JsonExportTree t)
        {
            if (nodes == null)
            {
                return;
            }
            if (nodes.Count > 0 && t.nodes == null)
            {
                t.nodes = new List<JsonExportTree>();
            }
            foreach (TreeNode n in nodes)
            {
                if (n.Text== "魅影(1752880324)")
                {


                    var aa = "";
                }
                JsonExportTree t0 = (new JsonExportTree() { text = n.Text, location = "", icon = t.icon, tags = new string[] { n.Total.ToString() } });
                CreateItemJson(n.Items, path, t0, (Type)n.Type);
                CreateTreeNodeJson(n.TreeNodes, path, t0);
                t.nodes.Add(t0);
            }
        }

        private void CreateItemJson(IDataItems items, string dir, JsonExportTree t, Type itemType = null)
        {
            if (items == null)
            {
                return;
            }
            t.location = items.Key;
            string path = Path.Combine(dir, $"{items.Key}.js"); ;       // 文件：\data\3bd9a209-cdaf-42ab-b232-1aa4636f5a17.js
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                #region 生成数据json
                sw.Write("var __data = [");
                int r = 0;
                //items.Filter();
                foreach (var c in items.View)
                {
                            foreach (var columnVal in DisplayAttributeHelper.FindDisplayAttributes(itemType))
                            {
                                string val = string.Empty;
                                var value = columnVal.GetValue(c);

                                if (value is IEnumerable<string>)
                                {
                                    var array = (value as IEnumerable<string>).ToArray();
                                    val = array.Count() == 0 ? string.Empty : array[0];
                                }
                                else
                                {
                                    val = value == null ? string.Empty : value.ToString();
                                }
                                if (val.Contains(CurrentTaskPath) || val.Contains("\\"))
                                {
                                    CoypFile(val);
                                }
                            }
                        if (r != 0)
                        sw.Write(",");
                    sw.Write(Serializer.JsonSerilize(c));
                    r++;
                }
                sw.Write("];");
                #endregion

                #region 生成列属性json
                if (itemType == null)            //如果没有传入类型，则根据泛型参数类型来获取
                {
                    if (items.GetType().IsGenericType)
                    {
                        itemType = items.GetType().GetGenericArguments()[0];
                    }
                    else
                    {
                        throw new Exception("暂时先不处理的类型问题");
                    }
                }
                sw.Write("var __columns = ");
                List<JsonExportColumn> cols = new List<JsonExportColumn>();
                foreach (var c in DisplayAttributeHelper.FindDisplayAttributes(itemType))
                {
                    if (c.Visibility != EnumDisplayVisibility.ShowInDatabase)
                        cols.Add(new JsonExportColumn() { field = c.PropertyName, title = c.Text });
                  
                }
                sw.Write(Serializer.JsonFastSerilize(cols));
                sw.Write(";");
                #endregion
            }
        }
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="typeDirectory">分类目录</param>
        private void CoypFile(string sourceFile)
        {
            try
            {
                if (!File.Exists(sourceFile))
                {
                    return;
                }
                string fileName = Path.GetFileName(sourceFile);

                string file = sourceFile.Replace(CurrentTaskPath, "");
                if (file.StartsWith("\\"))
                {
                    file = file.Substring(1, file.Length - 1);
                }
                int index = -1;
                if (file.Contains("Source"))
                {
                    index = sourceFile.IndexOf("Source");
                }
                if (file.Contains("mtp"))
                {
                    index = sourceFile.IndexOf("mtp");
                }
                if (-1 == index) return ;
                string typeDirectory = sourceFile.Substring(index, sourceFile.Length - index - fileName.Length);
                string fileDirectory = Path.Combine(TargetDirectory, typeDirectory);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                string filePath = Path.Combine(fileDirectory, fileName);
                // 去除文件只读属性
                var fileInfo = new FileInfo(sourceFile);
                fileInfo.Attributes &= ~FileAttributes.ReadOnly;

                File.Copy(sourceFile, filePath);
                 

            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
