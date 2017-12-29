/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    public class MTPPictureDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public MTPPictureDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{1FFF30B3-3B01-4856-9D03-12EB563941E4}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Image);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android | EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.MTP;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "Image";
            pluginInfo.Icon = "\\icons\\pictures.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_MTPImage);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("$Image,jpg;jpeg;bmp;png;exif;dxf;pcx;fpx;ufo;tiff;svg;eps;gif;psd;ai;cdr;tga;pcd;hdri;map");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            FileTreeDataSource ds = new FileTreeDataSource();
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                if (FileHelper.IsValidDictory(pi.SourcePath[0].Local))
                {
                    FileDataParser.GetImageFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取MTP设备图片文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
