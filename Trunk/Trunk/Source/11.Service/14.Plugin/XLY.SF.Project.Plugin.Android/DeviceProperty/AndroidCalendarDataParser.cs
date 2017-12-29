/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 16:29:10 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using System.IO;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidCalendarDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidCalendarDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{38921FD4-77CD-42C3-859E-E4EFD1672680}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Calendar);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 9;

            pluginInfo.AppName = "com.android.providers.calendar";
            pluginInfo.Icon = "\\icons\\Calendar.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidCalendar);
            pluginInfo.SourcePath = new SourceFileItems();
            //安卓内置
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.calendar/databases/#F");
            //酷派
            pluginInfo.SourcePath.AddItem("/data/data/com.yulong.android.calendar/databases/#F");
            //VIVO
            pluginInfo.SourcePath.AddItem("/data/data/calendar/calendar.json");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(Calendar);
                ds.Items = new DataItems<Calendar>(pi.SaveDbPath);

                ds.Items.AddRange(GetCalendars(Path.Combine(pi.SourcePath[0].Local, "Calendar.db")));
                ds.Items.AddRange(GetCalendars(Path.Combine(pi.SourcePath[1].Local, "Calendar.db")));
                ds.Items.AddRange(GetVivoCalendars(pi.SourcePath[2].Local));
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备日历信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private List<Calendar> GetCalendars(string CalendarDbFile)
        {
            List<Calendar> list = new List<Calendar>();

            if (!FileHelper.IsValid(CalendarDbFile))
            {
                return list;
            }

            string newfile = SqliteRecoveryHelper.DataRecovery(CalendarDbFile, "", "Events");
            using (var dataContext = new SqliteContext(newfile))
            {
                var tempEvents = dataContext.FindByName("Events");
                tempEvents.ForEach(s =>
                {
                    try
                    {
                        var calendar = new Calendar();

                        calendar.DataState = DynamicConvert.ToEnumByValue(s.XLY_DataType, EnumDataState.Normal);
                        calendar.Title = DynamicConvert.ToSafeString(s.title);
                        calendar.EventLocation = DynamicConvert.ToSafeString(s.eventLocation);
                        calendar.Description = DynamicConvert.ToSafeString(s.description);
                        calendar.DtStart = DynamicConvert.ToSafeDateTime(s.dtstart);
                        calendar.DtEnd = DynamicConvert.ToSafeDateTime(s.dtend);
                        calendar.Duration = DynamicConvert.ToSafeString(s.duration);

                        list.Add(calendar);
                    }
                    catch (Exception ex)
                    {
                        Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取日历信息出错！", ex);
                    }
                });
            }

            return list;
        }

        private List<Calendar> GetVivoCalendars(string jsonPath)
        {
            List<Calendar> list = new List<Calendar>();

            if (!FileHelper.IsValid(jsonPath))
            {
                return list;
            }

            var lines = GetVCalendarParse(jsonPath);
            var calendar = new Calendar();
            try
            {
                foreach (Dictionary<String, object> content in lines)
                {
                    calendar = new Calendar();
                    if (content.Keys.Contains("SUMMARY"))
                    {
                        calendar.Title = DynamicConvert.ToSafeString(content["SUMMARY"]);
                    }

                    if (content.Keys.Contains("DTSTART"))
                    {
                        calendar.DtStart = DateTime.Parse(content["DTSTART"].ToSafeString());
                    }

                    if (content.Keys.Contains("DTEND"))
                    {
                        calendar.DtEnd = DateTime.Parse(content["DTEND"].ToSafeString());
                    }

                    if (content.Keys.Contains("DURATION"))
                    {
                        calendar.Duration = content["DURATION"].ToSafeString();
                    }

                    if (content.Keys.Contains("DESCRIPTION"))
                    {
                        calendar.Description = DynamicConvert.ToSafeString(content["DESCRIPTION"]);
                    }

                    list.Add(calendar);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取日历信息出错！", ex);
            }

            return list;
        }

        private List<dynamic> GetVCalendarParse(String path)
        {
            String[] content;
            if (FileHelper.IsValid(path))
                content = File.ReadAllLines(path);
            else
                return new List<dynamic>();

            List<dynamic> list = new List<dynamic>();
            Dictionary<String, object> model = new Dictionary<string, object>();

            foreach (var temp in content)
            {
                if (temp == "BEGIN:VEVENT")
                    model = new Dictionary<string, object>();
                if (temp == "END:VEVENT")
                    list.Add(model);
                if (temp.StartsWith("SUMMARY;"))
                {
                    model.Add("SUMMARY", DecodeDP(Regex.Match(temp, @"(=[0-9A-F]{2})+").Value));
                }
                if (temp.StartsWith("DTSTAMP:"))
                {
                    model.Add("DTSTAMP", GetDateTime(temp.Replace("DTSTAMP:", "")));
                }
                if (temp.StartsWith("DTSTART;"))
                {
                    model.Add("DTSTART", GetDateTime(temp.Replace("DTSTART;TZID=", "")));
                }
                if (temp.StartsWith("DTEND;"))
                {
                    model.Add("DTEND", GetDateTime(temp.Replace("DTEND;TZID=", "")));
                }
                if (temp.StartsWith("DURATION:"))
                {
                    model.Add("DURATION", GetDuration(temp.Replace("DURATION:", "")));
                }
                if (temp.StartsWith("X-BBK-TIMEZONE:"))
                {
                    model.Add("X-BBK-TIMEZONE", temp.Replace("X-BBK-TIMEZONE:", ""));
                }
                if (temp.StartsWith("DESCRIPTION;"))
                {
                    model.Add("DESCRIPTION", DecodeDP(Regex.Match(temp, @"(=[0-9A-F]{2})+").Value));
                }
            }
            return list;
        }

        //Quoted-Printable 解码  
        public string DecodeDP(string _ToDecode)
        {
            char[] chars = _ToDecode.ToCharArray();
            byte[] bytes = new byte[chars.Length];
            int bytesCount = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '=')
                {
                    bytes[bytesCount++] = Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));
                    i += 2;
                }
                else
                {
                    bytes[bytesCount++] = Convert.ToByte(chars[i]);
                }
            }

            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytesCount);
        }

        private DateTime GetDateTime(String strDT)
        {
            string tmp = "";
            if (strDT.Split(':').Length > 1)
            {
                tmp = strDT.Split(':')[1].ToString();
            }
            else
            {
                tmp = strDT;
            }
            String time = tmp.Substring(0, 4) + "-" + tmp.Substring(4, 2) + "-" + tmp.Substring(6, 2) + " " + tmp.Substring(9, 2) + ":" + tmp.Substring(11, 2) + ":" + tmp.Substring(13, 2);
            DateTime dt = DateTime.Parse(time);
            return dt;
        }

        private Int32 GetDuration(String strDuration)
        {
            if (strDuration.StartsWith("P"))
            {
                Int32 duration = 0;
                String p = strDuration.Replace("P", "");
                String date = "";
                String time = "";
                if (p.Split('T').Length == 2)
                {
                    date = p.Split('T')[0].ToString();
                    time = p.Split('T')[1].ToString();
                }
                else if (p.Contains('Y') || (p.Contains('D')))
                {
                    date = p;
                }
                else
                {
                    time = p;
                }

                if (date != "")
                {
                    if (date.Split('Y').Length >= 2)
                    {
                        String year = date.Split('Y')[0].ToString();
                        duration += Int32.Parse(year) * 365 * 24 * 3600;
                        date = date.Split('Y')[1].ToString();
                    }
                    if (date.Split('M').Length >= 2)
                    {
                        String month = date.Split('M')[0].ToString();
                        duration += Int32.Parse(month) * 30 * 24 * 3600;
                        if (date.Split('M').Length == 2)
                            date = date.Split('M')[1].ToString();
                        else
                            date = date.Split('M')[1].ToString() + date.Split('M')[2].ToString();
                    }
                    if (date.Split('W').Length >= 2)
                    {
                        String week = date.Split('W')[0].ToString();
                        duration += Int32.Parse(week) * 7 * 24 * 3600;
                        date = date.Split('W')[1].ToString();
                    }
                    if (date.Split('D').Length >= 2)
                    {
                        String day = date.Split('D')[0].ToString();
                        duration += Int32.Parse(day) * 24 * 3600;
                        date = date.Split('D')[1].ToString();
                    }
                }
                if (time != "")
                {
                    if (time.Split('H').Length >= 2)
                    {
                        String hour = time.Split('H')[0].ToString();
                        duration += Int32.Parse(hour) * 3600;
                        time = time.Split('H')[1].ToString();
                    }
                    if (time.Split('M').Length >= 2)
                    {
                        String min = time.Split('M')[0].ToString();
                        duration += Int32.Parse(min) * 60;
                        time = time.Split('M')[1].ToString();
                    }
                    if (time.Split('S').Length >= 2)
                    {
                        String sec = time.Split('S')[0].ToString();
                        duration += Int32.Parse(sec);
                    }
                }
                return duration;
            }
            else
            {
                return 0;
            }
        }

    }
}
