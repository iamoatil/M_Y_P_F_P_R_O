/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 17:57:23 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件浏览服务工厂
    /// </summary>
    public static class FileBrowsingServiceFactory
    {
        /// <summary>
        /// 获取文件浏览服务
        /// </summary>
        /// <param name="source">数据源 可以是手机设备或者镜像文件</param>
        /// <returns></returns>
        public static AbsFileBrowsingService GetFileBrowsingService(object source)
        {
            if (null == source)
            {
                return null;
            }

            if (source is Device)
            {//手机
                var device = source as Device;

                if (device.OSType == EnumOSType.Android)
                {//安卓手机
                    return new AndroidDeviceFileBrowsingService(device);
                }
                else if (device.OSType == EnumOSType.IOS)
                {//iPhone
                    return new IOSDeviceFileBrowsingService(device);
                }
            }
            else if (source is String)
            {//文件或文件夹
                var path = source.ToString();
                if (FileHelper.IsValid(path))
                {//文件
                    if (FileHelper.IsAndroidMirrorFile(path))
                    {//安卓镜像
                        return new AndroidMirrorFileBrowsingService(path);
                    }
                    else if (FileHelper.IsIOSMirrorFile(path))
                    {//IOS镜像
                        return new IOSMirrorFileBrowsingService(path);
                    }
                }
                else if (FileHelper.IsValidDictory(path))
                {//文件夹
                    string itunsPath = string.Empty;
                    if (FileHelper.IsItunsBackupPath(path, ref itunsPath))
                    {//iTuns备份
                        return new ItunesBackupFileBrowsingService(itunsPath);
                    }

                }
            }

            return null;
        }
    }
}
