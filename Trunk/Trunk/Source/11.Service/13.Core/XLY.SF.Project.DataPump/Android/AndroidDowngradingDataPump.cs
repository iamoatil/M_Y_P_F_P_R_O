using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using zlibNET;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 用于降级提取的数据泵。
    /// </summary>
    public class AndroidDowngradingDataPump : InitAtExecutionDataPump
    {
        #region Fields

        private readonly static String AdbPath;

        private readonly static String ApkBasePath;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.AndroidDowngradingDataPump。
        /// </summary>
        static AndroidDowngradingDataPump()
        {
            AdbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "adb", "adb.exe");
            ApkBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downgrading", "apks");
        }

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.AndroidDowngradingDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public AndroidDowngradingDataPump(Pump metadata)
            :base(metadata)
        {

        }

        #endregion

        #region Methods

        #region Protected

        protected override Boolean InitAtFirstTime(DataPumpExecutionContext context)
        {
            IEnumerable<String> files = DowngradingBackup(context);
            ResolveBackupFiles(files, context.TargetDirectory);
            return true;
        }

        protected override void OverrideExecute(DataPumpExecutionContext context)
        {
        }

        #endregion

        #region Private

        /// <summary>
        /// 降级备份。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>备份文件列表。</returns>
        private ICollection<String> DowngradingBackup(DataPumpExecutionContext context)
        {
            IntPtr handle = IntPtr.Zero;
            Device device = context.PumpDescriptor.Source as Device;
            String savePath = context.TargetDirectory;

            String backupDataBasePath = Path.Combine(savePath, Guid.NewGuid().ToString());
            if (!Directory.Exists(backupDataBasePath)) Directory.CreateDirectory(backupDataBasePath);
            String backupAppBasePath = Path.Combine(savePath, Guid.NewGuid().ToString());
            if (!Directory.Exists(backupAppBasePath)) Directory.CreateDirectory(backupAppBasePath);

            ICollection<String> files = new Collection<String>();
            String backupAppFile;
            String backupDataFile;
            foreach (ExtractItem item in context.ExtractionItems)
            {
                if (context.IsCancellationRequested) break;
                handle = Mount(device.ID, item.AppName);
                if (handle == IntPtr.Zero)
                {
                    continue;
                }
                backupAppFile = Path.Combine(backupAppBasePath, $"{item.AppName}.apk");
                if (BackupApp(handle, backupAppFile))
                {
                    if (InstallApp(handle, Path.Combine(ApkBasePath, item.AppName)))
                    {
                        backupDataFile = Path.Combine(backupDataBasePath, $"{item.AppName}.ab");
                        if (BackupData(handle, backupDataFile))
                        {
                            files.Add(backupDataFile);
                        }
                        if (InstallApp(handle, backupAppFile))
                        {
                        }
                    }
                }
                Unmount(handle);
            }
            return files;
        }

        /// <summary>
        /// 解析备份文件。
        /// </summary>
        /// <param name="files">备份文件列表。</param>
        private void ResolveBackupFiles(IEnumerable<String> files, String savePath)
        {
            String appName;
            String backupDataBasePath;
            foreach (String file in files)
            {
                if (!CheckAbFile(file))
                {
                    continue;
                }
                appName = Path.GetFileNameWithoutExtension(file);
                backupDataBasePath = Path.GetDirectoryName(file);
                using (FileStream fsource = new FileStream(file, FileMode.Open))
                {
                    using (FileStream ftarget = new FileStream(Path.Combine(backupDataBasePath, $"{appName}.zip"), FileMode.Create))
                    {
                        using (zlibNET.ZOutputStream zs = new ZOutputStream(ftarget))
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

                //解压文件并拷贝到保存的目录下
                //Unzip(app.AppName, zipFilePathName, SavePath);
                Unzip(savePath, backupDataBasePath, appName);
                //分应用文件处理
                AppFileHandle(savePath, appName);
            }
        }

        /// <summary>
        /// 检查ab文件
        /// </summary>
        private Boolean CheckAbFile(String filePath)
        {
            byte[] bts = File.ReadAllBytes(filePath);
            if (bts.Length < 7)
            {
                return false;
            }
            //读取文件头的两个字节
            var head1 = bts[0];
            var head2 = bts[1];
            var head3 = bts[2];
            if (head1 == 0x41 && head2 == 0x4E && head3 == 0x44)//ab文件
            {
                return true;
            }

            return false;
        }

        private Boolean Unzip(String savePath,String backupDataBasePath, String rarName)
        {
            rarName = string.Format("{0}.zip", rarName);
            return WinRARCSharp.UnRAR(savePath, backupDataBasePath, rarName);
        }

        private void AppFileHandle(String savePath,string appName)
        {
            string appPath = Path.Combine(savePath, "apps", appName);

            //微信：需要将com.tencent.mm\r\MicroMsg目录拷贝到com.tencent.mm目录下。
            if (appName.Contains("com.tencent.mm"))
            {
                //com.tencent.mm\r\MicroMsg目录拷贝到com.tencent.mm目录
                WeixinHandler(appPath);
                return;
            }
            else
            {
                //大部分App：需要将db文件夹改名为databases，将sp改名为shared_prefs。
                Handler(appPath);
                return;
            }
        }

        #region 处理各App

        /// <summary>
        /// 处理大部分App：需要将db文件夹改名为Databases，将sp改名为shared_prefs。
        /// </summary>
        private void Handler(string appPath)
        {

            string db = Path.Combine(appPath, "db");
            string dataBases = Path.Combine(appPath, "databases");

            //databases文件夹不存在就创建
            if (!Directory.Exists(dataBases))
            {
                Directory.CreateDirectory(dataBases);
                //把db文件夹的文件拷贝到Databases
                CopyDirectory(db, dataBases);
                //删除db文件夹
                DirectoryInfo dir = new DirectoryInfo(db);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }
            }
            //shared_prefs文件夹不存在就创建
            string shared_prefs = Path.Combine(appPath, "shared_prefs");
            string sp = Path.Combine(appPath, "sp");
            if (!Directory.Exists(shared_prefs))
            {
                Directory.CreateDirectory(shared_prefs);
                //把sp文件夹的文件拷贝到shared_prefs
                CopyDirectory(sp, shared_prefs);
                //删除db文件夹
                DirectoryInfo dir = new DirectoryInfo(sp);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }
            }
        }

        /// <summary>
        /// 处理微信：需要将com.tencent.mm\r\MicroMsg目录拷贝到com.tencent.mm目录下。
        /// </summary>
        private void WeixinHandler(string appPath)
        {
            string microMsg = Path.Combine(appPath, @"r\MicroMsg");
            string newMicroMsg = Path.Combine(appPath, "MicroMsg");
            //Databases文件夹不存在就创建
            if (!Directory.Exists(newMicroMsg))
            {
                Directory.CreateDirectory(newMicroMsg);
                CopyDirectory(microMsg, newMicroMsg);
                DirectoryInfo dir = new DirectoryInfo(microMsg);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }
            }
        }

        /// <summary>
        /// 处理Twitter：需要把db改名为databases
        /// </summary>
        /// <param name="appPath"></param>
        private void TwitterHandler(string appPath)
        {
            string db = Path.Combine(appPath, "db");
            string dataBases = Path.Combine(appPath, "databases");

            //databases文件夹不存在就创建
            if (!Directory.Exists(dataBases))
            {
                Directory.CreateDirectory(dataBases);
                //把db文件夹的文件拷贝到Databases
                CopyDirectory(db, dataBases);
                //删除db文件夹
                DirectoryInfo dir = new DirectoryInfo(db);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }
            }
        }

        /// <summary>
        /// 拷贝目录
        /// </summary>
        /// <param name="sourceDirectory">资源目录</param>
        /// <param name="targetDirectory">目标目录</param>
        private void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory) || !Directory.Exists(targetDirectory))
            {
                return;
            }
            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirectory);
            FileInfo[] fileInfo = sourceInfo.GetFiles();
            foreach (FileInfo fiTemp in fileInfo)
            {
                // 去除文件只读属性
                fiTemp.Attributes &= ~FileAttributes.ReadOnly;
                if (File.Exists(string.Format(@"{0}\{1}", targetDirectory, fiTemp.Name)))
                {
                    continue;
                }
                File.Copy(sourceDirectory + "\\" + fiTemp.Name, targetDirectory + "\\" + fiTemp.Name, true);
            }
            DirectoryInfo[] diInfo = sourceInfo.GetDirectories();
            foreach (DirectoryInfo diTemp in diInfo)
            {
                string sourcePath = diTemp.FullName;
                string targetPath = diTemp.FullName.Replace(sourceDirectory, targetDirectory);
                Directory.CreateDirectory(targetPath);
                CopyDirectory(sourcePath, targetPath);
            }
        }

        #endregion

        #region API

        /// <summary>
        /// 加载手机。
        /// </summary>
        /// <param name="deviceId">设备Id。</param>
        /// <param name="appName">app应用名称。</param>
        /// <returns>设备句柄。</returns>
        private IntPtr Mount(String deviceId, String appName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 备份原App。
        /// </summary>
        /// <param name="handle">设备句柄。</param>
        /// <param name="savePath">保存路径。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        private Boolean BackupApp(IntPtr handle, String savePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 安装App。
        /// </summary>
        /// <param name="handle">设备句柄。</param>
        /// <param name="appPath">App路径。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        private Boolean InstallApp(IntPtr handle, String appPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 备份数据。
        /// </summary>
        /// <param name="handle">设备句柄。</param>
        /// <param name="backupPath">备份到的路径。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        private Boolean BackupData(IntPtr handle, String backupPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 卸载设备。
        /// </summary>
        /// <param name="handle">设备句柄。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        private Boolean Unmount(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #endregion
    }
}
