/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/17 10:26:08 
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
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS 企业QQ数据解析
    /// </summary>
    internal class IOSEimQQDataParseCoreV1_0
    {
        /// <summary>
        /// IOS 企业QQ数据解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="name">QQ名称 例如 QQ、QQ分身</param>
        /// <param name="datapath">微信数据文件根目录，例如 I:\本地数据\com.tencent.mqq</param>
        public IOSEimQQDataParseCoreV1_0(string savedatadbpath, string name, string datapath)
        {
            DbFilePath = savedatadbpath;
            QQName = name;
            DataFileRootPath = datapath;
        }

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// QQ名称，例如 QQ、QQ分身
        /// </summary>
        private string QQName { get; set; }

        /// <summary>
        ///QQ数据文件根目录，例如 I:\本地数据\com.tencent.mqq
        ///该文件下包含了Documents子文件夹，保存了QQ相关数据和文件
        /// </summary>
        private string DataFileRootPath { get; set; }

        #endregion

        #region 临时属性

        /// <summary>
        /// 当前QQ帐号
        /// </summary>
        private QQAccountShow QQAccount { get; set; }

        /// <summary>
        /// 当前QQ帐号数据库所在文件夹路径
        /// </summary>
        private string QQDbPath { get; set; }

        /// <summary>
        /// 当前QQ帐号多媒体文件所在文件夹路径
        /// </summary>
        private string MediaFilePath { get; set; }

        /// <summary>
        /// 主数据库 QQ.db
        /// </summary>
        private SqliteContext MainContext { get; set; }

        /// <summary>
        /// 主数据库 FTSMsg.db
        /// </summary>
        private SqliteContext FTSMsgContext { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        private List<QQFriendShow> LsAllFriends { get; set; }

        /// <summary>
        /// 群组列表
        /// </summary>
        private List<QQGroupShow> LsAllGroups { get; set; }

        /// <summary>
        /// 讨论组列表
        /// </summary>
        private List<QQDiscussShow> LsAllDiscuss { get; set; }

        /// <summary>
        /// 清除临时属性
        /// 一般用于插件执行完毕后执行
        /// </summary>
        private void ClearCache()
        {
            QQAccount = null;
            QQDbPath = null;
            MediaFilePath = null;

            MainContext?.Dispose();
            MainContext = null;

            FTSMsgContext?.Dispose();
            FTSMsgContext = null;

            LsAllFriends?.Clear();
            LsAllFriends = null;

            LsAllGroups?.Clear();
            LsAllGroups = null;

            LsAllDiscuss?.Clear();
            LsAllDiscuss = null;

        }

        #endregion

        /// <summary>
        /// 解析QQ数据
        /// </summary>
        /// <returns></returns>
        public TreeNode BuildTree()
        {
            TreeNode rootNode = new TreeNode();
            try
            {
                rootNode.Text = QQName;
                rootNode.Type = typeof(QQAccountShow);
                rootNode.Items = new DataItems<QQAccountShow>(DbFilePath);

                foreach (var account in LoadQQAccounts())
                {
                    ClearCache();

                    BuildQQAcountTree(account, rootNode);
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
        /// 构建单个QQ帐号的数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="rootNode"></param>
        private void BuildQQAcountTree(QQAccountShow account, TreeNode rootNode)
        {
            QQAccount = account;
            QQDbPath = Path.Combine(DataFileRootPath, @"Documents\contents\" + QQAccount.QQNumber);
            MediaFilePath = Path.Combine(DataFileRootPath, @"Documents\" + QQAccount.QQNumber);

            //当前账户树节点
            var accountNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = QQAccount.FullName,
                Id = QQAccount.QQNumber,
                Type = typeof(QQAccountShow),
                Items = new DataItems<QQAccountShow>(DbFilePath)
            };
            accountNode.Items.Add(QQAccount);
            rootNode.Items.Add(QQAccount);
            rootNode.TreeNodes.Add(accountNode);

            //数据库恢复
            GetSqliteContext();

            //构建好友分组信息
            BuildFriendGroupTree(accountNode);

            //构建好友聊天记录
            BuildFriendMsgTree(accountNode);

            //构建QQ群信息
            BuildTroopTree(accountNode);

            //构建QQ群聊天记录
            BuildTroopMsgTree(accountNode);

            //构建讨论组信息
            BuildDiscussTree(accountNode);

            //构建讨论组聊天记录
            BuildDiscussMsgTree(accountNode);

            //构建通讯录
            BuildAddrBookTree(accountNode);

            //构建QQ通话记录
            BuildCallRecordTree(accountNode);
        }

        /// <summary>
        /// 构建好友分组信息
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildFriendGroupTree(TreeNode accountNode)
        {
            var friendNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "好友列表",
                Type = typeof(QQFriendShow),
                Items = new DataItems<QQFriendShow>(DbFilePath),
                Id = QQAccount.QQNumber
            };

            accountNode.TreeNodes.Add(friendNode);

            LsAllFriends = new List<QQFriendShow>();

            //1.从QQ.db的tb_userSummary表获取数据
            MainContext.UsingSafeConnection("SELECT * FROM tb_userSummary", r =>
             {
                 dynamic summ;
                 QQFriendShow friendInfo;

                 while (r.Read())
                 {
                     summ = r.ToDynamic();
                     friendInfo = new QQFriendShow();

                     friendInfo.QQNumber = DynamicConvert.ToSafeString(summ.uin);
                     friendInfo.Nick = DynamicConvert.ToSafeString(summ.nick);
                     friendInfo.Remark = DynamicConvert.ToSafeString(summ.remark);
                     friendInfo.Alias = DynamicConvert.ToSafeString(summ.showName);
                     friendInfo.Sex = DynamicConvert.ToSafeString(summ.gender) == "1" ? EnumSex.Female : EnumSex.Male;
                     friendInfo.Age = DynamicConvert.ToSafeInt(summ.age);
                     friendInfo.Address = string.Format("{0}-{1}-{2}",
                         DynamicConvert.ToSafeString(summ.country), DynamicConvert.ToSafeString(summ.province), DynamicConvert.ToSafeString(summ.city));
                     friendInfo.BrithDay = ConvertIOSQQDateTime(DynamicConvert.ToSafeString(summ.birth));
                     friendInfo.BindPhone = DynamicConvert.ToSafeString(summ.bindPhoneInfo);
                     friendInfo.Company = DynamicConvert.ToSafeString(summ.company);
                     friendInfo.School = DynamicConvert.ToSafeString(summ.school);
                     friendInfo.ContactName = DynamicConvert.ToSafeString(summ.contactName);
                     friendInfo.ContactPhone = DynamicConvert.ToSafeString(summ.mobileNum);
                     friendInfo.Feed = DynamicConvert.ToSafeString(summ.qzoneFeedsDesc);
                     friendInfo.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(summ.XLY_DataType, EnumDataState.Normal);

                     LsAllFriends.Add(friendInfo);
                 }
             });

            //2.从消息表获取QQ号码
            var listMsgTable = MainContext.Find("select * from sqlite_master where type = 'table' and name like 'tb_c2cMsg_%'");
            string qqNumber;
            QQFriendShow friend;
            foreach (var msgTable in listMsgTable)
            {
                qqNumber = DynamicConvert.ToSafeString(msgTable.name);
                qqNumber = qqNumber.TrimStart("tb_c2cMsg_");
                if (LsAllFriends.Any(f => f.QQNumber == qqNumber))
                {
                    continue;
                }

                friend = new QQFriendShow();
                friend.DataState = EnumDataState.Normal;
                friend.QQNumber = qqNumber;
                switch (qqNumber)
                {
                    case "2711679534":
                        friend.Nick = "QQ钱包";
                        break;
                    case "2010741172":
                        friend.Nick = "QQ邮箱";
                        break;
                    case "1344242394":
                        friend.Nick = "QQ红包";
                        break;
                }

                LsAllFriends.Add(friend);
            }

            foreach (var friendF in LsAllFriends)
            {
                friendNode.Items.Add(friendF);
            }
        }

        /// <summary>
        /// 构建好友聊天记录
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildFriendMsgTree(TreeNode accountNode)
        {
            var msgRootNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "好友消息",
            };

            accountNode.TreeNodes.Add(msgRootNode);

            TreeNode node;

            foreach (var friend in LsAllFriends)
            {
                node = LoadFriendMsg(friend);
                if (null != node)
                {
                    msgRootNode.TreeNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 构建QQ群
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildTroopTree(TreeNode accountNode)
        {
            LsAllGroups = new List<QQGroupShow>();

            var troopNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "群成员",
                Type = typeof(QQGroupShow),
                Items = new DataItems<QQGroupShow>(DbFilePath),
            };

            accountNode.TreeNodes.Add(troopNode);

            MainContext.UsingSafeConnection("SELECT * FROM tb_troop", r =>
             {
                 dynamic groupDy;
                 QQGroupShow groupInfo;

                 while (r.Read())
                 {
                     groupDy = r.ToDynamic();
                     groupInfo = new QQGroupShow();

                     groupInfo.DataState = DynamicConvert.ToEnumByValue(groupDy.XLY_DataType, EnumDataState.None);
                     groupInfo.Name = DynamicConvert.ToSafeString(groupDy.GroupName);
                     groupInfo.GroupId = DynamicConvert.ToSafeString(groupDy.groupid);
                     groupInfo.QQNumber = DynamicConvert.ToSafeString(groupDy.groupcode);
                     groupInfo.MemberCount = DynamicConvert.ToSafeInt(groupDy.groupMemNum);

                     var groupMemTree = new TreeNode
                     {
                         DataState = EnumDataState.Normal,
                         Text = groupInfo.FullName,
                         Type = typeof(QQGroupMemberShow),
                         Items = new DataItems<QQGroupMemberShow>(DbFilePath),
                     };

                     LsAllGroups.Add(groupInfo);
                     groupMemTree.Items.Add(groupInfo);
                     troopNode.TreeNodes.Add(groupMemTree);

                     MainContext.UsingSafeConnection(string.Format("SELECT * FROM tb_TroopMem WHERE GroupCode = '{0}'", groupInfo.QQNumber), rd =>
                      {
                          dynamic memberDy;
                          QQGroupMemberShow memberInfo;

                          while (rd.Read())
                          {
                              memberDy = rd.ToDynamic();
                              memberInfo = new QQGroupMemberShow();

                              memberInfo.DataState = DynamicConvert.ToEnumByValue(memberDy.XLY_DataType, EnumDataState.None);
                              memberInfo.QQNumber = DynamicConvert.ToSafeString(memberDy.MemUin);
                              memberInfo.Nick = DynamicConvert.ToSafeString(memberDy.Nick);
                              memberInfo.SpecialTitle = DynamicConvert.ToSafeString(memberDy.SpecialTitle);
                              memberInfo.Sex = DynamicConvert.ToSafeString(memberDy.Gender) == "1" ? EnumSex.Female : EnumSex.Male;
                              memberInfo.Age = DynamicConvert.ToSafeInt(memberDy.Age);
                              memberInfo.JoinTime = DynamicConvert.ToSafeDateTime(memberDy.JoinTime);
                              memberInfo.LastSpeakTime = DynamicConvert.ToSafeDateTime(memberDy.LastSpeakTime);

                              groupMemTree.Items.Add(memberInfo);
                          }
                      });
                 }
             });
        }

        /// <summary>
        /// 构建QQ群聊天记录
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildTroopMsgTree(TreeNode accountNode)
        {
            var troopMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "群消息",
            };

            accountNode.TreeNodes.Add(troopMsgNode);

            TreeNode node;

            foreach (var group in LsAllGroups)
            {
                node = LoadGroupMsg(group);
                if (null != node)
                {
                    troopMsgNode.TreeNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 构建讨论组
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildDiscussTree(TreeNode accountNode)
        {
            LsAllDiscuss = new List<QQDiscussShow>();

            var discussNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "讨论组成员",
                Type = typeof(QQDiscussShow),
                Items = new DataItems<QQDiscussShow>(DbFilePath),
            };

            accountNode.TreeNodes.Add(discussNode);

            MainContext.UsingSafeConnection("SELECT * FROM tb_discussGrp_list", r =>
            {
                dynamic disDy;
                QQDiscussShow disInfo;

                while (r.Read())
                {
                    disDy = r.ToDynamic();
                    disInfo = new QQDiscussShow();

                    disInfo.DataState = DynamicConvert.ToEnumByValue(disDy.XLY_DataType, EnumDataState.None);
                    disInfo.Name = DynamicConvert.ToSafeString(disDy.name);
                    disInfo.QQNumber = DynamicConvert.ToSafeString(disDy.uin);
                    disInfo.Creator = DynamicConvert.ToSafeString(disDy.owner_uin);
                    disInfo.CreateTime = DynamicConvert.ToSafeDateTime(disDy.create_time);
                    disInfo.MemberCount = DynamicConvert.ToSafeInt(disDy.member_num);

                    var disMemTree = new TreeNode
                    {
                        DataState = EnumDataState.Normal,
                        Text = disInfo.FullName,
                        Type = typeof(QQGroupMemberShow),
                        Items = new DataItems<QQGroupMemberShow>(DbFilePath),
                    };

                    LsAllDiscuss.Add(disInfo);
                    disMemTree.Items.Add(disInfo);
                    discussNode.TreeNodes.Add(disMemTree);

                    MainContext.UsingSafeConnection(string.Format("SELECT * FROM tb_discussGrp_member WHERE dis_uin = '{0}'", disInfo.QQNumber), rd =>
                    {
                        dynamic memberDy;
                        QQGroupMemberShow memberInfo;

                        while (rd.Read())
                        {
                            memberDy = rd.ToDynamic();
                            memberInfo = new QQGroupMemberShow();

                            memberInfo.DataState = DynamicConvert.ToEnumByValue(memberDy.XLY_DataType, EnumDataState.None);
                            memberInfo.QQNumber = DynamicConvert.ToSafeString(memberDy.uin);
                            memberInfo.Nick = DynamicConvert.ToSafeString(memberDy.interemark);
                            memberInfo.JoinTime = DynamicConvert.ToSafeDateTime(memberDy.join_time);

                            disMemTree.Items.Add(memberInfo);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 构建讨论组聊天记录
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildDiscussMsgTree(TreeNode accountNode)
        {
            var disMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "讨论组消息",
            };

            accountNode.TreeNodes.Add(disMsgNode);

            TreeNode node;

            foreach (var dis in LsAllDiscuss)
            {
                node = LoadDiscussMsg(dis);
                if (null != node)
                {
                    disMsgNode.TreeNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 加载通讯录
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildAddrBookTree(TreeNode accountNode)
        {
            var treeNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "通讯录",
                Type = typeof(QQAddrBookFriendShow),
                Items = new DataItems<QQAddrBookFriendShow>(DbFilePath),
            };

            accountNode.TreeNodes.Add(treeNode);

            var plistFile = Path.Combine(QQDbPath, @"Friends\RecommendedPeople.data");
            if (!FileHelper.IsValid(plistFile))
            {
                return;
            }

            var jData = PListToJsonHelper.PlistToJson(plistFile) as JArray;
            foreach (var jPerson in jData)
            {
                if (null == jPerson["_cModel"])
                {
                    continue;
                }

                QQAddrBookFriendShow show = new QQAddrBookFriendShow();
                show.DataState = EnumDataState.Normal;
                show.Name = jPerson["_cModel"]["Contact_name"].ToSafeString();
                show.QQNumber = jPerson["_cModel"]["Contact_uin"].ToSafeString();
                show.NickName = jPerson["_cModel"]["Contact_nickName"].ToSafeString();
                show.PhoneCode = jPerson["_cModel"]["Contact_phoneCode"].ToSafeString();

                treeNode.Items.Add(show);
            }
        }

        /// <summary>
        /// 加载QQ通话记录
        /// </summary>
        /// <param name="accountNode"></param>
        private void BuildCallRecordTree(TreeNode accountNode)
        {
            var treeNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "通话记录",
                Type = typeof(QQCallRecordShow),
                Items = new DataItems<QQCallRecordShow>(DbFilePath),
            };

            accountNode.TreeNodes.Add(treeNode);

            var dbFile = Path.Combine(QQDbPath, "QQ_Mix.db");
            if (!FileHelper.IsValid(dbFile))
            {
                return;
            }

            using (var dbContent = new SqliteContext(dbFile))
            {
                dbContent.UsingSafeConnection("SELECT uin,nickname,time,duration,discussGroupUin,msgcontent,phonecode FROM tb_callRecord ORDER BY time DESC", r =>
                 {
                     dynamic call;
                     QQCallRecordShow record;

                     while (r.Read())
                     {
                         call = r.ToDynamic();

                         record = new QQCallRecordShow() { DataState = EnumDataState.Normal };
                         record.Uin = DynamicConvert.ToSafeString(call.uin);
                         record.DiscussGroupUin = DynamicConvert.ToSafeString(call.discussGroupUin);
                         record.NickName = DynamicConvert.ToSafeString(call.nickname);
                         record.MsgContent = DynamicConvert.ToSafeString(call.msgcontent);
                         record.PhoneCode = DynamicConvert.ToSafeString(call.phonecode);
                         record.Duration = DynamicConvert.ToSafeString(call.duration);
                         record.Time = DynamicConvert.ToSafeFromUnixTime(call.time, 1);
                         if (record.DiscussGroupUin.IsValid() && record.DiscussGroupUin != "0")
                         {
                             record.CallType = "群组通话";
                         }
                         else
                         {
                             record.CallType = "个人通话";
                         }

                         //数据完善，剔除无效数据显示
                         if ("0" == record.Uin.Trim())
                         {
                             record.Uin = string.Empty;
                         }
                         if ("0" == record.DiscussGroupUin.Trim())
                         {
                             record.DiscussGroupUin = string.Empty;
                         }
                         if ("+0" == record.PhoneCode.Trim() || "+(null)" == record.PhoneCode.Trim())
                         {
                             record.PhoneCode = string.Empty;
                         }

                         treeNode.Items.Add(record);
                     }
                 });
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取QQ帐号列表
        /// </summary>
        /// <returns></returns>
        private List<QQAccountShow> LoadQQAccounts()
        {
            var accountPath = Path.Combine(DataFileRootPath, @"Documents\contents\QQAccountsManager");
            var jDataArr = PListToJsonHelper.PlistToJson(accountPath) as JArray;

            List<QQAccountShow> listAccounts = new List<QQAccountShow>();
            QQAccountShow account;
            foreach (var jData in jDataArr)
            {
                account = new QQAccountShow();

                account.DataState = EnumDataState.Normal;
                account.QQNumber = jData["_uin"].ToSafeString();
                account.Nick = jData["_nick"].ToSafeString();
                account.Signature = jData["_sig"].ToSafeString();
                account.Address = string.Format("{0}-{1}-{2}", jData["_sCountry"].ToSafeString(), jData["_sProvince"].ToSafeString(), jData["_sCity"].ToSafeString());
                account.BrithDay = ConvertIOSQQDateTime(jData["_birs"].ToSafeString());

                if ("1" == jData["_sex"].ToSafeString() || "1" == jData["_profileSex"].ToSafeString())
                {
                    account.Sex = EnumSex.Male;
                }
                else
                {
                    account.Sex = EnumSex.Female;
                }

                try
                {
                    var dbFile = Path.Combine(DataFileRootPath, @"Documents\contents\" + account.QQNumber, "QQ.db");
                    if (FileHelper.IsValid(dbFile))
                    {
                        using (var db = new SqliteContext(dbFile))
                        {
                            var data = db.Find(string.Format("SELECT * FROM tb_userSummary WHERE uin = '{0}'", account.QQNumber)).FirstOrDefault();

                            account.Age = DynamicConvert.ToSafeInt(data.age);
                            account.PhoneNumber = DynamicConvert.ToSafeString(data.bindPhoneInfo);
                            account.Company = DynamicConvert.ToSafeString(data.company);
                            account.School = DynamicConvert.ToSafeString(data.school);
                        }
                    }
                }
                catch { }

                listAccounts.Add(account);
            }

            return listAccounts;
        }

        /// <summary>
        /// 数据库恢复
        /// </summary>
        private void GetSqliteContext()
        {
            var mainDbFile = Path.Combine(QQDbPath, "QQ.db");

            string[] baseTables = { "tb_File", "tb_recentC2CMsg", "tb_userSummary", "tb_troop", "tb_TroopMem", "tb_SecSession", "tb_SecMsg", "tb_discussGrp_list", "tb_discussGrp_member" };
            var allTables = SqliteRecoveryHelper.ButtomGetAllTables(mainDbFile);
            var recoveryTables = string.Join(",", allTables.Where(table => baseTables.Contains(table) || table.StartsWith("tb_TroopMsg_") || table.StartsWith("tb_c2cMsg_") || table.StartsWith("tb_discussGrp_") || table.StartsWith("tb_SecMsg_")));

            var revMainDbFile = SqliteRecoveryHelper.DataRecovery(mainDbFile, @"chalib\IOS_QQ\QQ5.1.db.charactor", recoveryTables);
            MainContext = new SqliteContext(revMainDbFile);

            var ftsDbFile = Path.Combine(QQDbPath, "FTSMsg.db");
            if (FileHelper.IsValid(ftsDbFile))
            {
                Dictionary<string, string> dicIndex = new Dictionary<string, string>();
                dicIndex.Add("tb_Index_c2cMsg_content", "c1uin");
                dicIndex.Add("tb_Index_TroopMsg_content", "c8conversationUin");
                dicIndex.Add("tb_Index_discussGrp_content", "c8conversationUin");

                FTSMsgContext = new SqliteContext(SqliteRecoveryHelper.DataRecovery(ftsDbFile, @"chalib\IOS_QQ\FTSMsg.db.charactor",
                                                "tb_Index_c2cMsg_content,tb_Index_discussGrp_content,tb_Index_TroopMsg_content", "", "", true, dicIndex));
            }
            else
            {
                FTSMsgContext = null;
            }
        }

        /// <summary>
        /// QQ时间戳转换
        /// </summary>
        /// <param name="numberStr">例如 130484251 </param>
        /// <returns></returns>
        private DateTime? ConvertIOSQQDateTime(string numberStr)
        {
            try
            {
                string hexStr = int.Parse(numberStr).ToString("X8");

                if (hexStr.Length == 8)
                {
                    int year = int.Parse(hexStr.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                    int month = int.Parse(hexStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    int day = int.Parse(hexStr.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

                    return new DateTime(year, month, day);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解析QQ好友聊天记录
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        private TreeNode LoadFriendMsg(QQFriendShow friend)
        {
            var friendMsgNode = new TreeNode
            {
                DataState = friend.DataState,
                Text = friend.FullName,
                Type = typeof(MessageCore),
                Items = new DataItems<MessageCore>(DbFilePath),
            };

            bool bAll = true;

            var accountName = QQAccount.FullName;
            var friendName = friend.FullName;

            #region 从主数据库的Chat_xxxx表获取聊天记录

            var msgTableName = string.Format("tb_c2cMsg_{0}", friend.QQNumber);
            var isExitMsgTable = MainContext.ExistTable(msgTableName);
            if (isExitMsgTable)
            {
                MainContext.UsingSafeConnection(string.Format("SELECT * FROM {0}", msgTableName), r =>
                {
                    if (r.Read())
                    {
                        bAll = false;
                        do
                        {
                            CreateFriendMessageCore(friend.QQNumber, r.ToDynamic(), friendMsgNode, accountName, friendName);
                        } while (r.Read());
                    }
                });
            }

            #endregion

            #region 从主数据库的tb_recentC2CMsg表获取删除聊天记录

            MainContext.UsingSafeConnection(string.Format("SELECT * FROM tb_recentC2CMsg where uin = '{0}' and XLY_DataType = 1", friend.QQNumber), r =>
            {
                while (r.Read())
                {
                    CreateFriendMessageCore(friend.QQNumber, r.ToDynamic(), friendMsgNode, accountName, friendName);
                }
            });

            #endregion

            #region 从FTSMsg.db获取删除聊天记录

            if (null != FTSMsgContext)
            {
                string sql = "";
                if (bAll)
                {
                    sql = string.Format(@"SELECT
                                          	c2time,
                                          	c4flag,
                                          	CAST (c7content AS varchar) AS c7content
                                          FROM
                                          	tb_Index_c2cMsg_content
                                          WHERE
                                          	c1uin = '{0}'
                                          ORDER BY
                                          	c2time", friend.QQNumber);
                }
                else
                {
                    sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                        SELECT
                                        	c2time,
                                        	c4flag,
                                        	CAST (c7content AS varchar) AS c7content
                                        FROM
                                        	tb_Index_c2cMsg_content
                                        WHERE
                                        	c1uin = '{1}'
                                        AND c0msgId NOT IN (SELECT msgId FROM MsgDB.tb_c2cMsg_{1} WHERE msgId NOTNULL)
                                        ORDER BY
                                        	c2time",
                                        MainContext.DbFilePath, friend.QQNumber);
                }

                FTSMsgContext.UsingSafeConnection(new SQLiteString(sql), r =>
                {
                    while (r.Read())
                    {
                        friendMsgNode.Items.Add(CreateFriendMessageCoreFromFTSMsg(r.ToDynamic(), accountName, friendName));
                    }
                });
            }

            #endregion

            return friendMsgNode;
        }

        /// <summary>
        /// 解析QQ群聊天记录
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private TreeNode LoadGroupMsg(QQGroupShow group)
        {
            var groupMsgNode = new TreeNode
            {
                DataState = group.DataState,
                Text = group.FullName,
                Type = typeof(MessageCore),
                Items = new DataItems<MessageCore>(DbFilePath),
            };

            bool bAll = true;

            var accountName = QQAccount.FullName;
            var groupName = group.FullName;

            #region 从主数据库的Chat_xxxx表获取聊天记录

            var msgTableName = string.Format("tb_TroopMsg_{0}", group.QQNumber);
            var isExitMsgTable = MainContext.ExistTable(msgTableName);
            if (isExitMsgTable)
            {
                MainContext.UsingSafeConnection(string.Format("SELECT * FROM {0}", msgTableName), r =>
                {
                    if (r.Read())
                    {
                        bAll = false;
                        do
                        {
                            CreateGroupMessageCore(group.QQNumber, r.ToDynamic(), groupMsgNode, accountName, groupName);
                        } while (r.Read());
                    }
                });
            }

            #endregion

            #region 从FTSMsg.db获取删除聊天记录

            if (null != FTSMsgContext)
            {
                string sql = "";
                if (bAll)
                {
                    sql = string.Format(@"SELECT
                                          	c1uin,
                                          	c2time,
                                            CAST (c5nickName AS varchar) AS c5nickName,
                                          	CAST (c7content AS varchar) AS c7content
                                          FROM
                                          	tb_Index_TroopMsg_content
                                          WHERE
                                          	c8conversationUin = '{0}'
                                          ORDER BY
                                          	c2time", group.QQNumber);
                }
                else
                {
                    sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                        SELECT
                                          	c1uin,
                                          	c2time,
                                            CAST (c5nickName AS varchar) AS c5nickName,
                                          	CAST (c7content AS varchar) AS c7content
                                        FROM
                                          	tb_Index_TroopMsg_content
                                        WHERE
                                          	c8conversationUin = '{1}'
                                        AND c0msgId NOT IN (SELECT MsgId FROM MsgDB.tb_TroopMsg_{1} WHERE MsgId NOTNULL)
                                        ORDER BY
                                        	c2time",
                                        MainContext.DbFilePath, group.QQNumber);
                }

                FTSMsgContext.UsingSafeConnection(new SQLiteString(sql), r =>
                {
                    while (r.Read())
                    {
                        groupMsgNode.Items.Add(CreateTroopMessageCoreFromFTSMsg(r.ToDynamic(), accountName, groupName));
                    }
                });
            }

            #endregion

            return groupMsgNode;
        }

        /// <summary>
        /// 解析QQ讨论组聊天记录
        /// </summary>
        /// <param name="discuss"></param>
        /// <returns></returns>
        private TreeNode LoadDiscussMsg(QQDiscussShow discuss)
        {
            var disMsgNode = new TreeNode
            {
                DataState = discuss.DataState,
                Text = discuss.FullName,
                Type = typeof(MessageCore),
                Items = new DataItems<MessageCore>(DbFilePath),
            };

            bool bAll = true;

            var accountName = QQAccount.FullName;
            var disName = discuss.FullName;

            #region 从主数据库的Chat_xxxx表获取聊天记录

            var msgTableName = string.Format("tb_discussGrp_{0}", discuss.QQNumber);
            var isExitMsgTable = MainContext.ExistTable(msgTableName);
            if (isExitMsgTable)
            {
                MainContext.UsingSafeConnection(string.Format("SELECT * FROM {0}", msgTableName), r =>
                {
                    if (r.Read())
                    {
                        bAll = false;
                        do
                        {
                            CreateDiscussMessageCore(discuss.QQNumber, r.ToDynamic(), disMsgNode, accountName, disName);
                        } while (r.Read());
                    }
                });
            }

            #endregion

            #region 从FTSMsg.db获取删除聊天记录

            if (null != FTSMsgContext)
            {
                string sql = "";
                if (bAll)
                {
                    sql = string.Format(@"SELECT
                                          	c1uin,
                                          	c2time,
                                            CAST (c5nickName AS varchar) AS c5nickName,
                                          	CAST (c7content AS varchar) AS c7content
                                          FROM
                                          	tb_Index_discussGrp_content
                                          WHERE
                                          	c8conversationUin = '{0}'
                                          ORDER BY
                                          	c2time", discuss.QQNumber);
                }
                else
                {
                    sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                        SELECT
                                          	c1uin,
                                          	c2time,
                                            CAST (c5nickName AS varchar) AS c5nickName,
                                          	CAST (c7content AS varchar) AS c7content
                                        FROM
                                          	tb_Index_discussGrp_content
                                        WHERE
                                          	c8conversationUin = '{1}'
                                        AND c0msgId NOT IN (SELECT MsgId FROM MsgDB.tb_discussGrp_{1} WHERE msgId NOTNULL)
                                        ORDER BY
                                        	c2time",
                                        MainContext.DbFilePath, discuss.QQNumber);
                }

                FTSMsgContext.UsingSafeConnection(new SQLiteString(sql), r =>
                {
                    while (r.Read())
                    {
                        disMsgNode.Items.Add(CreateTroopMessageCoreFromFTSMsg(r.ToDynamic(), accountName, disName));
                    }
                });
            }

            #endregion

            return null;
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        private void CreateFriendMessageCore(string friendQQnumber, dynamic messDy, TreeNode friendMsgNode, string accountName, string friendName)
        {
            MessageCore msg = new MessageCore();

            //获取消息类型
            GetMessageType(DynamicConvert.ToSafeString(messDy.type), ref msg);

            //获取消息内容
            GetMessageContent(ref msg, messDy, DynamicConvert.ToSafeString(messDy.content), friendQQnumber);
            ;
            #region 处理临时会话中的无用字符

            try
            {
                if (msg.Type == EnumColumnType.String && DynamicConvert.ToSafeString(messDy.type) == "10000")
                {
                    var data = Encoding.UTF8.GetBytes(msg.Content);
                    if (data[0] == 0x18)
                    {
                        var index = data.Reverse().ToList().IndexOf(0x18);
                        if (data[data.Length - index] == 0x14)
                        {
                            index--;
                        }
                        msg.Content = Encoding.UTF8.GetString(data, data.Length - index, index);
                    }
                }
            }
            catch { }

            #endregion

            msg.DataState = DynamicConvert.ToEnumByValue(messDy.XLY_DataType, EnumDataState.None);
            msg.Date = DynamicConvert.ToSafeDateTime(messDy.time);

            if (DynamicConvert.ToSafeString(messDy.flag) == "0")
            {//发送
                msg.SendState = EnumSendState.Send;
                msg.SenderName = accountName;
                msg.Receiver = friendName;
            }
            else
            {//接收
                msg.SendState = EnumSendState.Receive;
                msg.SenderName = friendName;
                msg.Receiver = accountName;
            }

            friendMsgNode.Items.Add(msg);
        }

        private void CreateGroupMessageCore(string groupQQnumber, dynamic messDy, TreeNode groupMsgNode, string accountName, string groupName)
        {
            MessageCore msg = new MessageCore();

            //获取消息类型
            GetMessageType(DynamicConvert.ToSafeString(messDy.sMsgType), ref msg);

            //获取消息内容
            GetMessageContent(ref msg, messDy, DynamicConvert.ToSafeString(messDy.strMsg), groupQQnumber);

            #region 处理临时会话中的无用字符

            try
            {
                if (msg.Type == EnumColumnType.String && DynamicConvert.ToSafeString(messDy.sMsgType) == "10000")
                {
                    var data = Encoding.UTF8.GetBytes(msg.Content);
                    if (data[0] == 0x18)
                    {
                        var index = data.Reverse().ToList().IndexOf(0x18);
                        if (data[data.Length - index] == 0x14)
                        {
                            index--;
                        }
                        msg.Content = Encoding.UTF8.GetString(data, data.Length - index, index);
                    }
                }
            }
            catch { }

            #endregion

            msg.DataState = DynamicConvert.ToEnumByValue(messDy.XLY_DataType, EnumDataState.None);
            msg.Date = DynamicConvert.ToSafeDateTime(messDy.MsgTime);

            string sendUin = DynamicConvert.ToSafeString(messDy.SendUin);

            if (sendUin == QQAccount.QQNumber)
            {//发送
                msg.SendState = EnumSendState.Send;
                msg.SenderName = accountName;
                msg.Receiver = groupName;
            }
            else
            {//接收
                msg.SendState = EnumSendState.Receive;
                msg.SenderName = string.Format("{0}({1})", DynamicConvert.ToSafeString(messDy.nickName), sendUin);
                msg.Receiver = accountName;
            }

            groupMsgNode.Items.Add(msg);
        }

        private void CreateDiscussMessageCore(string discussQQnumber, dynamic messDy, TreeNode discussMsgNode, string accountName, string discussName)
        {
            MessageCore msg = new MessageCore();

            //获取消息类型
            GetMessageType(DynamicConvert.ToSafeString(messDy.MsgType), ref msg);

            //获取消息内容
            GetMessageContent(ref msg, messDy, DynamicConvert.ToSafeString(messDy.Msg), discussQQnumber);

            #region 处理临时会话中的无用字符

            try
            {
                if (msg.Type == EnumColumnType.String && DynamicConvert.ToSafeString(messDy.MsgType) == "10000")
                {
                    var data = Encoding.UTF8.GetBytes(msg.Content);
                    if (data[0] == 0x18)
                    {
                        var index = data.Reverse().ToList().IndexOf(0x18);
                        if (data[data.Length - index] == 0x14)
                        {
                            index--;
                        }
                        msg.Content = Encoding.UTF8.GetString(data, data.Length - index, index);
                    }
                }
            }
            catch { }

            #endregion

            msg.DataState = DynamicConvert.ToEnumByValue(messDy.XLY_DataType, EnumDataState.None);
            msg.Date = DynamicConvert.ToSafeDateTime(messDy.MsgTime);

            string sendUin = DynamicConvert.ToSafeString(messDy.SendUin);

            if (sendUin == QQAccount.QQNumber)
            {//发送
                msg.SendState = EnumSendState.Send;
                msg.SenderName = accountName;
                msg.Receiver = discussName;
            }
            else
            {//接收
                msg.SendState = EnumSendState.Receive;
                msg.SenderName = string.Format("{0}({1})", DynamicConvert.ToSafeString(messDy.NickName), sendUin);
                msg.Receiver = accountName;
            }

            discussMsgNode.Items.Add(msg);
        }

        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="typeid">类型ID</param>
        /// <param name="msg">消息对象</param>
        private void GetMessageType(string typeid, ref MessageCore msg)
        {
            switch (typeid)
            {
                case "0":
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
                case "1":
                case "3847":
                    msg.Type = EnumColumnType.Image;
                    msg.MessageType = "图片";
                    break;
                case "3":
                case "3849":
                    msg.Type = EnumColumnType.Audio;
                    msg.MessageType = "语音";
                    break;
                case "4":
                    msg.Type = EnumColumnType.File;
                    msg.MessageType = "文件传输";
                    break;
                case "181":
                case "3848":
                    msg.Type = EnumColumnType.Video;
                    msg.MessageType = "视频";
                    break;
                case "12":
                    msg.Type = EnumColumnType.VideoChat;
                    msg.MessageType = "视频通话";
                    break;
                case "147":
                    msg.Type = EnumColumnType.AudioCall;
                    msg.MessageType = "语音通话";
                    break;
                case "157":
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "窗口抖动";
                    break;
                case "8":
                case "143":
                case "7141":
                case "7132":
                    msg.Type = EnumColumnType.System;
                    msg.MessageType = "系统消息";
                    break;
                case "141":
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "其他消息";
                    break;
                default:
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
            }
        }

        /// <summary>
        /// 获取消息内容
        /// </summary>
        /// <param name="msg">消息对象</param>
        /// <param name="mess">源消息对象</param>
        /// <param name="mess">消息内容</param>
        /// <param name="uin">好友消息为好友uin,讨论组消息为讨论组uin</param>
        private void GetMessageContent(ref MessageCore msg, dynamic mess, string content, string uin)
        {
            string path = string.Empty;

            switch (msg.Type)
            {
                case EnumColumnType.Image:
                    path = GetImagePath(content, DynamicConvert.ToSafeString(mess.picUrl));
                    break;
                case EnumColumnType.Audio:
                    path = GetAudioPath(content, DynamicConvert.ToSafeString(mess.picUrl));
                    break;
                case EnumColumnType.Video:
                    path = GetVideoPath(content, DynamicConvert.ToSafeString(mess.picUrl));
                    break;
                case EnumColumnType.File:
                    IEnumerable<dynamic> lsfileInfo = MainContext.Find(string.Format("SELECT fileName,filePath FROM tb_File WHERE peerUin = '{0}' AND msgID = '{1}' AND XLY_DataType = 2",
                        uin, DynamicConvert.ToSafeString(mess.msgId)));
                    if (lsfileInfo.IsValid())
                    {
                        var fileInfo = lsfileInfo.FirstOrDefault();

                        string filepath = Path.Combine(DataFileRootPath, "Documents", DynamicConvert.ToSafeString(fileInfo.filePath));
                        if (FileHelper.IsValid(filepath))
                        {
                            path = filepath;
                        }
                        else
                        {
                            path = Path.Combine(MediaFilePath, "FileRecv", DynamicConvert.ToSafeString(fileInfo.fileName));
                        }
                    }
                    break;
                case EnumColumnType.System:
                    msg.Content = content;
                    return;
                default:
                    msg.Content = content;
                    return;
            }

            msg.Content = FileHelper.IsValid(path) ? path : content;
        }

        /// <summary>
        /// 从消息内容中，匹配图片名称
        /// </summary>
        private static readonly Regex ImageNameReg = new Regex(@"(?<=<img>).+(?=</img>)", RegexOptions.Compiled);

        /// <summary>
        /// 获取聊天图片路径
        /// </summary>
        /// <param name="content"></param>
        /// <param name="picUrl"></param>
        /// <returns></returns>
        private string GetImagePath(string content, string picUrl)
        {
            try
            {
                string realFileName = string.Empty;

                if (picUrl.IsInvalid())
                {
                    // picUrl为空，直接根据content解析图片名称
                    if (content.Contains("<img>") && content.Contains("</img>"))
                    {
                        realFileName = ImageNameReg.Match(content).Value;
                    }
                }
                else
                {
                    var jData = JArray.Parse(picUrl)[0];

                    realFileName = jData["md5"].ToSafeString();
                    if (realFileName.IsInvalid())
                    {
                        realFileName = jData["name"].ToSafeString().TrimStart("<img>").TrimEnd("</img>");
                    }
                }

                if (realFileName.IsValid())
                {
                    realFileName = realFileName + ".*";

                    var path = GetFilePath(realFileName, Path.Combine(MediaFilePath, "Image"), false);
                    if (FileHelper.IsValid(path))
                    {
                        return path;
                    }

                    path = GetFilePath(realFileName, Path.Combine(MediaFilePath, "image_big"), false);
                    if (FileHelper.IsValid(path))
                    {
                        return path;
                    }

                    path = GetFilePath(realFileName, Path.Combine(MediaFilePath, "image_original"), false);
                    if (FileHelper.IsValid(path))
                    {
                        return path;
                    }

                    path = GetFilePath(realFileName, Path.Combine(MediaFilePath, "image_thumbnail"), false);
                    if (FileHelper.IsValid(path))
                    {
                        return path;
                    }

                    return realFileName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return content;
            }
        }

        /// <summary>
        /// 获取聊天语音文件路径
        /// </summary>
        /// <param name="content"></param>
        /// <param name="picUrl"></param>
        /// <returns></returns>
        private string GetAudioPath(string content, string picUrl)
        {
            if (picUrl.IsInvalid())
            {
                return content;
            }

            try
            {
                var jData = JArray.Parse(picUrl)[0];
                var voicePath = GetFilePath(jData["fileName"].ToSafeString() + ".*", Path.Combine(MediaFilePath, "Audio"), false);
                if (!FileHelper.IsValid(voicePath))
                {
                    return content;
                }

                return AudioDecodeHelper.Decode(voicePath);
            }
            catch
            {
                //picUrl中，非正常格式json字符串，不处理
                return content;
            }
        }

        /// <summary>
        /// 获取聊天视频文件路径
        /// </summary>
        /// <param name="content"></param>
        /// <param name="picUrl"></param>
        /// <returns></returns>
        private string GetVideoPath(string content, string picUrl)
        {
            if (picUrl.IsInvalid())
            {
                return content;
            }

            try
            {
                var jData = JArray.Parse(picUrl)[0];
                return GetFilePath(jData["videoMD5"].ToSafeString() + ".*", Path.Combine(MediaFilePath, "ShortVideo"), false);
            }
            catch
            {
                //picUrl中，非正常格式json字符串，不处理
                return content;
            }
        }

        /// <summary>
        /// 根据文件名称，查找文件全路径
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="searchPath">文件检索路径</param>
        /// <returns>文件全路径</returns>
        private string GetFilePath(string fileName, string searchPath, bool isAll = true)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(searchPath);
                if (!info.Exists)
                {
                    return fileName;
                }

                var files = info.GetFiles(fileName, isAll ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                var matchFiles = files.OrderByDescending(c => c.Length).FirstOrDefault();
                return matchFiles == null ? string.Empty : matchFiles.FullName;
            }
            catch
            {
                return fileName;
            }
        }

        private MessageCore CreateFriendMessageCoreFromFTSMsg(dynamic message, string accountText, string friendText)
        {
            MessageCore mess = new MessageCore();
            mess.Type = EnumColumnType.String;
            mess.Date = DynamicConvert.ToSafeDateTime(message.c2time);
            mess.Content = DynamicConvert.ToSafeString(message.c7content);
            mess.DataState = EnumDataState.Deleted;

            if (DynamicConvert.ToSafeString(message.c4flag) == "0")
            {
                mess.SendState = EnumSendState.Send;
                mess.SenderName = accountText;
                mess.Receiver = friendText;
            }
            else
            {
                mess.SendState = EnumSendState.Receive;
                mess.SenderName = friendText;
                mess.Receiver = accountText;
            }

            return mess;
        }

        private MessageCore CreateTroopMessageCoreFromFTSMsg(dynamic message, string accountText, string groupText)
        {
            string uin = DynamicConvert.ToSafeString(message.c1uin);

            MessageCore mess = new MessageCore();
            mess.Type = EnumColumnType.String;
            mess.Date = DynamicConvert.ToSafeDateTime(message.c2time);
            mess.Content = DynamicConvert.ToSafeString(message.c7content);
            mess.DataState = EnumDataState.Deleted;

            if (uin == QQAccount.QQNumber)
            {
                mess.SendState = EnumSendState.Send;
                mess.SenderName = accountText;
                mess.Receiver = groupText;
            }
            else
            {
                mess.SendState = EnumSendState.Receive;
                mess.SenderName = string.Format("{0}({1})", DynamicConvert.ToSafeString(message.c5nickName), uin);
                mess.Receiver = accountText;
            }

            return mess;
        }

        #endregion

    }
}
