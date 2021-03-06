﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.BaseUtility;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 数据接收器.
    /// </summary>
    internal class LSFileReceiver : AbstractOutputReceiver
    {
        /// <summary>
        /// 文件列表结果
        /// </summary>
        public List<LSFile> Files;

        /// <summary>
        /// 源路径（指令路径）
        /// </summary>
        public string Source = string.Empty;

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public override void DoResolver()
        {
            if (Source == null) return;
            var files = ProcessLines(Lines, Source);
            if (files.Count == 1)
            {
                var f = files.First();
                //if folder
                var name = FileHelper.GetLinuxFileName(Source);
                if (!f.IsFolder && f.Name == name)
                {
                    f.Path = FileHelper.GetLinuxFilePath(Source);
                    f.IsRootFile = true;
                }
            }
            Files = files;
        }

        /****************** private methods ******************/

        private List<LSFile> ProcessLines(List<string> lines, string path)
        {
            List<LSFile> files = new List<LSFile>();
            if (!lines.IsValid() || path == null) return files;
            foreach (var line in lines)
            {
                var f = ProcessLine(line);
                if (f == null) continue;
                f.Path = path;
                files.Add(f);
            }
            return files;
        }

        public const String LS_PATTERN_EX = @"^([bcdlsp-][-r][-w][-xsS][-r][-w][-xsS][-r][-w][-xstST])\s+(?:\d{0,})?\s*(\S+)\s+(\S+)\s+(?:\d{1,},\s+)?(\d{1,}|\s)\s+(\w{3}|\d{4})[\s-](?:\s?(\d{1,2})\s?)[\s-]\s?(?:(\d{2}|\d{4}|\s)\s*)?(\d{2}:\d{2}|\s)\s*(.*?)([/@=*\|]?)$";

        private LSFile ProcessLine(string line)
        {
            if (!line.IsValid()) return null;
            var m = Regex.Match(line.TrimStart("OKAY").Trim(),LS_PATTERN_EX, RegexOptions.Compiled);
            if (!m.Success) return null;
            LSFile file = new LSFile();
            file.Name = m.Groups[9].Value;
            file.Permission = m.Groups[1].Value;
            var sized = m.Groups[4].Value.Trim();
            file.Size = sized.ToSafeInt();
            //folder
            file.IsFolder = false;
            switch (file.Permission[0])
            {
                case 'b':
                    file.Type = "Block";
                    break;
                case 'c':
                    file.Type = "Character";
                    break;
                case 'd':
                    /* 遇到一个recovery模式的手机，文件夹也有大小，切大小为4096，此处临时修改为此方案，以后根据测试情况在看 */
                    file.Type = "Directory"; if (!sized.IsValid() || file.Size == 4096) file.IsFolder = true;
                    break;
                case 'l':
                    file.Type = "Link";
                    ProcessLink(file.Name, file);
                    break;
                case 's':
                    file.Type = "Socket";
                    break;
                case 'p':
                    file.Type = "FIFO";
                    break;
                case '-':
                default:
                    file.Type = "File";
                    break;
            }
            //datetime
            String date1 = m.Groups[5].Value.Trim();
            String date2 = m.Groups[6].Value.Trim();
            String date3 = m.Groups[7].Value.Trim();
            String time = m.Groups[8].Value.Trim();
            string datestr = String.Format("{0}-{1}-{2} {3}", date1, date2.PadLeft(2, '0'), date3, time);
            try
            {
                var date = DateTime.ParseExact(datestr, "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture);
                file.CreateDate = date;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex,string.Format("can not convert '{0}' to datetime.", datestr));
            }

            //link file
            //return
            return file;
        }

        private void ProcessLink(string name, LSFile file)
        {
            String[] segments = name.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
            // we should have 2 segments
            if (segments.Length == 2)
            {
                file.LinkPath = segments[1].Trim();
            }
        }
    }
}
