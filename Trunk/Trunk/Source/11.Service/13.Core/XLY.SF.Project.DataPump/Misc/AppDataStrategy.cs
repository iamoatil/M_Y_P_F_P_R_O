using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump
{
    public class AppDataStategy : IProcessControllableStrategy
    {
        private String _destPath;

        private String _sourcePath;

        private List<string> _findApps = new List<string>();

        #region Methods

        #region Public

        public void InitExecution(string sourcePath, string destPath)
        {
            _sourcePath = sourcePath;
            _destPath = destPath;
        }

        public void Process(DataPumpExecutionContext context)
        {
            var appName = context.ExtractionItems.First().AppName;

            if (!_findApps.Contains(appName))
            {
                FindAppData(appName, _sourcePath, _destPath);
                _findApps.Add(appName);
            }

            var si = context.Source;
            if (si.ItemType == Domains.SourceFileItemType.NormalPath)
            {
                string path = si.Config.TrimEnd("#F");
                if (path.StartsWith("/data/data/"))
                {
                    path = path.TrimStart("/data/data/");
                }
                else if (path.StartsWith("/data/"))
                {
                    path = path.TrimStart("/data/");
                }
                path = path.Replace("/", @"\");

                var local = Path.Combine(_destPath, "data", path.Replace('/', '\\').TrimStart('\\'));
                if (FileHelper.IsValidDictory(local) || FileHelper.IsValid(local))
                {
                    si.Local = local;
                }
            }
        }

        #region 查找APP

        private bool FindAppData(string app, string source, string dest)
        {
            bool res = false;
            try
            {
                if (!res)
                {
                    res = FindAppDataForVivo(app, source, dest);
                }

                if (!res)
                {
                    res = FindAppDataForOppo(app, source, dest);
                }

                if (!res)
                {
                    res = FindAppDataForXiaoMi(app, source, dest);
                }

                if (!res)
                {
                    res = FindAppDataForHuawei(app, source, dest);
                }

                if (app == "com.tencent.mm")
                {//查找安卓微信分身
                    if (FindAndroidWechat(source, dest))
                    {
                        res = true;
                    }
                }
            }
            catch
            {
                res = false;
            }
            return res;
        }

        #region VIVO备份

        private bool FindAppDataForVivo(string app, string source, string dest)
        {
            if (source.EndsWith(app))
            {
                AppDirMove(app, source, dest, true);
                return true;
            }

            var arr = Directory.GetDirectories(source, app, SearchOption.AllDirectories).
                Where((d) => d.EndsWith(app) && (!new DirectoryInfo(d).Name.StartsWith("com.") || Directory.GetDirectories(d).Any() || app == "com.wechatBackup")).ToArray();

            if (arr.IsValid())
            {//找到APP文件夹
                AppDirMove(app, arr[0], dest, true);
                return true;
            }
            else
            {//APP文件夹查找失败
                //尝试查找ZIP文件
                arr = Directory.GetFiles(source, String.Format("{0}.zip", app), SearchOption.AllDirectories);
                if (arr.IsValid())
                {
                    SevenZipHelper.ExtractArchive(arr[0], Path.Combine(dest, "temp"));

                    arr = Directory.GetDirectories(Path.Combine(dest, "temp"), app, SearchOption.AllDirectories).Where((d) => d.EndsWith(app)).ToArray();
                    if (arr.IsValid())
                    {//找到APP文件夹
                        AppDirMove(app, arr[0], dest);
                    }

                    FileHelper.DeleteDirectorySafe(Path.Combine(dest, "temp"));

                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Oppo备份

        private bool FindAppDataForOppo(string app, string source, string dest)
        {
            var arr = Directory.GetFiles(source, String.Format("{0}.tar", app), SearchOption.AllDirectories);
            if (arr.IsValid())
            {
                SevenZipHelper.ExtractArchive(arr[0], Path.Combine(dest, "temp"));

                arr = Directory.GetDirectories(Path.Combine(dest, "temp"), app, SearchOption.AllDirectories).Where((d) => d.EndsWith(app)).ToArray();
                if (arr.IsValid())
                {//找到APP文件夹
                    AppDirMove(app, arr[0], dest);
                }

                FileHelper.DeleteDirectorySafe(Path.Combine(dest, "temp"));

                return true;
            }

            return false;
        }

        #endregion

        #region 小米备份

        private List<string> ListXiaoMiBak = null;

        private bool FindAppDataForXiaoMi(string app, string source, string dest)
        {
            string filepath;

            if (null == ListXiaoMiBak)
            {
                ListXiaoMiBak = Directory.GetFiles(source, "*.bak", SearchOption.AllDirectories).ToList();
            }

            if (ListXiaoMiBak.IsInvalid())
            {
                return false;
            }

            if (app == "contacts")
            {
                filepath = ListXiaoMiBak.FirstOrDefault((bak) => new FileInfo(bak).Name.ToLower().Contains("通讯录"));
            }
            else if (app == "com.android.contacts")
            {
                filepath = ListXiaoMiBak.FirstOrDefault((bak) => new FileInfo(bak).Name.ToLower().Contains("通话记录"));
            }
            else if (app == "com.android.mms")
            {
                filepath = ListXiaoMiBak.FirstOrDefault((bak) => new FileInfo(bak).Name.ToLower().Contains("短信"));
            }
            else
            {
                filepath = ListXiaoMiBak.FirstOrDefault((bak) => new FileInfo(bak).Name.ToLower().Contains($"({app.ToLower()})"));
            }

            if (null == filepath)
            {
                return false;
            }

            return DealXiaoMiBakFile(app, filepath, dest);
        }

        private static readonly byte[] adbHead = new byte[] { 0x41, 0x4E, 0x44, 0x52, 0x4F, 0x49, 0x44, 0x20, 0x42, 0x41, 0x43, 0x4B, 0x55, 0x50 };
        private bool DealXiaoMiBakFile(string app, string sourcefile, string destpath)
        {
            bool res = false;
            string tempPath = Path.Combine(destpath, "localtemp");
            string tempfile = Path.Combine(tempPath, "temp.bak");
            string tempZipFile = Path.Combine(tempPath, "temp.zip");
            string tarPath = Path.Combine(tempPath, "apk.tar");

            if (!File.Exists(sourcefile))
            {
                return false;
            }

            Directory.CreateDirectory(tempPath);
            try
            {
                byte[] baksource = null;
                int tempLength = 2048;
                using (var fs = new FileStream(sourcefile, FileMode.Open))
                {
                    if (fs.Length < tempLength)
                    {
                        baksource = new byte[fs.Length];
                        fs.Read(baksource, 0, (int)fs.Length);
                    }
                    else
                    {
                        baksource = new byte[tempLength];
                        fs.Read(baksource, 0, tempLength);
                    }
                }

                if (baksource.IsValid() && (baksource.Count() > 2) && (baksource[0].ToString("x2") == "4d") && (baksource[1].ToString("x2") == "49"))  //小米3/红米 note2等处理流程
                {
                    int adbHeadPos = FindSubByteArray(baksource, adbHead);

                    if (adbHeadPos < 0)
                    {
                        return false;
                    }

                    using (var source = new FileStream(sourcefile, FileMode.Open))
                    {
                        using (FileStream fs = new FileStream(tempfile, FileMode.Create))
                        {
                            source.Position = adbHeadPos;
                            source.CopyTo(fs);
                        }
                    }

                    baksource = null;

                    try
                    {
                        ResolveBackupFile(tempfile, tempZipFile);
                    }
                    catch
                    {
                        ResolveBackupFileForXiaoMi5(tempfile, tempZipFile);
                    }

                    SevenZipHelper.ExtractArchive(tempZipFile, tempPath);
                }
                else //小米2处理流程
                {
                    SevenZipHelper.ExtractArchive(sourcefile, tempPath);

                    if (File.Exists(tarPath))
                    {
                        try
                        {
                            ResolveBackupFile(tarPath, tempZipFile);
                        }
                        catch
                        {
                            ResolveBackupFileForXiaoMi5(tarPath, tempZipFile);
                        }

                        SevenZipHelper.ExtractArchive(tempZipFile, tempPath);
                    }
                }

                var arr = Directory.GetDirectories(tempPath, app, SearchOption.AllDirectories).Where((d) => d.EndsWith(app)).ToArray();
                if (arr.IsValid())
                {
                    string appPath = arr[0];

                    DirReName(appPath, "r", "");
                    DirReName(appPath, "db", "databases");
                    DirReName(appPath, "f", "files");
                    DirReName(appPath, "sp", "shared_prefs");

                    AppDirMove(app, appPath, destpath);

                    res = true;
                }
                else
                {
                    if (app == "contacts")
                    {
                        if (Directory.GetDirectories(tempPath, "com.android.contacts", SearchOption.AllDirectories).Count() != 0)
                        {
                            AppDirMove(app, Directory.GetDirectories(tempPath, "com.android.contacts", SearchOption.AllDirectories).ToArray()[0], destpath);
                        }
                        else
                        {
                            AppDirMove(app, tempPath, destpath);
                        }
                        res = true;
                    }
                    else if ((app == "com.android.contacts") || (app == "com.android.mms"))
                    {
                        AppDirMove(app, tempPath, destpath);
                    }
                    else
                    {
                        res = false;
                    }
                }

                FileHelper.DeleteDirectorySafe(tempPath);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (Directory.Exists(tempPath))
                {
                    new DirectoryInfo(tempPath).Delete(true);
                }
            }
            return res;
        }

        /// <summary>
        /// 从原字节数组中查找子数组，找到则返回子数组起始位置，否则返回-1
        /// </summary>
        /// <param name="srcBytes"></param>
        /// <param name="searchBytes"></param>
        /// <returns></returns>
        private int FindSubByteArray(byte[] srcBytes, byte[] searchBytes)
        {
            if (srcBytes == null) { return -1; }
            if (searchBytes == null) { return -1; }
            if (srcBytes.Length == 0) { return -1; }
            if (searchBytes.Length == 0) { return -1; }
            if (srcBytes.Length < searchBytes.Length) { return -1; }
            for (int i = 0; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] == searchBytes[0])
                {
                    if (searchBytes.Length == 1) { return i; }
                    bool flag = true;
                    for (int j = 1; j < searchBytes.Length; j++)
                    {
                        if (srcBytes[i + j] != searchBytes[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag) { return i; }
                }
            }
            return -1;
        }

        /// <summary>
        /// 将安卓备份文件转换为普通压缩文件
        /// </summary>
        /// <param name="bakFile"></param>
        /// <param name="targetPath"></param>
        private void ResolveBackupFile(string bakFile, string targetPath)
        {
            using (FileStream fsource = new FileStream(bakFile, FileMode.Open))
            {
                using (FileStream ftarget = new FileStream(targetPath, FileMode.Create))
                {
                    using (var zs = new zlibNET.ZOutputStream(ftarget))
                    {
                        byte[] buff = new byte[4096];
                        int len = fsource.Read(buff, 0, buff.Length);
                        zs.Write(buff, 24, len - 24);       //去掉头24字节

                        while (true)
                        {
                            len = fsource.Read(buff, 0, buff.Length);
                            if (len <= 0)
                            {
                                break;
                            }
                            zs.Write(buff, 0, len);
                        }
                        zs.Flush();
                    }
                }
            }
        }

        private void ResolveBackupFileForXiaoMi5(string bakFile, string targetPath)
        {
            using (FileStream fsource = new FileStream(bakFile, FileMode.Open))
            {
                using (FileStream ftarget = new FileStream(targetPath, FileMode.Create))
                {
                    byte[] buff = new byte[4096];
                    int len = fsource.Read(buff, 0, buff.Length);
                    ftarget.Write(buff, 24, len - 24);       //去掉头24字节

                    while (true)
                    {
                        len = fsource.Read(buff, 0, buff.Length);
                        if (len <= 0)
                        {
                            break;
                        }
                        ftarget.Write(buff, 0, len);
                    }
                }
            }
        }


        #endregion

        #region 华为备份

        private static string[] _huaweiApp = { "calllog", "contact", "sms" };
        private bool FindAppDataForHuawei(string app, string source, string dest)
        {
            try
            {
                string dbFile = string.Format("{0}.db", app);

                var fls = Directory.GetFiles(source, dbFile, SearchOption.AllDirectories).Where(f => f.EndsWith(dbFile));
                if (fls.IsInvalid())
                {
                    return false;
                }

                if (_huaweiApp.Contains(app))
                {
                    var targetFilePath = new FileInfo(Path.Combine(dest, "data", app, dbFile));
                    if (!Directory.Exists(targetFilePath.DirectoryName))
                    {
                        Directory.CreateDirectory(targetFilePath.DirectoryName);
                    }

                    File.Copy(fls.Single(), targetFilePath.FullName, true);

                    return true;
                }

                using (var context = new SqliteContext(fls.FirstOrDefault()))
                {
                    //获取文件列表
                    var file_info = context.Find(new SQLiteString("SELECT file_index,file_path FROM apk_file_info ORDER BY file_index"));

                    foreach (var file in file_info)
                    {
                        try
                        {
                            string file_index = DynamicConvert.ToSafeString(file.file_index);
                            string file_path = DynamicConvert.ToSafeString(file.file_path);
                            if (file_index == "-1")
                            {//-1是文件夹,不管
                                continue;
                            }

                            var targetFilePath = new FileInfo(Path.Combine(dest, "data", file_path.Substring(file_path.IndexOf(app))));
                            if (!Directory.Exists(targetFilePath.DirectoryName))
                            {
                                Directory.CreateDirectory(targetFilePath.DirectoryName);
                            }

                            //恢复文件
                            using (Stream fs = new FileStream(targetFilePath.FullName, FileMode.Create))
                            {
                                context.UsingSafeConnection(new SQLiteString(string.Format("SELECT file_data FROM apk_file_data WHERE file_index = '{0}' ORDER BY data_index", file_index)), dr =>
                                {
                                    while (dr.Read())
                                    {
                                        var data = dr.ToDynamic();
                                        var da = (byte[])data.file_data;
                                        fs.Write(da, 0, da.Length);
                                    }
                                });
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 安卓微信分身

        private bool FindAndroidWechat(string source, string dest)
        {
            var lsfinds = new DirectoryInfo(source).GetDirectories("MicroMsg", SearchOption.AllDirectories).Where(d => d.Name == "MicroMsg" && d.Parent.Name != "com.tencent.mm");

            foreach (var cwh in lsfinds)
            {
                AppDirMove(cwh.Parent.Name, cwh.Parent.FullName, dest, true);
            }

            return lsfinds.Any();
        }

        #endregion

        private void DirReName(string basePath, string source, string dest)
        {
            string sourcePath = Path.Combine(basePath, source);
            if (Directory.Exists(sourcePath))
            {
                if ("" == dest)
                {
                    DirMove(sourcePath, basePath);
                }
                else
                {
                    Directory.Move(sourcePath, Path.Combine(basePath, dest));
                }
            }
        }

        private void DirMove(string source, string dest)
        {
            foreach (var file in new DirectoryInfo(source).GetFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    //创建目标路径
                    string destPath = dest.TrimEnd('/', '\\') + @"\" + file.DirectoryName.Substring(source.Length).TrimStart('/', '\\');
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    File.Move(file.FullName, Path.Combine(destPath, file.Name));
                }
                catch
                {
                }
            }
            try
            {
                new DirectoryInfo(source).Delete(true);
            }
            catch
            {
            }
        }

        private void AppDirMove(string app, string source, string dest, bool isCopy = false)
        {
            foreach (var file in new DirectoryInfo(source).GetFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    //创建目标路径
                    string destPath = dest.TrimEnd('/', '\\') + @"\data\" + app + @"\" + file.DirectoryName.Substring(source.Length).TrimStart('/', '\\');
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }

                    if (isCopy)
                    {
                        File.Copy(file.FullName, Path.Combine(destPath, file.Name), true);
                    }
                    else
                    {
                        File.Move(file.FullName, Path.Combine(destPath, file.Name));
                    }
                }
                catch
                {
                }
            }

            if (!isCopy)
            {
                FileHelper.DeleteDirectorySafe(source);
            }

            switch (app)
            {
                case "com.tencent.mobileqq":
                case "com.tencent.mm":
                    var dirs = new DirectoryInfo(source).GetDirectories().Select(di => di.Name).ToList();
                    if (dirs.Count == 5
                        && dirs.Contains("db")
                        && dirs.Contains("ef")
                        && dirs.Contains("f")
                        && dirs.Contains("r")
                        && dirs.Contains("sp"))
                    {
                        string destPath = dest.TrimEnd('/', '\\') + @"\data\" + app;

                        DirReName(destPath, "r", "");
                        DirReName(destPath, "db", "databases");
                        DirReName(destPath, "f", "files");
                        DirReName(destPath, "sp", "shared_prefs");
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
