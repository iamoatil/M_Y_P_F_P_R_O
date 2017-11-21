using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：MyDriverInfo  
* Author     ：litao
* Create Date：2017/11/20 15:14:22
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    class MyDriverInfo
    {
        /// <summary>
        /// 获取目标磁盘剩余空间
        /// </summary>
        /// <returns></returns>
        public long GetTargetDriverFreeSpace(string dir)
        {
            string driveDirectoryName = Path.GetPathRoot(dir);
            DriveInfo drive = new DriveInfo(driveDirectoryName);
            return drive.TotalFreeSpace;
        }


        /// <summary>
        /// 获取一个可用于存放镜像文件的目录
        /// </summary>
        /// <returns></returns>
        public string GetProperTargetDir()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();            
            DriveInfo largestItem = drives[0];
            foreach (DriveInfo drive in drives)
            {
                if(largestItem != drive)
                {
                    if(drive.TotalFreeSpace > largestItem.TotalFreeSpace)
                    {
                        largestItem = drive;
                    }
                }
            }
            return largestItem.Name + @"XLYMirror\";
        }
    }
}
