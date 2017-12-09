/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/17 10:25:46 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS微信数据解析
    /// </summary>
    internal class IOSWeChatDataParseCoreV1_0
    {
        /// <summary>
        /// IOS微信数据解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="name">微信 例如 微信、小米微信分身</param>
        /// <param name="datapath">微信数据文件根目录，例如 I:\本地数据\com.tencent.xin</param>
        public IOSWeChatDataParseCoreV1_0(string savedatadbpath, string name, string datapath)
        {
            DbFilePath = savedatadbpath;
            WeChatName = name;
            DataFileRootPath = datapath;
        }

        #region 其他属性

        private const string _ftsMsgTables = "fts_message_table_0_content,fts_message_table_1_content,fts_message_table_2_content,fts_message_table_3_content,fts_message_table_4_content,fts_message_table_5_content,fts_message_table_6_content,fts_message_table_7_content,fts_message_table_8_content,fts_message_table_9_content";
        private const string _ftsMsgNewTable = "fts_message_content";

        #endregion

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// 微信名称，例如 微信、粉色微信
        /// </summary>
        private string WeChatName { get; set; }

        /// <summary>
        ///微信数据文件根目录，例如 I:\本地数据\com.tencent.xin
        ///该文件下包含了Documents子文件夹，保存了微信相关数据和文件
        /// </summary>
        private string DataFileRootPath { get; set; }

        #endregion

        #region 临时属性

        /// <summary>
        /// 当前微信帐号文件夹
        /// </summary>
        private string MD5AccountPath { get; set; }

        /// <summary>
        /// 当前微信帐号
        /// </summary>
        private WeChatLoginShow WeChatAccount { get; set; }

        /// <summary>
        /// 当前微信帐号显示名称
        /// </summary>
        private string WeChatAccountShowName { get; set; }

        /// <summary>
        /// 主数据库 MM.sqlite
        /// </summary>
        private SqliteContext MainContext { get; set; }

        /// <summary>
        /// fts_message.db 用于删除数据提取
        /// </summary>
        private SqliteContext FtsContext { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        private List<WeChatFriendShow> LsAllFriends { get; set; }

        /// <summary>
        /// 群组好友
        /// </summary>
        private List<WeChatGroupShow> LsAllGroupFriends { get; set; }

        /// <summary>
        /// 清除临时属性
        /// 一般用于插件执行完毕后执行
        /// </summary>
        private void ClearCache()
        {
            MD5AccountPath = string.Empty;
            WeChatAccount = null;
            WeChatAccountShowName = string.Empty;

            MainContext?.Dispose();
            MainContext = null;

            FtsContext?.Dispose();
            FtsContext = null;

            LsAllFriends?.Clear();
            LsAllFriends = null;

            LsAllGroupFriends?.Clear();
            LsAllGroupFriends = null;

        }

        #endregion

        /// <summary>
        /// 解析微信数据
        /// </summary>
        /// <returns></returns>
        public TreeNode BuildTree()
        {
            TreeNode rootNode = new TreeNode();
            try
            {
                rootNode.Text = WeChatName;
                rootNode.Type = typeof(WeChatLoginShow);
                rootNode.Items = new DataItems<WeChatLoginShow>(DbFilePath);

                var accountPaths = LoadSourcePath();
                foreach (var path in accountPaths)
                {
                    ClearCache();

                    BuildWeChatTree(rootNode, path);
                }

                return rootNode;
            }
            finally
            {
                rootNode.BuildParent();

                ClearCache();
            }
        }

        #region 构建数据

        /// <summary>
        /// 构建微信帐号树
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="sourcePath">微信帐号文件夹 例如 D:\com.tencent.xin\Documents\9cb0637d719e7caa3c5b90a27427c097</param>
        private void BuildWeChatTree(TreeNode rootNode, string sourcePath)
        {
            MD5AccountPath = sourcePath;

            //构建帐号信息
            var accountNode = BuildAccountInfo(rootNode);

            //数据库恢复
            GetSqliteContext();

            //获取通讯录列表
            LoadAllContacts();

            //构建好友和公众号
            BuildContactsNode(accountNode);

            //构建通讯录消息
            BuildContactsMsgNode(accountNode);

            //构建群聊
            BuildGroupNode(accountNode);

            //构建群聊消息
            BuildGroupMsgNode(accountNode);

            //构建群发消息树
            BuildMassendMsgNode(accountNode);

            //构建其他删除消息
            BuildOtherFTSMsgNode(accountNode);

            //构建朋友圈
            BuildSnsNode(accountNode);

            //构建 微信支付
            BuildMyWalletNode(accountNode);
        }

        /// <summary>
        /// 构建微信帐号信息
        /// </summary>
        /// <param name="rootNode">微信根节点</param>
        /// <returns>微信帐号根节点</returns>
        private TreeNode BuildAccountInfo(TreeNode rootNode)
        {
            //获取帐号信息
            WeChatAccount = GetAccountUser();
            WeChatAccountShowName = WeChatAccount.ShowName;

            //当前账户树节点
            var accountNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = string.Format("{0}({1})", WeChatAccount.Nick, WeChatAccount.WeChatId),
                Id = WeChatAccount.WeChatId,
                Type = typeof(WeChatLoginShow),
                Items = new DataItems<WeChatLoginShow>(DbFilePath)
            };
            accountNode.Items.Add(WeChatAccount);
            rootNode.Items.Add(WeChatAccount);
            rootNode.TreeNodes.Add(accountNode);

            return accountNode;
        }

        /// <summary>
        /// 构建通讯录
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildContactsNode(TreeNode rootNode)
        {
            var friendNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_AddressBook),
                Type = typeof(WeChatFriendShow),
                Items = new DataItems<WeChatFriendShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            var gongzhonghaoNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_Public),
                Type = typeof(WeChatFriendShow),
                Items = new DataItems<WeChatFriendShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(friendNode);
            rootNode.TreeNodes.Add(gongzhonghaoNode);

            foreach (var friend in LsAllFriends)
            {
                if (friend.FriendType == WeChatFriendTypeEnum.Subscription)
                {
                    gongzhonghaoNode.Items.Add(friend);
                }
                else
                {
                    friendNode.Items.Add(friend);
                }
            }
        }

        /// <summary>
        /// 构建通讯录消息
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildContactsMsgNode(TreeNode rootNode)
        {
            var msgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_FriendMsg),
                Id = WeChatAccount.WeChatId
            };

            var ghMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_PublicMsg),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(msgNode);
            rootNode.TreeNodes.Add(ghMsgNode);

            TreeNode friendMsgNode;
            foreach (var friend in LsAllFriends)
            {
                friendMsgNode = LoadFriendMessage(friend);
                if (null != friendMsgNode)
                {
                    if (friend.FriendType == WeChatFriendTypeEnum.Subscription)
                    {
                        ghMsgNode.TreeNodes.Add(friendMsgNode);
                    }
                    else
                    {
                        msgNode.TreeNodes.Add(friendMsgNode);
                    }
                }
            }
        }

        /// <summary>
        /// 构建群聊树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildGroupNode(TreeNode rootNode)
        {
            var groupNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_ChatRoom),
                Type = typeof(WeChatGroupShow),
                Items = new DataItems<WeChatGroupShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(groupNode);

            foreach (var group in LsAllGroupFriends)
            {
                groupNode.Items.Add(group);
            }
        }

        /// <summary>
        /// 构建群聊消息树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildGroupMsgNode(TreeNode rootNode)
        {
            var groupMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_ChatRoomMsg),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(groupMsgNode);

            TreeNode msgNode;
            foreach (var group in LsAllGroupFriends)
            {
                msgNode = LoadGroupMessage(group);
                if (null != msgNode)
                {
                    groupMsgNode.TreeNodes.Add(msgNode);
                }
            }
        }

        /// <summary>
        /// 构建群发消息树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildMassendMsgNode(TreeNode rootNode)
        {
            TreeNode massendNode = new TreeNode()
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_MassendMsg),
                Items = new DataItems<MessageCore>(DbFilePath),
                Type = typeof(MessageCore)
            };

            rootNode.TreeNodes.Add(massendNode);

            if (!MainContext.ExistTable("Chat_a971a8ea24a72bb45e64826275fc017f"))
            {
                return;
            }

            var lsMsgs = MainContext.Find(new SQLiteString("SELECT MesLocalID,CreateTime,Message,Type FROM Chat_a971a8ea24a72bb45e64826275fc017f"));
            if (lsMsgs.IsInvalid())
            {
                return;
            }

            Dictionary<string, string> massSendContact = new Dictionary<string, string>();
            string WCDBPath = Path.Combine(MD5AccountPath, "DB", "WCDB_Contact.sqlite");
            if (File.Exists(WCDBPath))
            {
                using (var wcdbContext = new SqliteContext(WCDBPath))
                {
                    foreach (var contact in wcdbContext.Find(new SQLiteString("SELECT UsrName,Detail FROM MassSendContact")))
                    {
                        massSendContact.Add(DynamicConvert.ToSafeString(contact.UsrName), DynamicConvert.ToSafeString(contact.Detail));
                    }
                }
            }

            MessageCore message = null;
            string typeStr = string.Empty;
            string localID = string.Empty;
            string tolistmd5 = string.Empty;
            string msgStr = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var massendInfo in lsMsgs)
            {
                message = new MessageCore();
                message.SenderName = WeChatAccountShowName;
                message.SenderImage = WeChatAccount.HeadPng;
                message.SendState = EnumSendState.Send;
                message.Date = DynamicConvert.ToSafeDateTime(massendInfo.CreateTime);

                localID = DynamicConvert.ToSafeString(massendInfo.MesLocalID);
                typeStr = DynamicConvert.ToSafeString(massendInfo.Type);

                GetMassInfo(DynamicConvert.ToSafeString(massendInfo.Message), out tolistmd5, out msgStr);

                if (massSendContact.Keys.Contains(tolistmd5))
                {
                    var tolist = massSendContact[tolistmd5];

                    sb.Clear();
                    foreach (var toUin in tolist.Split(';'))
                    {
                        var toFriend = LsAllFriends.FirstOrDefault(f => f.WeChatId == toUin);
                        if (null != toFriend)
                        {
                            sb.AppendLine(toFriend.ShowName);
                        }
                        else
                        {
                            sb.AppendLine(toUin);
                        }
                    }
                    message.Receiver = sb.ToString().Trim();
                }

                switch (typeStr)
                {
                    case "3":
                        message.Type = EnumColumnType.Image;
                        message.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Image);
                        message.Content = string.Format("{0}\\{1}.pic", GetMediaPath("Img", "a971a8ea24a72bb45e64826275fc017f"), localID);
                        break;
                    default:
                        message.Type = EnumColumnType.String;
                        message.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                        message.Content = msgStr;
                        break;
                }

                massendNode.Items.Add(message);

            }
        }

        /// <summary>
        /// 构建FTS数据库其他删除消息
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildOtherFTSMsgNode(TreeNode rootNode)
        {
            var ftsIndexMessageContentNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_OtherDelMsg),
                Id = WeChatAccount.WeChatId,
                Type = typeof(MessageCore)
            };
            var wordMsg = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_WordDelMsg),
                Id = WeChatAccount.WeChatId,
                Type = typeof(MessageCore)
            };
            var numMsg = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_NumDelMsg),
                Id = WeChatAccount.WeChatId,
                Type = typeof(MessageCore)
            };

            ftsIndexMessageContentNode.TreeNodes.Add(wordMsg);
            ftsIndexMessageContentNode.TreeNodes.Add(numMsg);
            rootNode.TreeNodes.Add(ftsIndexMessageContentNode);

            if (null == FtsContext)
            {
                return;
            }

            var sql = @"SELECT
                        	msg.c2CreateTime,
                        	msg.c3Message
                        FROM
                        	fts_message_content msg
                        LEFT JOIN fts_username_id user ON msg.c0usernameid = user.usernameid
                        WHERE
                        	UsrName ISNULL";

            FtsContext.UsingSafeConnection(sql, r =>
             {
                 dynamic msg;
                 MessageCore mc;
                 string strMsg;

                 while (r.Read())
                 {
                     msg = r.ToDynamic();

                     strMsg = DynamicConvert.ToSafeString(msg.c3Message);
                     if (!Regex.IsMatch(strMsg, @"\w+"))
                     {
                         continue;
                     }

                     if (Regex.IsMatch(strMsg, @"[\u4E00-\u9FA5]"))
                     {
                         mc = new MessageCore();
                         mc.Content = strMsg;
                         mc.DataState = EnumDataState.Deleted;
                         mc.Date = DynamicConvert.ToSafeDateTime(msg.c2CreateTime);

                         wordMsg.Items.Add(mc);
                     }
                     else if (Regex.IsMatch(strMsg.Trim(), @"^[\d.-]+$"))
                     {
                         mc = new MessageCore();
                         mc.Content = strMsg;
                         mc.DataState = EnumDataState.Deleted;
                         mc.Date = DynamicConvert.ToSafeDateTime(msg.c2CreateTime);

                         numMsg.Items.Add(mc);
                     }
                     else
                     {
                         continue;
                     }
                 }
             });
        }

        /// <summary>
        /// 构建朋友圈
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildSnsNode(TreeNode rootNode)
        {
            TreeNode snsTree = new TreeNode()
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_Sns),
                Items = new DataItems<WeChatIosSns>(DbFilePath),
                Type = typeof(WeChatIosSns)
            };

            rootNode.TreeNodes.Add(snsTree);

            var snsDbPath = Path.Combine(MD5AccountPath, "wc", "wc005_008.db");
            if (!FileHelper.IsValid(snsDbPath))
            {
                return;
            }

            byte[] buffer = null;
            WeChatIosSns sns = null;
            StringBuilder sb = null;
            Dictionary<string, string> dicCommentUsers = null;

            using (var snsDbContext = new SqliteContext(snsDbPath))
            {
                snsDbContext.UsingSafeConnection("SELECT Buffer FROM MyWC_Timeline01 ORDER BY Id DESC", r =>
                {
                    dynamic data;

                    while (r.Read())
                    {
                        data = r.ToDynamic();
                        try
                        {
                            #region 解析朋友圈数据

                            try
                            {
                                buffer = data.Buffer;
                                if (buffer.IsValid())
                                {
                                    var jo = PListToJsonHelper.PlistToJson(buffer);

                                    sns = new WeChatIosSns();

                                    //发表人
                                    sns.Creater = string.Format("{0}({1})", jo["nickname"].ToSafeString(), jo["username"].ToSafeString());

                                    //发表时间
                                    sns.Createtime = jo["createtime"].ToSafeString().ToSafeInt64().ToSafeDateTime().Value.AddHours(8);

                                    //发表内容
                                    sns.Content = jo["contentDesc"].ToSafeString();

                                    var contentObjLinkUrl = jo["contentObj"]["linkUrl"].ToSafeString();
                                    var contentObjTitle = jo["contentObj"]["title"].ToSafeString();
                                    var contentObjDesc = jo["contentObj"]["desc"].ToSafeString();
                                    //var contentObjType = jo["contentObj"]["type"].ToSafeString();
                                    if (contentObjTitle.IsValid())
                                    {
                                        if (sns.Content.IsValid())
                                        {
                                            sns.Content = string.Join("\r\n", sns.Content, contentObjTitle, contentObjDesc, contentObjLinkUrl);
                                        }
                                        else
                                        {
                                            sns.Content = string.Join("\r\n", contentObjTitle, contentObjDesc, contentObjLinkUrl);
                                        }
                                    }
                                    sns.Content = sns.Content.Trim();

                                    //附件
                                    var mediaList = jo["contentObj"]["mediaList"] as JArray;
                                    if (mediaList.IsValid())
                                    {
                                        sns.MediaCount = mediaList.Count;
                                        sns.MediaList = new List<string>();

                                        foreach (var media in mediaList)
                                        {
                                            sns.MediaList.Add(media["dataUrl"]["url"].ToSafeString());
                                        }
                                    }

                                    //评论内容
                                    sns.CommentCount = jo["realCommentCount"].ToSafeString().ToSafeInt();
                                    if (sns.CommentCount > 0)
                                    {
                                        dicCommentUsers = new Dictionary<string, string>();
                                        sb = new StringBuilder();

                                        var commentUsers = jo["commentUsers"] as JArray;
                                        foreach (var commentUser in commentUsers)
                                        {
                                            var username = commentUser["username"].ToSafeString();
                                            var nickname = commentUser["nickname"].ToSafeString();
                                            var userFullName = string.Format("{0}({1})", commentUser["nickname"].ToSafeString(), commentUser["username"].ToSafeString());
                                            var content = commentUser["content"].ToSafeString();
                                            var refUserName = commentUser["refUserName"].ToSafeString();
                                            var createTime = commentUser["createTime"].ToSafeString().ToSafeInt64().ToSafeDateTime().Value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");

                                            if (!dicCommentUsers.Keys.Contains(username))
                                            {
                                                dicCommentUsers.Add(username, nickname);
                                            }

                                            if (refUserName.IsValid() && dicCommentUsers.Keys.Contains(refUserName))
                                            {
                                                content = string.Format("{0} @ {1}:{2}", userFullName, dicCommentUsers[refUserName], content);
                                            }
                                            else
                                            {
                                                content = string.Format("{0}:{1}", userFullName, content);
                                            }

                                            sb.AppendFormat("{0} {1}\r\n", createTime, content);
                                        }
                                        sns.Comment = sb.ToString().TrimEnd("\r\n");
                                    }

                                    //点赞
                                    sns.LikeCount = jo["realLikeCount"].ToSafeString().ToSafeInt();
                                    if (sns.LikeCount > 0)
                                    {
                                        sb = new StringBuilder();

                                        var likeUsers = jo["likeUsers"] as JArray;
                                        foreach (var likeUser in likeUsers)
                                        {
                                            sb.AppendFormat("{0} {1}({2})\r\n",
                                                            likeUser["createTime"].ToSafeString().ToSafeInt64().ToSafeDateTime().Value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                                                            likeUser["nickname"].ToSafeString(),
                                                            likeUser["username"].ToSafeString());
                                        }
                                        sns.Like = sb.ToString().TrimEnd("\r\n");
                                    }
                                    sns.LikeFlag = jo["likeFlag"].ToSafeString() != "false";


                                    //定位信息
                                    var city = jo["locationInfo"]["city"].ToSafeString();
                                    var poiAddress = jo["locationInfo"]["poiAddress"].ToSafeString();
                                    var poiName = jo["locationInfo"]["poiName"].ToSafeString();
                                    sns.LocationInfo = string.Format("{0} {1} {2}", city, poiAddress, poiName).Trim();

                                    //APP名称
                                    sns.AppName = jo["appInfo"]["appName"].ToSafeString();

                                    snsTree.Items.Add(sns);
                                }
                            }
                            catch
                            {
                            }

                            #endregion
                        }
                        catch { }
                    }
                });
            }

        }

        /// <summary>
        /// 构建 微信绑定银行卡
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildMyWalletNode(TreeNode rootNode)
        {
            TreeNode tree = new TreeNode()
            {
                DataState = EnumDataState.Normal,
                Text = LanguageHelper.GetString(Languagekeys.PluginWechat_MyWalle),
                Items = new DataItems<WeChatBackCard>(DbFilePath),
                Type = typeof(WeChatBackCard)
            };

            rootNode.TreeNodes.Add(tree);

            var plistFile = Path.Combine(MD5AccountPath, "WCPay", "WCPayPayCardList.list");
            if (!FileHelper.IsValid(plistFile))
            {
                return;
            }

            var plist = PListToJsonHelper.PlistToJson(plistFile);

            foreach (var jo in plist)
            {
                if (jo["m_bankFlag"].ToSafeString() == "false")
                {
                    continue;
                }

                var cardInfo = new WeChatBackCard();
                cardInfo.BankName = jo["m_cardBankName"].ToSafeString();
                cardInfo.BankType = jo["m_cardTypeName"].ToSafeString();
                cardInfo.Phone = jo["m_bankPhone"].ToSafeString();
                cardInfo.Number = jo["m_cardNumber"].ToSafeString();

                tree.Items.Add(cardInfo);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        ///  加载微信用户数据源路径
        /// </summary>
        private List<string> LoadSourcePath()
        {
            var sourcePath = Path.Combine(DataFileRootPath, "Documents");

            var sourcepaths = new List<string>();

            var dirInfo = new DirectoryInfo(sourcePath);
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("mmsetting.archive", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (!file.Directory.Name.IsMatch("^0+$"))
                    {
                        sourcepaths.Add(file.Directory.ToString());
                    }
                }
            }

            return sourcepaths;
        }

        /// <summary>
        /// 获取微信帐号信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private WeChatLoginShow GetAccountUser()
        {
            var show = new WeChatLoginShow();

            var sourcePath = Path.Combine(MD5AccountPath, "mmsetting.archive");
            var data = PListToJsonHelper.PlistToJson(sourcePath) as JObject;

            show.WeChatId = data["UsrName"].ToSafeString();
            show.WeChatAccout = data["AliasName"].ToSafeString();
            show.Nick = data["NickName"].ToSafeString();
            show.Signature = data["Signature"].ToSafeString() == "$null" ? "" : data["Signature"].ToSafeString();
            show.BindingQQ = data["PushmailFolderUrl"].ToSafeString() == "$null" ? "" : data["PushmailFolderUrl"].ToSafeString();
            if (Regex.IsMatch(show.BindingQQ, @"uin=(\d+)"))
            {
                show.BindingQQ = Regex.Match(show.BindingQQ, @"uin=(\d+)").Groups[1].Value;
                show.BindingQQ = show.BindingQQ == "0" ? "" : show.BindingQQ;
            }
            else
            {
                show.BindingQQ = string.Empty;
            }
            show.BindingEmail = data["Email"].ToSafeString() == "$null" ? "" : data["Email"].ToSafeString();
            show.BindingPhone = data["Mobile"].ToSafeString() == "$null" ? "" : data["Mobile"].ToSafeString();
            show.BindingWeiBo = data["WeiboAddress"].ToSafeString() == "$null" ? "" : data["WeiboAddress"].ToSafeString();
            show.WeiBoNick = data["WeiboNickName"].ToSafeString() == "$null" ? "" : data["WeiboNickName"].ToSafeString();
            show.Address = data["City"].ToSafeString() == "$null" ? "" : data["City"].ToSafeString();

            if (null != data["new_dicsetting"])
            {
                show.HeadUrl = data["new_dicsetting"]["headhdimgurl"].ToSafeString() == "$null" ? "" : data["new_dicsetting"]["headhdimgurl"].ToSafeString();
            }

            show.HeadPng = GetHeadImage(show.WeChatId);

            //show.WeiBoBackImg = data["m_pcWCBGImgID"].ToSafeString() == "$null" ? "" : data["m_pcWCBGImgID"].ToSafeString();
            //data["m_pcWCBGImgID"].ToSafeString() == "$null" ? "" : data["m_pcWCBGImgID"].ToSafeString(); //背景图

            if ("1" == data["Sex"].ToSafeString())
            {
                show.Gender = EnumSex.Male;
            }
            else
            {
                show.Gender = EnumSex.Female;
            }

            return show;
        }

        /// <summary>
        /// 构建动态恢复的表
        /// </summary>
        /// <param name="sourcedb"></param>
        /// <returns></returns>
        private string RecoveryTables(string sourcedb)
        {
            var sqliteObj = new SqliteContext(sourcedb);

            var listTables = sqliteObj.Find(new SQLiteString("select tbl_name from sqlite_master where type = 'table'")).
                Select(table => DynamicConvert.ToSafeString(table.tbl_name)).Cast<string>().Where(t => t.StartsWith("Chat_")).ToList();

            listTables.Add("Friend");
            listTables.Add("Friend_Ext");

            return string.Join(",", listTables);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        private void GetSqliteContext()
        {
            string sourceDbPath = Path.Combine(MD5AccountPath, "DB", "MM.sqlite");
            var recoveryDb = SqliteRecoveryHelper.DataRecovery(sourceDbPath, @"chalib\IOS_WeChat\MM.sqlite.charactor", RecoveryTables(sourceDbPath));
            MainContext = new SqliteContext(recoveryDb);

            string sourceFtsPath = Path.Combine(MD5AccountPath, "fts", "fts_message.db");
            if (File.Exists(sourceFtsPath))
            {
                try
                {
                    Dictionary<string, string> dicIndex = new Dictionary<string, string>();
                    dicIndex.Add("fts_username_id", "UsrName");

                    FtsContext = new SqliteContext(SqliteRecoveryHelper.DataRecovery(
                                                sourceFtsPath, @"chalib\IOS_WeChat\fts_message.db.charactor", _ftsMsgTables + ",fts_username_id", "", "", true, dicIndex));
                    FtsContext.MergeTables(_ftsMsgTables, _ftsMsgNewTable, "IndexNameId", "c0usernameid");
                }
                catch
                {
                    FtsContext = null;
                }
            }
        }

        /// <summary>
        /// 获取通讯录  包括好友和群聊
        /// </summary>
        private void LoadAllContacts()
        {
            LsAllFriends = new List<WeChatFriendShow>();
            LsAllGroupFriends = new List<WeChatGroupShow>();

            #region 从MM.sqlite的Friend表获取好友信息

            var sql = @"SELECT
                            f.*, 
                            e.*
                        FROM
                            Friend f
                        LEFT JOIN Friend_Ext e ON f.UsrName = e.UsrName";
            MainContext.UsingSafeConnection(sql, r =>
            {
                dynamic data;
                WeChatFriendShow friendInfo;

                while (r.Read())
                {
                    data = r.ToDynamic();

                    if (DynamicConvert.ToSafeString(data.UsrName).Contains("@chatroom"))
                    {
                        LsAllGroupFriends.Add(CreateWeChatGroupShow(data));
                    }
                    else
                    {
                        friendInfo = new WeChatFriendShow();
                        CreateWeChatFriendShow(data, ref friendInfo);

                        LsAllFriends.Add(friendInfo);
                    }
                }
            });

            #endregion

            #region 从WCDB_Contact.sqlite的表Friend获取好友信息

            string WCDBPath = Path.Combine(MD5AccountPath, "DB", "WCDB_Contact.sqlite");
            if (FileHelper.IsValid(WCDBPath))
            {
                List<char> listC = new List<char>();
                for (int i = 0; i < 32; i++)
                {
                    listC.Add((char)i);
                }
                listC.Add((char)127);
                var arrC = listC.ToArray();//分割字符

                using (var WCDB = new SqliteContext(WCDBPath))
                {
                    WCDB.UsingSafeConnection("SELECT * FROM Friend WHERE imgStatus != 1", r =>
                    {//imgStatus为1的记录为系统功能
                        dynamic fd;
                        DynamicEx dy;
                        WeChatFriendShow friendInfo;

                        while (r.Read())
                        {
                            try
                            {
                                fd = r.ToDynamic();

                                dy = new DynamicEx();
                                dy.Set("UsrName", fd.userName);
                                dy.Set("type", fd.type);
                                dy.Set("certificationFlag", fd.certificationFlag);
                                dy.Set("imgStatus", fd.imgStatus);
                                dy.Set("XLY_DataType", "2");

                                byte[] dbContactRemark = fd.dbContactRemark;
                                if (dbContactRemark.IsValid())
                                {//获取昵称、备注、修改后微信号
                                    GetdbContactRemark(dbContactRemark, ref dy);
                                }

                                byte[] dbContactProfile = fd.dbContactProfile;
                                if (dbContactProfile.IsValid())
                                {//获取位置和签名
                                    GetdbContactProfile(dbContactProfile, ref dy);
                                }

                                byte[] dbContactChatRoom = fd.dbContactChatRoom;
                                if (dbContactChatRoom.IsValid())
                                {//获取群组成员列表
                                    var dArr = System.Text.Encoding.UTF8.GetString(dbContactChatRoom).Split(arrC, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (dArr.IsValid())
                                    {
                                        string ConChatRoomMem = dArr.FirstOrDefault(d => d.Contains(";"));
                                        string ConChatRoomOwner = "";
                                        if (dArr.IndexOf(ConChatRoomMem) + 1 < dArr.Count)
                                        {//群组的创建者在群组成员后面
                                            ConChatRoomOwner = dArr[dArr.IndexOf(ConChatRoomMem) + 1];
                                        }

                                        if (0 == dArr.IndexOf(ConChatRoomMem))
                                        {//如果成员的索引是0，前面会多出一个随机的字符
                                            ConChatRoomMem = ConChatRoomMem.Substring(1);
                                        }

                                        dy.Set("ConChatRoomMem", ConChatRoomMem);
                                        dy.Set("ConChatRoomOwner", ConChatRoomOwner);
                                    }
                                }

                                if (DynamicConvert.ToSafeString(fd.userName).Contains("@chatroom"))
                                {
                                    LsAllGroupFriends.Add(CreateWeChatGroupShow(dy));
                                }
                                else
                                {
                                    friendInfo = new WeChatFriendShow();
                                    CreateWeChatFriendShow(dy, ref friendInfo);

                                    LsAllFriends.Add(friendInfo);
                                }
                            }
                            catch { }
                        }
                    });
                }
            }

            #endregion
        }

        #region 解析好友信息

        /// <summary>
        /// 构建好友信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="friendInfo"></param>
        private void CreateWeChatFriendShow(dynamic data, ref WeChatFriendShow friendInfo)
        {
            friendInfo.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            friendInfo.WeChatId = DynamicConvert.ToSafeString(data.UsrName);
            friendInfo.WeChatAccout = DynamicConvert.ToSafeString(data.Alias);
            friendInfo.Nick = DynamicConvert.ToSafeString(data.NickName);
            friendInfo.Email = DynamicConvert.ToSafeString(data.Email);
            friendInfo.Mobile = DynamicConvert.ToSafeString(data.Mobile);
            friendInfo.Remark = DynamicConvert.ToSafeString(data.ConRemark);
            friendInfo.Signature = DynamicConvert.ToSafeString(data.Signature);

            if (DynamicConvert.ToSafeString(data.Sex) == "1")
            {
                friendInfo.Gender = EnumSex.Male;
            }
            else
            {
                friendInfo.Gender = EnumSex.Female;
            }

            friendInfo.FriendType = GetFriendType(friendInfo.WeChatId,
                DynamicConvert.ToSafeString(data.certificationFlag), DynamicConvert.ToSafeString(data.imgStatus), DynamicConvert.ToSafeString(data.type));

            GetFriendInfo(DynamicConvert.ToSafeString(data.ConStrRes2), ref friendInfo);

            if (friendInfo.Remark.IsInvalid())
            {
                friendInfo.Remark = DynamicConvert.ToSafeString(data.RemarkName);
            }
            if (friendInfo.Address.IsInvalid())
            {
                friendInfo.Address = string.Format("{0}-{1}-{2}", DynamicConvert.ToSafeString(data.Country), DynamicConvert.ToSafeString(data.Province), DynamicConvert.ToSafeString(data.City));
            }

            friendInfo.HeadPng = GetHeadImage(friendInfo.WeChatId);
        }

        /// <summary>
        /// 构建群聊信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private WeChatGroupShow CreateWeChatGroupShow(dynamic data)
        {
            var group = new WeChatGroupShow();
            group.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            group.WeChatId = data.UsrName;
            group.GroupName = data.NickName;
            group.Member = DynamicConvert.ToSafeString(data.ConChatRoomMem);
            group.MemberNum = group.Member.Split(';').Length;

            string xmlText = DynamicConvert.ToSafeString(data.ConStrRes2);
            if (xmlText.IsValid())
            {
                GetGroupInfo(xmlText, ref group);
            }

            if (group.GroupOwnerUser.IsInvalid())
            {
                group.GroupOwnerUser = DynamicConvert.ToSafeString(data.ConChatRoomOwner);
            }

            group.HeadPng = GetHeadImage(group.WeChatId);

            return group;
        }

        /// <summary>
        /// 获取好友类型
        /// </summary>
        /// <param name="weChatId"></param>
        /// <param name="certificationFlag"></param>
        /// <param name="imgStatus"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private WeChatFriendTypeEnum GetFriendType(string weChatId, string certificationFlag, string imgStatus, string type)
        {
            if (imgStatus == "1")
            {
                return WeChatFriendTypeEnum.Subscription;
            }

            if (certificationFlag == "24" || weChatId.StartsWith("gh_"))
            {
                return WeChatFriendTypeEnum.Subscription;
            }

            if (type == "4")
            {
                return WeChatFriendTypeEnum.ChatRoom;
            }

            return WeChatFriendTypeEnum.Normal;
        }

        /// <summary>
        /// 获取头像文件地址
        /// </summary>
        /// <param name="wechatID"></param>
        /// <returns></returns>
        private string GetHeadImage(string wechatID)
        {
            var headImageRootPath = Path.Combine(MD5AccountPath, "prcode");

            if (wechatID.IsInvalid() || headImageRootPath.IsInvalid() || !Directory.Exists(headImageRootPath))
            {
                return string.Empty;
            }

            var md5 = CryptographyHelper.MD5FromString(wechatID);
            var path = Path.Combine(headImageRootPath, string.Format("{0}.jpg", md5));

            if (File.Exists(path))
            {
                return path;
            }

            return "";
        }

        private void GetFriendInfo(string xmlText, ref WeChatFriendShow friendInfo)
        {
            if (xmlText.IsInvalid())
            {
                return;
            }
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml("<Root>" + xmlText + "</Root>");
            }
            catch
            {
                return;
            }
            friendInfo.HeadUrl = GetXmlNodeValueByKey(xml, "HeadImgHDUrl");
            //friendInfo.WeiBoAddr = string.Format("{0}({1})", GetXmlNodeValueByKey(xml, "weibonickname"), GetXmlNodeValueByKey(xml, "weiboaddress"));
            friendInfo.Address = string.Format("{0}-{1}-{2}", GetXmlNodeValueByKey(xml, "country"), GetXmlNodeValueByKey(xml, "province"), GetXmlNodeValueByKey(xml, "city"));
            //friendInfo.QQ = GetXmlNodeValueByKey(xml, "qquin");
            //friendInfo.QQNick = GetXmlNodeValueByKey(xml, "qqnickname");
        }

        private void GetGroupInfo(string xmlText, ref WeChatGroupShow groupInfo)
        {
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml("<Root>" + xmlText + "</Root>");
            }
            catch
            {
                return;
            }
            groupInfo.GroupOwnerUser = GetXmlNodeValueByKey(xml, "owner");
        }

        private string GetXmlNodeValueByKey(XmlNode xml, string xpath)
        {
            try
            {
                var node = xml.SelectSingleNode("/Root/" + xpath);
                return node != null ? node.InnerText : "";
            }
            catch
            {
                return string.Empty;
            }
        }

        private void GetdbContactRemark(byte[] data, ref DynamicEx friend)
        {
            try
            {
                int index = 0;

                if (data[index++] != 0x0A)
                {
                    return;
                }

                int length = data[index++];
                if (0 != length)
                {//昵称NickName
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("NickName", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x12)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//修改后微信号Alias
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Alias", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x1A)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//备注RemarkName
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("RemarkName", res);
                }
                index += length;
            }
            catch
            {
            }
        }

        private void GetdbContactProfile(byte[] data, ref DynamicEx friend)
        {
            try
            {
                int index = 0;
                while (index < data.Length && data[index] != 0x12)
                {
                    index++;
                }

                if (index >= data.Length || data[index++] != 0x12)
                {
                    return;
                }

                int length = data[index++];
                if (0 != length)
                {//国家Country
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Country", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x1a)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//省Province
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Province", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x22)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//市City
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("City", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x2a)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//个性签名Signature
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Signature", res);
                }
                index += length;
            }
            catch
            {
            }
        }

        #endregion

        /// <summary>
        /// 加载好友的聊天信息
        /// </summary>
        /// <param name="friend"></param>
        private TreeNode LoadFriendMessage(WeChatFriendShow friend)
        {
            var msgNode = new TreeNode()
            {
                Text = friend.ShowName,
                Type = typeof(MessageCore),
                DataState = friend.DataState,
                Items = new DataItems<MessageCore>(DbFilePath)
            };

            bool hasRows = false;

            var accountName = WeChatAccountShowName;
            var friendName = friend.ShowName;
            var friendMd5 = CryptographyHelper.MD5FromString(friend.WeChatId);

            #region 从主数据库的Chat_xxxx表获取聊天记录

            var msgTableName = string.Format("Chat_{0}", friendMd5);
            var isExitMsgTable = MainContext.ExistTable(msgTableName);
            if (isExitMsgTable)
            {
                MainContext.UsingSafeConnection(string.Format("SELECT * FROM {0}", msgTableName), r =>
                 {
                     dynamic msg;
                     MessageCore message;

                     if (r.HasRows)
                     {
                         hasRows = true;
                     }
                     while (r.Read())
                     {
                         try
                         {
                             msg = r.ToDynamic();
                             message = new MessageCore();

                             message.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(msg.XLY_DataType, EnumDataState.Normal);
                             message.Date = DynamicConvert.ToSafeDateTime(msg.CreateTime);

                             if (DynamicConvert.ToSafeString(msg.Des) == "0")
                             {
                                 message.SendState = EnumSendState.Send;
                                 message.SenderName = accountName;
                                 message.SenderImage = WeChatAccount.HeadPng;
                                 message.Receiver = friendName;
                             }
                             else
                             {
                                 message.SendState = EnumSendState.Receive;
                                 message.SenderName = friendName;
                                 message.SenderImage = friend.HeadPng;
                                 message.Receiver = accountName;
                             }

                             GetMessageType(DynamicConvert.ToSafeString(msg.Type), message);

                             GetMessageContent(msg, ref message, friendMd5);

                             if (message.Type == EnumColumnType.System)
                             {
                                 message.SendState = EnumSendState.Receive;
                                 message.SenderName = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
                                 message.Receiver = accountName;
                             }

                             msgNode.Items.Add(message);
                         }
                         catch { }
                     }
                 });
            }

            #endregion

            #region 从fts数据库获取聊天记录

            if (null != FtsContext)
            {
                string sql = "";
                if (!isExitMsgTable)
                {
                    sql = string.Format(@"SELECT
                                    	msg.c2CreateTime,
                                    	msg.c3Message
                                    FROM
                                    	fts_message_content msg,
                                        fts_username_id user
                                    WHERE
                                    	user.UsrName = '{0}'
                                    AND msg.c0usernameid = user.usernameid", friend.WeChatId);
                }
                else
                {
                    sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                    SELECT
                                    	msg.c2CreateTime,
                                    	msg.c3Message
                                    FROM
                                    	fts_message_content msg,
                                        fts_username_id user
                                    WHERE
                                        msg.c0usernameid = user.usernameid
                                    AND	user.UsrName = '{1}'
                                    AND msg.c1MesLocalID NOT IN (SELECT MesLocalID FROM MsgDB.{2} WHERE MesLocalID NOTNULL)",
                                        MainContext.DbFilePath, friend.WeChatId, msgTableName);
                }

                FtsContext.UsingSafeConnection(sql, r =>
                 {
                     MessageCore mc;
                     dynamic msg;

                     if (r.HasRows)
                     {
                         hasRows = true;
                     }
                     while (r.Read())
                     {
                         msg = r.ToDynamic();

                         mc = new MessageCore();
                         mc.DataState = EnumDataState.Deleted;
                         mc.Content = DynamicConvert.ToSafeString(msg.c3Message);
                         mc.Date = DynamicConvert.ToSafeDateTime(msg.c2CreateTime);

                         msgNode.Items.Add(mc);
                     }
                 });
            }

            #endregion

            return hasRows ? msgNode : null;
        }

        /// <summary>
        /// 加载群的聊天信息
        /// </summary>
        /// <param name="msg"></param>
        private TreeNode LoadGroupMessage(WeChatGroupShow group)
        {
            var msgNode = new TreeNode()
            {
                Text = group.ShowName,
                Type = typeof(MessageCore),
                DataState = group.DataState,
                Items = new DataItems<MessageCore>(DbFilePath)
            };

            bool hasRows = false;

            var accountName = WeChatAccountShowName;
            var groupName = group.ShowName;
            var groupMd5 = CryptographyHelper.MD5FromString(group.WeChatId);

            #region 从主数据库的Chat_xxxx表获取聊天记录

            var msgTableName = string.Format("Chat_{0}", groupMd5);
            var isExitMsgTable = MainContext.ExistTable(msgTableName);
            if (isExitMsgTable)
            {
                MainContext.UsingSafeConnection(string.Format("SELECT * FROM {0}", msgTableName), r =>
                {
                    dynamic msg;
                    MessageCore message;

                    if (r.HasRows)
                    {
                        hasRows = true;
                    }
                    while (r.Read())
                    {
                        try
                        {
                            msg = r.ToDynamic();
                            message = new MessageCore();

                            message.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(msg.XLY_DataType, EnumDataState.Normal);
                            message.Date = DynamicConvert.ToSafeDateTime(msg.CreateTime);

                            if (DynamicConvert.ToSafeString(msg.Des) == "0")
                            {
                                message.SendState = EnumSendState.Send;
                                message.SenderName = accountName;
                                message.SenderImage = WeChatAccount.HeadPng;
                                message.Receiver = groupName;
                            }
                            else
                            {
                                message.SendState = EnumSendState.Receive;
                                message.Receiver = accountName;

                                var data = GetGroupContent(ref msg);
                                var friend = LsAllFriends.FirstOrDefault(o => o.WeChatId == data.Key);
                                if (null == friend)
                                {
                                    message.SenderName = data.Key;
                                }
                                else
                                {
                                    message.SenderName = friend.ShowName;
                                }
                            }

                            GetMessageType(DynamicConvert.ToSafeString(msg.Type), message);

                            GetMessageContent(msg, ref message, groupMd5);

                            if (message.Type == EnumColumnType.System)
                            {
                                message.SendState = EnumSendState.Receive;
                                message.SenderName = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
                                message.Receiver = accountName;
                            }

                            msgNode.Items.Add(message);
                        }
                        catch { }
                    }
                });
            }

            #endregion

            #region 从fts数据库获取聊天记录

            if (null != FtsContext)
            {
                string sql = "";
                if (!isExitMsgTable)
                {
                    sql = string.Format(@"SELECT
                                    	msg.c2CreateTime,
                                    	msg.c3Message
                                    FROM
                                    	fts_message_content msg,
                                        fts_username_id user
                                    WHERE
                                    	user.UsrName = '{0}'
                                    AND msg.c0usernameid = user.usernameid", group.WeChatId);
                }
                else
                {
                    sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                    SELECT
                                    	msg.c2CreateTime,
                                    	msg.c3Message
                                    FROM
                                    	fts_message_content msg,
                                        fts_username_id user
                                    WHERE
                                        msg.c0usernameid = user.usernameid
                                    AND	user.UsrName = '{1}'
                                    AND msg.c1MesLocalID NOT IN (SELECT MesLocalID FROM MsgDB.{2} WHERE MesLocalID NOTNULL)",
                                        MainContext.DbFilePath, group.WeChatId, msgTableName);
                }

                FtsContext.UsingSafeConnection(sql, r =>
                {
                    MessageCore mc;
                    dynamic msg;

                    if (r.HasRows)
                    {
                        hasRows = true;
                    }
                    while (r.Read())
                    {
                        msg = r.ToDynamic();

                        mc = new MessageCore();
                        mc.DataState = EnumDataState.Deleted;
                        mc.Content = DynamicConvert.ToSafeString(msg.c3Message);
                        mc.Date = DynamicConvert.ToSafeDateTime(msg.c2CreateTime);

                        msgNode.Items.Add(mc);
                    }
                });
            }

            #endregion

            return hasRows ? msgNode : null;
        }

        #region 解析消息

        /// <summary>
        /// 解析群聊天记录
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private KeyValueItem GetGroupContent(ref dynamic msg)
        {
            string content = DynamicConvert.ToSafeString(msg.Message);

            var data = new KeyValueItem();
            if (content.IndexOf(':') > -1)
            {
                data.Key = content.Substring(0, content.IndexOf(':'));
                data.Value = content.Substring(content.IndexOf(':') + 1).TrimStart('\n');

                msg.Message = data.Value;
            }
            else if (Regex.IsMatch(content, "fromusername=\"(\\S+)\""))
            {
                data.Key = Regex.Match(content, "fromusername=\"(\\S+)\"").Groups[1].Value;
                data.Value = content;
            }
            else
            {
                data.Value = content;
                data.Key = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
            }
            return data;
        }

        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private void GetMessageType(string type, MessageCore msg)
        {
            switch (type)
            {
                case "1":
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                    break;
                case "3":
                    msg.Type = EnumColumnType.Image;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Image);
                    break;
                case "34":
                    msg.Type = EnumColumnType.Audio;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Audio);
                    break;
                case "48":
                    msg.Type = EnumColumnType.Location;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Location);
                    break;
                case "62":
                case "43":
                    msg.Type = EnumColumnType.Video;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Video);
                    break;
                case "47":
                    msg.Type = EnumColumnType.Emoji;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Emoji);
                    break;
                case "42":
                    msg.Type = EnumColumnType.Card;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Card);
                    break;
                case "52":
                    msg.Type = EnumColumnType.VideoChat;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_VideoChat);
                    break;
                case "50":
                    msg.Type = EnumColumnType.AudioCall;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_VideoCall);
                    break;
                case "10000":
                    msg.Type = EnumColumnType.System;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
                    break;
                case "49":
                    msg.Type = EnumColumnType.HTML;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                    break;
                default:
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                    break;
            }
        }

        /// <summary>
        /// 获取消息内容
        /// </summary>
        private void GetMessageContent(dynamic mess, ref MessageCore msg, string friendMd5)
        {
            switch (msg.Type)
            {
                case EnumColumnType.Audio:
                    msg.Content = MessageToVoice.MessageConvert(mess, GetMediaPath("Audio", friendMd5));
                    return;
                case EnumColumnType.Image:
                    msg.Content = MessageToImage.MessageConvert(mess, GetMediaPath("Img", friendMd5));
                    return;
                case EnumColumnType.Video:
                    msg.Content = MessageToVideo.MessageConvert(mess, GetMediaPath("Video", friendMd5));
                    return;
                case EnumColumnType.Emoji:
                    msg.Content = MessageToEmoji(mess);
                    return;
                case EnumColumnType.Location:
                    msg.Content = MessageToLocation(mess);
                    return;
                case EnumColumnType.AudioCall:
                    msg.Content = MessageToAudioCall(mess);
                    return;
                case EnumColumnType.HTML:
                    msg.Content = MessageToHTML(mess, ref msg);
                    return;
                case EnumColumnType.System:
                    msg.Content = MessageToSystem(mess, ref msg);
                    return;
            }

            msg.Content = DynamicConvert.ToSafeString(mess.Message);
        }

        /// <summary>
        /// 获取图片、语音路径
        /// </summary>
        /// <param name="type">图片？语音？</param>
        /// <param name="friendMd5">那个好友文件夹</param>
        /// <returns></returns>
        private string GetMediaPath(string type, string friendMd5)
        {
            return Path.Combine(MD5AccountPath, type, friendMd5);
        }

        private Regex EmojiRegex = new Regex(" md5=\"([0-9a-z]+)\"");

        private string MessageToEmoji(dynamic mess)
        {
            string msg = DynamicConvert.ToSafeString(mess.Message);
            var res = EmojiRegex.Match(msg);
            if (res.Success)
            {
                return res.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        private string MessageToSystem(dynamic mess, ref MessageCore msg)
        {
            string strMessage = DynamicConvert.ToSafeString(mess.Message);

            if (strMessage.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
            {//红包领取信息
                msg.Type = EnumColumnType.WeChatRedPack;
                msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatRedPack);
                strMessage = string.Format("{0}-{1}", LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatRedPack), Regex.Replace(strMessage, @"<[^<>]*>", "").Trim());
            }

            return strMessage;
        }

        private string MessageToHTML(dynamic mess, ref MessageCore msg)
        {
            string strMessage = DynamicConvert.ToSafeString(mess.Message);

            string strType = "";
            string strTitle = "";
            string strDesc = "";
            string strUrl = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLink = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlReader xmlLink = XmlReader.Create(msXmlLink);
                while (xmlLink.Read())
                {
                    if (xmlLink.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLink.Name == "type")
                            strType = xmlLink.ReadString();
                        else if (xmlLink.Name == "title")
                            strTitle = xmlLink.ReadString();
                        else if (xmlLink.Name == "des")
                            strDesc = xmlLink.ReadString();
                        else if (xmlLink.Name == "url")
                            strUrl = xmlLink.ReadString();
                    }
                }
                xmlLink.Close();
                msXmlLink.Close();
            }
            catch
            {
                return strMessage;
            }

            if (strType == "2000")
            {//转账记录
                msg.Type = EnumColumnType.WeChatTransfer;
                msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatTransfer);
                if (strMessage.Contains("<paysubtype>1</paysubtype>"))
                {//转账
                    strMessage = string.Format("{0}-{1}", strTitle, strDesc);
                }
                else if (strMessage.Contains("<paysubtype>3</paysubtype>"))
                {//接收转账
                    strMessage = string.Format("{0}-{1}", strTitle, LanguageHelper.GetString(Languagekeys.PluginWechat_TransferOK));
                }
            }
            else if (strType == "2001")
            {//红包
                msg.Type = EnumColumnType.WeChatRedPack;
                msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatRedPack);
                //发送红包
                strMessage = strDesc;
            }
            //else if(strType == "17")
            //{
            //    //strMessage = "<a style=\"color:#888\"> 实时位置信息... </a>";
            //}
            else
            {
                if (strUrl.Trim().Length == 0)
                    strMessage = string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_HtmlInfoA), strTitle, strDesc);
                else
                    strMessage = string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_HtmlInfoB), strTitle, strDesc, strUrl);
            }

            return strMessage;
        }

        private string MessageToLocation(dynamic mess)
        {
            string strMessage = DynamicConvert.ToSafeString(mess.Message);

            string strX = "";
            string strY = "";
            string strLabel = "";
            string strPoiname = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLoc = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlTextReader xmlLoc = new XmlTextReader(msXmlLoc);
                while (xmlLoc.Read())
                {
                    if (xmlLoc.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLoc.Name == "location")
                        {
                            strX = xmlLoc.GetAttribute("x");
                            strY = xmlLoc.GetAttribute("y");
                            strLabel = xmlLoc.GetAttribute("label");
                            strPoiname = xmlLoc.GetAttribute("poiname");
                            if (strLabel.Length == 0)
                                strLabel = " ";
                            if (strPoiname.Length == 0)
                                strPoiname = " ";
                        }
                    }
                }
                xmlLoc.Close();
                msXmlLoc.Close();
            }
            catch
            {
                return strMessage;
            }

            //return string.Format("<a target=\"_blank\" href=\"http://apis.map.qq.com/uri/v1/marker?" +
            //    "marker=coord:{0},{1};title:{2};addr:{3}\">位置信息</a>", strX, strY, strPoiname, strLabel);

            return string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_LocationInfo), strPoiname, strLabel, strX, strY);
        }

        /// <summary>
        /// 获取视频聊天通话记录显示
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        private string MessageToAudioCall(dynamic mess)
        {
            string message = DynamicConvert.ToSafeString(mess.Message);
            string time = message.Substring(Convert.ToInt32(message.IndexOf("<duration>").ToSafeString()) + 10, 2).Replace("<", "");
            return string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_VideoCallTime), time);
        }

        #endregion

        private void GetMassInfo(string context, out string tolistmd5, out string msg)
        {
            //<msgroot><masssendmsg tolistmd5="d36634839b516dfe464c5f12b13d2f87"><![CDATA[车是群发]]></masssendmsg></msgroot>

            tolistmd5 = string.Empty;
            msg = string.Empty;

            var xdocumet = XElement.Parse(context);

            tolistmd5 = xdocumet.Element("masssendmsg").Attribute("tolistmd5").Value;
            msg = xdocumet.Element("masssendmsg").Value;
        }

        #endregion

    }
}
