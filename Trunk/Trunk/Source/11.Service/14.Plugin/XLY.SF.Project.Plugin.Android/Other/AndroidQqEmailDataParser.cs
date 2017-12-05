/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 13:20:49 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓QQ邮箱数据解析
    /// </summary>
    public class AndroidQqEmailDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 安卓QQ邮箱数据解析
        /// </summary>
        public AndroidQqEmailDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{611D613F-E749-442B-86D5-EB40209F27E3}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_QQEmail);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_WebMail);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "2.0.4";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 4;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.tencent.androidqqmail";
            pluginInfo.Icon = "\\icons\\QQMail.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidQQEmail);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.androidqqmail/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.androidqqmail/shared_prefs/user_info.xml");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;
                var databasesPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(databasesPath))
                {
                    return ds;
                }

                BuildData(ds, pi.SaveDbPath, databasesPath, pi.SourcePath[1].Local);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓QQ邮箱数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        #region 数据解析

        private void BuildData(TreeDataSource datasource, string dbfilePath, string databasesPath, string user_infoFilePath)
        {
            var doc = new XmlDocument();
            string[] allUserInfoLines = File.ReadAllLines(user_infoFilePath, Encoding.UTF8);
            var userContent = allUserInfoLines.ToList();

            var xmlContent = new StringBuilder();
            foreach (var content in userContent)
            {
                xmlContent.Append(content);
            }

            doc.LoadXml(xmlContent.ToString());
            XmlNodeList userInfoNodeList = doc.SelectNodes("map//int[@value='1']");

            if (userInfoNodeList == null || userInfoNodeList.Count == 0)
            {
                return;
            }

            var accountsDbNames = new List<string>();
            foreach (XmlElement n in userInfoNodeList)
            {
                string userNameResource = n.Attributes["name"].Value;

                if (userNameResource.Contains("newmailnotification"))
                {
                    var qqNumber = userNameResource.Substring("newmailnotification".Length);
                    accountsDbNames.Add(qqNumber);
                }
            }

            foreach (var currDbName in accountsDbNames)
            {
                var sendTree = new TreeNode();
                sendTree.Text = LanguageHelper.GetString(Languagekeys.PluginEmail_SendBox);
                sendTree.Type = typeof(EmailInfo);
                sendTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var receiveTree = new TreeNode();
                receiveTree.Text = LanguageHelper.GetString(Languagekeys.PluginEmail_ReceiveBox);
                receiveTree.Type = typeof(EmailInfo);
                receiveTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var draftsTree = new TreeNode();
                draftsTree.Text = LanguageHelper.GetString(Languagekeys.PluginEmail_DraftsBox);
                draftsTree.Type = typeof(EmailInfo);
                draftsTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var deleteTree = new TreeNode();
                deleteTree.Text = LanguageHelper.GetString(Languagekeys.PluginEmail_DeleteBox);
                deleteTree.Type = typeof(EmailInfo);
                deleteTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var accountTree = new TreeNode();
                accountTree.Type = typeof(EmailAccount);
                accountTree.Items = new DataItems<EmailAccount>(dbfilePath);
                accountTree.Text = currDbName + "@qq.com";

                accountTree.TreeNodes.Add(sendTree);
                accountTree.TreeNodes.Add(receiveTree);
                accountTree.TreeNodes.Add(draftsTree);
                accountTree.TreeNodes.Add(deleteTree);

                string dbName = "QMDatabase" + currDbName;

                var account = new EmailAccount();
                account.Nick = currDbName;
                account.EmailAddress = accountTree.Text;
                accountTree.Items.Add(account);

                var file = Path.Combine(databasesPath, dbName);
                var nfile = SqliteRecoveryHelper.DataRecovery(file, @"\\chalib\Andriod_Tencent.mail_V2.0.4\QMDatabase1684333193.charactor", "mail");
                var context = new SqliteContext(nfile);
                var qqEmailObject = context.FindByName("mail");

                foreach (var source in qqEmailObject)
                {
                    var email = new EmailInfo();
                    email.Receiver = FormatQqEmailAccountDispaly(DynamicConvert.ToSafeString(source.receiverforsearch));
                    email.Sender = FormatQqEmailAccountDispaly(DynamicConvert.ToSafeString(source.senderforsearch));
                    email.Subject = DynamicConvert.ToSafeString(source.subject);
                    email.TextContent = DynamicConvert.ToSafeString(source.abstractcontent);
                    email.StartDate = DynamicConvert.ToSafeDateTime(source.date);
                    email.RecvDataTime = DynamicConvert.ToSafeDateTime(source.date);
                    email.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(source.XLY_DataType, EnumDataState.Normal);

                    switch (DynamicConvert.ToSafeString(source.folderid))
                    {
                        case "3":
                            sendTree.Items.Add(email);
                            break;
                        case "4":
                            draftsTree.Items.Add(email);
                            break;
                        case "5":
                            deleteTree.Items.Add(email);
                            break;
                        default:
                            receiveTree.Items.Add(email);
                            break;
                    }
                }

                datasource.TreeNodes.Add(accountTree);
            }
        }

        private string FormatQqEmailAccountDispaly(string source)
        {
            var reslut = new StringBuilder();
            var resoureArray = source.Split('\n');
            for (int i = 0; i < resoureArray.Length; i++)
            {
                if (i % 2 == 0)
                {
                    reslut.Append(resoureArray[i]);
                }
                else
                {
                    reslut.Append("<" + resoureArray[i] + ">");
                }
            }

            return reslut.ToString();
        }

        #endregion

    }
}
