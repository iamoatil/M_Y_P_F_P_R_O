/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 16:29:10 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidSwitchTimeDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidSwitchTimeDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{E16A6BC3-AB35-4053-AC68-3A879E9CAE93}";
            pluginInfo.Name = "开关机时间";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 8;

            pluginInfo.AppName = "com.android.switchtime";
            pluginInfo.Icon = "\\icons\\switchtime.png";
            pluginInfo.Description = "提取安卓设备开关机时间信息";
            pluginInfo.SourcePath = new SourceFileItems();
            //HUAWEI
            pluginInfo.SourcePath.AddItem("/data/system/users/0.xml");
            pluginInfo.SourcePath.AddItem("/data/log/mmi/#F");
            //samsung
            pluginInfo.SourcePath.AddItem("/data/system/SimCard.dat");
            pluginInfo.SourcePath.AddItem("/data/log/poweroff_info.txt");
            //OPPO
            pluginInfo.SourcePath.AddItem("/data/data/com.oppo.market/shared_prefs/com.oppo.market_preferences.xml");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(SwitchTimeInfo);
                ds.Items = new DataItems<SwitchTimeInfo>(pi.SaveDbPath);

                ds.Items.AddRange(GetHuawei(pi.SourcePath[0].Local, pi.SourcePath[1].Local));
                ds.Items.AddRange(GetSamsung(pi.SourcePath[2].Local, pi.SourcePath[3].Local));
                ds.Items.AddRange(GetOppo(pi.SourcePath[4].Local));
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备开关机时间数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private List<SwitchTimeInfo> GetHuawei(string xmlFile, string mmiPath)
        {
            List<SwitchTimeInfo> list = new List<SwitchTimeInfo>();

            // 最近一次的开机时间存放文件：data\system\users\0.xml
            if (FileHelper.IsValid(xmlFile))
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(xmlFile);
                    if (null != ds && ds.Tables.Count > 0 && ds.Tables.Contains("user"))
                    {
                        String lastLoggedIn = ds.Tables["user"].Rows[0]["lastLoggedIn"].ToString();
                        var switchTimeInfo = new SwitchTimeInfo();
                        switchTimeInfo.Type = EnumSwitchTimeType.Boot;
                        switchTimeInfo.SwitchTimeInfoDate = DynamicConvert.ToSafeDateTime(lastLoggedIn);

                        list.Add(switchTimeInfo);
                    }
                }
                catch (Exception ex)
                {
                    Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取开关机时间数据出错！", ex);
                }
            }

            // 每次开机时间文件存放路径：\data\log\mmi
            if (FileHelper.IsValidDictory(mmiPath))
            {
                DirectoryInfo dir = new DirectoryInfo(mmiPath);
                var fiList = dir.GetFiles();
                fiList.ForEach(f =>
                {
                    try
                    {
                        string fileName = f.Name;
                        string time = string.Empty;
                        int index = fileName.IndexOf("_normal_");
                        if (!string.IsNullOrEmpty(fileName) && index > 0)
                        {
                            time = fileName.Substring(0, index);
                            string yyyyMMdd = time.Substring(0, 10);
                            string hhmmss = time.Substring(11, 8);
                            hhmmss = hhmmss.Replace("-", ":");

                            var switchTimeInfo = new SwitchTimeInfo();
                            switchTimeInfo.Type = EnumSwitchTimeType.Boot;
                            switchTimeInfo.SwitchTimeInfoDate = (yyyyMMdd + " " + hhmmss).ToDateTime();

                            list.Add(switchTimeInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取开关机时间数据出错！", ex);
                    }
                });
            }

            return list;
        }

        private List<SwitchTimeInfo> GetSamsung(string dataFile, string txtFile)
        {
            List<SwitchTimeInfo> list = new List<SwitchTimeInfo>();

            // 获取最近一次的开机时间信息：data/system/SimCard.dat
            if (FileHelper.IsValid(dataFile))
            {
                try
                {
                    //开机时间
                    using (FileStream fs = new FileStream(dataFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            String content = sr.ReadToEnd();
                            string split = "SimChangeTime=";
                            int index = content.IndexOf(split);
                            if (string.IsNullOrEmpty(content) || index < 0 || content.Length < index + split.Length + 13)
                            {
                                //do nothing
                            }
                            else
                            {
                                string boot = content.Substring(index + split.Length, 13);

                                var switchTimeInfo = new SwitchTimeInfo();
                                switchTimeInfo.Type = EnumSwitchTimeType.SIMChange;
                                switchTimeInfo.SwitchTimeInfoDate = DynamicConvert.ToSafeDateTime(boot);

                                list.Add(switchTimeInfo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取开关机时间数据出错！", ex);
                }
            }

            // 获取每次开机时间信息：/data/log/poweroff_info.txt
            if (FileHelper.IsValid(txtFile))
            {
                try
                {
                    //开机时间
                    using (FileStream fs = new FileStream(txtFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            String content = sr.ReadToEnd();
                            if (!string.IsNullOrEmpty(content))
                            {
                                string split = "Batt Status";
                                int startIndex = 0;
                                int index = content.IndexOf(split);
                                while (index != -1 && index >= 19)
                                {
                                    string shutDown = content.Substring(startIndex, 19);
                                    index = content.IndexOf(split, index + split.Length);
                                    startIndex = index - 20;

                                    var switchTimeInfo = new SwitchTimeInfo();
                                    switchTimeInfo.Type = EnumSwitchTimeType.Shutdown;
                                    switchTimeInfo.SwitchTimeInfoDate = shutDown.ToDateTime();

                                    list.Add(switchTimeInfo);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取开关机时间数据出错！", ex);
                }
            }

            return list;
        }

        private List<SwitchTimeInfo> GetOppo(string xmlFile)
        {
            List<SwitchTimeInfo> list = new List<SwitchTimeInfo>();

            // 开机时间存放文件：data\data\com.oppo.market\shared_prefs\com.oppo.market_preferences.xml
            if (FileHelper.IsValid(xmlFile))
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(xmlFile);
                    if (null == ds || ds.Tables.Count <= 0 || !ds.Tables.Contains("long"))
                    {
                        return list;
                    }

                    var switchTime = from st in ds.Tables["long"].AsEnumerable()
                                         //where st.Field<string>("name").Equals("pref.start.phone.time")
                                     select new
                                     {
                                         name = st.Field<string>("name"),
                                         value = st.Field<string>("value"),
                                     };
                    switchTime.ForEach(s =>
                    {
                        //开机时间
                        if (!string.IsNullOrEmpty(s.name) && s.name == "pref.start.phone.time")
                        {
                            var switchTimeInfo = new SwitchTimeInfo();
                            switchTimeInfo.Type = EnumSwitchTimeType.Boot;
                            switchTimeInfo.SwitchTimeInfoDate = DynamicConvert.ToSafeDateTime(s.value);

                            list.Add(switchTimeInfo);
                        }
                        //关机时间：不确定是不是这个时间
                        //if (!string.IsNullOrEmpty(s.name) && s.name == "pref.start.market.time")
                        //{
                        //    //var switchTimeInfo = new SwitchTimeInfo();
                        //    //switchTimeInfo.Type = EnumSwitchTimeType.Shutdown;
                        //    //switchTimeInfo.SwitchTimeInfoDate = DynamicConvert.ToSafeDateTime(s.value);
                        //    //switchTimeInfos.Add(switchTimeInfo);
                        //}
                    });
                }
                catch (Exception ex)
                {
                    Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取开关机时间数据出错！", ex);
                }
            }

            return list;
        }

    }
}
