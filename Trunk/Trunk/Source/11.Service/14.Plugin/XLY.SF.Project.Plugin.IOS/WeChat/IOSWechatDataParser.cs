/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 14:48:33 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using System.Linq;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.IOS
{
    public class IOSWechatDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSWechatDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{488C0354-2C93-4FF7-A197-8C29ECA3F3D4}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Wechat);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.tencent.xin";
            pluginInfo.Icon = "\\icons\\weixin.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosWechat);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/com.tencent.xin/Documents/");
            pluginInfo.SourcePath.AddItem("/im.pre.inhouse.app1/Documents/");
            pluginInfo.SourcePath.AddItem("/com.tencent.xin.dailybuild/Documents/");
            pluginInfo.SourcePath.AddItem("/cn.bigxue.ge.123.14/Documents/");
            pluginInfo.SourcePath.AddItem("/cn.bigxue.ge.123.15/Documents/");
            pluginInfo.SourcePath.AddItem("/com.sunlink.juzi1/Documents/");
            pluginInfo.SourcePath.AddItem("/com.sunlink.juzi2/Documents/");
            pluginInfo.SourcePath.AddItem("/com.hotpot.hijun1/Documents/");
            pluginInfo.SourcePath.AddItem("/com.HangzhouDingdangTechnology.jiangGanCity1/Documents/");
            pluginInfo.SourcePath.AddItem("/com.HangzhouDingdangTechnology.jiangGanCity2/Documents/");
            pluginInfo.SourcePath.AddItem("/com.HangzhouDingdangTechnology.jiangGanCity3/Documents/");
            pluginInfo.SourcePath.AddItem("/com.HangzhouDingdangTechnology.jiangGanCity4/Documents/");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                BuildData(ds, pi.SaveDbPath, "IOS微信", pi.SourcePath[0].Local);

                BuildData(ds, pi.SaveDbPath, "Ai.粉色微信", pi.SourcePath[1].Local);

                foreach (var source in pi.SourcePath.Skip(2))
                {
                    BuildData(ds, pi.SaveDbPath, "微信分身", source.Local);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("提取微信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private void BuildData(TreeDataSource ds, string saveDbPath, string name, string DocumentsPath)
        {
            try
            {
                if (FileHelper.IsValidDictory(DocumentsPath))
                {
                    var mmPath = new DirectoryInfo(DocumentsPath).Parent.FullName;

                    var parser = new IOSWeChatDataParseCoreV1_0(saveDbPath, name, mmPath);

                    var qqNode = parser.BuildTree();

                    if (null != qqNode)
                    {
                        ds.TreeNodes.Add(qqNode);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("提取微信数据出错！", ex);
            }
        }
    }
}
