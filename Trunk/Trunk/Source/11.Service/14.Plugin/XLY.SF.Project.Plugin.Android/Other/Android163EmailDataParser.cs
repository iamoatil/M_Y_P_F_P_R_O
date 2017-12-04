/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 13:56:30 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓163邮箱数据解析
    /// </summary>
    public class Android163EmailDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 安卓163邮箱数据解析
        /// </summary>
        public Android163EmailDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{0D70BCDB-9388-4A4C-A8D0-AFF1E90408A9}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Email163);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_WebMail);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "3.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 4;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.netease.mobimail";
            pluginInfo.Icon = "\\icons\\Mail163.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidEmail163);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.netease.mobimail/mmail");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;
                var mmailFile = pi.SourcePath[0].Local;

                if (!FileHelper.IsValid(mmailFile))
                {
                    return ds;
                }

                BuildData(ds, pi.SaveDbPath, mmailFile);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓网易邮箱数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        #region 数据解析

        private void BuildData(TreeDataSource datasource, string dbfilePath, string mmailFilePath)
        {

            var nfile = SqliteRecoveryHelper.DataRecovery(mmailFilePath, @"\chalib\Andriod_163mail\163mmail.charactor", "Account");
            var context = new SqliteContext(nfile);

            //获取163账户。
            var accountObject = context.FindByName("Account");

            IEnumerable<EmailAccount> acccounts = this.GetEmailAccounts(accountObject);

            //获取163邮件账户对应的所有邮件。
            foreach (var account in acccounts)
            {
                var sendTree = new TreeNode();
                sendTree.Text = "发件箱";
                sendTree.Type = typeof(EmailInfo);
                sendTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var receiveTree = new TreeNode();
                receiveTree.Text = "收件箱";
                receiveTree.Type = typeof(EmailInfo);
                receiveTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var draftsTree = new TreeNode();
                draftsTree.Text = "草稿箱";
                draftsTree.Type = typeof(EmailInfo);
                draftsTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var deleteTree = new TreeNode();
                deleteTree.Text = "删除邮件";
                deleteTree.Type = typeof(EmailInfo);
                deleteTree.Items = new DataItems<EmailInfo>(dbfilePath);

                var accountTree = new TreeNode();
                accountTree.Type = typeof(EmailAccount);
                accountTree.Items = new DataItems<EmailAccount>(dbfilePath);
                accountTree.Text = account.Nick + "<" + account.EmailAddress + ">";
                accountTree.DataState = account.DataState;

                accountTree.TreeNodes.Add(sendTree);
                accountTree.TreeNodes.Add(receiveTree);
                accountTree.TreeNodes.Add(draftsTree);
                accountTree.TreeNodes.Add(deleteTree);

                accountTree.Items.Add(account);

                //读取账号对应邮件内容。
                var tname = "Mail_" + account.Id;
                nfile = SqliteRecoveryHelper.DataRecovery(mmailFilePath, @"\chalib\Andriod_163mail\mmail.charactor", tname);
                context = new SqliteContext(nfile);
                var emailContentObj = context.FindByName(tname);

                foreach (var source in emailContentObj)
                {
                    var email = new EmailInfo();

                    email.Receiver = JsonArrayFormatDisplay(DynamicConvert.ToSafeString(source.mailTo));

                    EmailUserInfo userInfo = JsonFormatDisplay(DynamicConvert.ToSafeString(source.mailFrom));
                    email.Sender = userInfo.Name + "<" + userInfo.MailAddress + ">";

                    email.Subject = DynamicConvert.ToSafeString(source.subject);
                    email.TextContent = DynamicConvert.ToSafeString(source.textContent);
                    email.StartDate = DynamicConvert.ToSafeDateTime(source.sendDate);
                    email.RecvDataTime = DynamicConvert.ToSafeDateTime(source.recvDate);
                    email.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(source.XLY_DataType, EnumDataState.Normal);

                    string tempStatus = DynamicConvert.ToSafeString(source.mailboxKey);
                    if (tempStatus == "订阅" || tempStatus == "INBOX")
                    {
                        receiveTree.Items.Add(email);
                    }
                    else if (tempStatus == "已删除")
                    {
                        deleteTree.Items.Add(email);
                    }
                    else if (tempStatus == "已发送")
                    {
                        sendTree.Items.Add(email);
                    }
                    else
                    {
                        receiveTree.Items.Add(email);
                    }
                }

                datasource.TreeNodes.Add(accountTree);
            }
        }

        private string JsonArrayFormatDisplay(string jsonArrText)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonArrText)))
            {
                var jsonSerializer = new DataContractJsonSerializer(typeof(EmailUserInfo[]));
                EmailUserInfo[] userInfos;
                try
                {
                    userInfos = (EmailUserInfo[])jsonSerializer.ReadObject(ms);
                }
                catch
                {
                    return string.Empty;
                }

                var dispalyBuilder = new StringBuilder();
                for (int i = 0; i < userInfos.Length; i++)
                {
                    dispalyBuilder.Append(userInfos[i].Name + "<" + userInfos[i].MailAddress + ">");
                    if (i < userInfos.Length - 1)
                    {
                        dispalyBuilder.AppendLine();
                    }
                }

                return dispalyBuilder.ToString();
            }
        }

        private EmailUserInfo JsonFormatDisplay(string jsonArrText)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonArrText)))
            {
                var jsonSerializer = new DataContractJsonSerializer(typeof(EmailUserInfo));
                var userInfos = new EmailUserInfo();
                try
                {
                    userInfos = (EmailUserInfo)jsonSerializer.ReadObject(ms);
                }
                catch
                {
                }
                return userInfos;
            }
        }

        private IEnumerable<EmailAccount> GetEmailAccounts(IEnumerable<dynamic> sources)
        {
            var accounts = new List<EmailAccount>();
            foreach (var source in sources)
            {
                var account = new EmailAccount();
                account.Id = DynamicConvert.ToSafeInt(source.id);
                EmailUserInfo userInfo = JsonFormatDisplay((string)DynamicConvert.ToSafeString(source.currentSender));
                account.Nick = userInfo.Name;
                account.EmailAddress = userInfo.MailAddress;
                account.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(source.XLY_DataType, EnumDataState.Normal);
                accounts.Add(account);
            }

            return accounts;
        }

        /// <summary>
        /// 邮件用户信息类。
        /// 本类加入的特性标示主要用于Json解析。
        /// </summary>
        [DataContract]
        class EmailUserInfo
        {
            [DataMember(Name = "mailAddress")]
            public string MailAddress { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }
        }

        #endregion

    }
}
