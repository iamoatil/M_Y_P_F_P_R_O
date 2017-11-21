/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 10:10:37 
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
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓Twitter数据解析类
    /// </summary>
    internal class AndroidTwitterDataParserCoreV1_0
    {
        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// /com.twitter.android/databases/ 路径
        /// </summary>
        private string DatabasesPath { get; set; }

        /// <summary>
        /// 安卓Twitter数据解析类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="databasesPath">/com.twitter.android/databases/ 路径</param>
        public AndroidTwitterDataParserCoreV1_0(string savedatadbpath, string databasesPath)
        {
            DbFilePath = savedatadbpath;
            DatabasesPath = databasesPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(TreeDataSource datasource)
        {
            var rootNode = new TreeNode();
            rootNode.Text = "Twitter";
            rootNode.Type = typeof(TwitterAccount);
            rootNode.Items = new DataItems<TwitterAccount>(DbFilePath);

            datasource.TreeNodes.Add(rootNode);

            var allDbs = FindDbs(DatabasesPath);
            foreach (var db in allDbs)
            {
                var accountNode = ParseTwitterAccountNode(db, out TwitterAccount account);

                rootNode.TreeNodes.Add(accountNode);
                rootNode.Items.Add(account);
            }
        }

        /// <summary>
        /// 构造Twitter帐号节点
        /// </summary>
        /// <param name="accountDb"></param>
        /// <returns></returns>
        private TreeNode ParseTwitterAccountNode(string accountDb, out TwitterAccount account)
        {
            TreeNode accountNode = new TreeNode() { Type = typeof(TwitterAccount), Items = new DataItems<TwitterAccount>(DbFilePath) };

            var dbFile = SqliteRecoveryHelper.DataRecovery(accountDb, @"chalib\Andriod_Twitter\twitter.charactor", GetRecoveryTables(accountDb));

            using (var context = new SqliteContext(accountDb))
            {
                using (var newContext = new SqliteContext(dbFile))
                {
                    //初始化导航树
                    TreeNode usersNode = new TreeNode() { Type = typeof(TwitterAccount), Text = "相关用户", Items = new DataItems<TwitterAccount>(DbFilePath) };
                    TreeNode statusesNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "相关推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };
                    TreeNode chatNode = new TreeNode() { Type = typeof(TwitterConverstationEntry), Text = "聊天记录", Items = new DataItems<TwitterConverstationEntry>(DbFilePath) };
                    TreeNode searchNode = new TreeNode() { Type = typeof(TwitterSearchEntry), Text = "搜索记录", Items = new DataItems<TwitterSearchEntry>(DbFilePath) };
                    TreeNode viewNode = new TreeNode() { Type = typeof(TwitterViewEntry), Text = "查询列表", Items = new DataItems<TwitterViewEntry>(DbFilePath) };
                    TreeNode momentNode = new TreeNode() { Type = typeof(TwitterMomentEntry), Text = "瞬间", Items = new DataItems<TwitterMomentEntry>(DbFilePath) };

                    accountNode.TreeNodes.Add(usersNode);
                    accountNode.TreeNodes.Add(statusesNode);
                    accountNode.TreeNodes.Add(chatNode);
                    accountNode.TreeNodes.Add(searchNode);
                    accountNode.TreeNodes.Add(viewNode);
                    accountNode.TreeNodes.Add(momentNode);

                    //1.获取主帐号信息
                    var masterAccount = GetMasterAccount(accountDb, context);
                    accountNode.Text = string.Format("{0}({1})", masterAccount.NickName, masterAccount.UserName);
                    accountNode.Items.Add(masterAccount);

                    //2.获取用户信息列表
                    var allUsers = GetAllTwitterAccount(context);
                    foreach (var user in allUsers.Where(u => u.UserId != masterAccount.UserId))
                    {
                        usersNode.Items.Add(user);
                    }

                    //3.获取推文信息
                    CreateStatusesNode(masterAccount, allUsers, newContext, statusesNode);

                    //4.获取聊天信息
                    CreateConversationNode(masterAccount, allUsers, context, chatNode);

                    //5.获取搜索记录
                    CreateSearchNode(context, searchNode);

                    //6.获取查询列表
                    CreateViewNode(context, viewNode);

                    //7.获取瞬间
                    CreateMomentNode(context, momentNode);

                    account = masterAccount;
                }
            }

            return accountNode;
        }

        /// <summary>
        /// 构造推文节点
        /// </summary>
        /// <param name="masterAccount"></param>
        /// <param name="allUsers"></param>
        /// <param name="context"></param>
        /// <param name="statusesNode"></param>
        private void CreateStatusesNode(TwitterAccount masterAccount, List<TwitterAccount> allUsers, SqliteContext context, TreeNode statusesNode)
        {
            TreeNode myStatusesNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "我发布的推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };
            TreeNode myHuifuNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "我回复的推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };
            TreeNode myZhuanfaNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "我转发的推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };
            TreeNode myOtherNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "其他推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };
            TreeNode unkownNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = "未知推文", Items = new DataItems<TwitterStatuses>(DbFilePath) };

            //1.获取推文列表
            var allStatuses = GetAllTwitterStatuses(context);

            List<TwitterStatuses> temp = new List<TwitterStatuses>();

            //2.获取我发布的推文
            var myStatuses = allStatuses.Where(s => s.TwitterStatusesType == TwitterStatusesType.Master && s.AuthorId == masterAccount.UserName);
            foreach (var st in myStatuses.Reverse())
            {//反转，优先显示最近的推文
                var stNode = new TreeNode() { Type = typeof(TwitterStatuses), Text = string.Format("{0}-{1}", st.CreateTime, st.AuthorName), Items = new DataItems<TwitterStatuses>(DbFilePath) };
                myStatusesNode.TreeNodes.Add(stNode);

                stNode.Items.Add(st);
                foreach (var ss in GetChildTwitterStatuses(allStatuses, st))
                {
                    stNode.Items.Add(ss);
                    temp.Add(ss);
                }
            }

            //3.获取其他推文
            var myrStatuses = allStatuses.Where(s => s.TwitterStatusesType == TwitterStatusesType.Master && s.AuthorId != masterAccount.UserName);
            bool isOther = true;
            foreach (var st in myrStatuses.Reverse())
            {//反转，优先显示最近的推文
                List<TwitterStatuses> tempItems = new List<TwitterStatuses>();

                var stNode = new TreeNode() { Type = typeof(TwitterStatuses), Items = new DataItems<TwitterStatuses>(DbFilePath) };
                stNode.Text = string.Format("{0}-{1}", st.CreateTime, st.AuthorName);
                stNode.Items.Add(st);
                tempItems.Add(st);

                foreach (var ss in GetChildTwitterStatuses(allStatuses, st))
                {
                    stNode.Items.Add(ss);
                    tempItems.Add(ss);
                }

                isOther = true;

                if (tempItems.Any(s => s.AuthorId == masterAccount.UserName))
                {//我回复的推文
                    myHuifuNode.TreeNodes.Add(stNode);
                    isOther = false;
                }

                if (tempItems.Any(s => s.Retweeted == 1))
                {//我转发的推文
                    myZhuanfaNode.TreeNodes.Add(stNode);
                    isOther = false;
                }

                if (isOther)
                {//其他推文
                    myOtherNode.TreeNodes.Add(stNode);
                }

                temp.AddRange(tempItems);
            }

            //4.获取未知推文
            foreach (var ss in allStatuses.Where(s => !temp.Any(ts => ts.StatusesId == s.StatusesId)))
            {
                unkownNode.Items.Add(ss);
            }

            //5.全部推文列表
            statusesNode.Items.AddRange(allStatuses);

            statusesNode.TreeNodes.Add(myStatusesNode);
            statusesNode.TreeNodes.Add(myHuifuNode);
            statusesNode.TreeNodes.Add(myZhuanfaNode);
            statusesNode.TreeNodes.Add(myOtherNode);
            statusesNode.TreeNodes.Add(unkownNode);
        }

        /// <summary>
        /// 构造聊天节点
        /// </summary>
        /// <param name="masterAccount"></param>
        /// <param name="allUsers"></param>
        /// <param name="context"></param>
        /// <param name="chatNode"></param>
        private void CreateConversationNode(TwitterAccount masterAccount, List<TwitterAccount> allUsers, SqliteContext context, TreeNode chatNode)
        {
            TreeNode privateChatNode = new TreeNode() { Type = typeof(TwitterConverstationEntry), Text = "私聊", Items = new DataItems<TwitterConverstationEntry>(DbFilePath) };
            TreeNode groupChatNode = new TreeNode() { Type = typeof(TwitterGroupConverstation), Text = "群聊", Items = new DataItems<TwitterGroupConverstation>(DbFilePath) };

            //1.获取私聊
            var privateChatUsers = context.Find(new SQLiteString("SELECT conversation_id FROM conversations WHERE type = 0 ORDER BY _id"));
            foreach (var privateChat in privateChatUsers)
            {
                string conversation_id = DynamicConvert.ToSafeString(privateChat.conversation_id);//私聊id格式为  XXX-MMM

                //获取用户列表
                var chatUsers = context.Find(new SQLiteString(string.Format("SELECT user_id FROM conversation_participants WHERE conversation_id = '{0}'", conversation_id)));
                //if (chatUsers.IsInvalid() || chatUsers.Count != 2)
                //{
                //    continue;
                //}

                TreeNode tempChatNode = new TreeNode() { Type = typeof(TwitterConverstationEntry), Items = new DataItems<TwitterConverstationEntry>(DbFilePath) };

                //获取私聊对象信息
                string otherUserId = chatUsers.Select(u => DynamicConvert.ToSafeString(u.user_id)).FirstOrDefault(id => id != masterAccount.UserId);
                var otherUser = allUsers.FirstOrDefault(u => u.UserId == otherUserId);
                if (null != otherUser)
                {
                    tempChatNode.Text = string.Format("{0}({1})", otherUser.NickName, otherUser.UserName);
                }
                else
                {
                    tempChatNode.Text = otherUserId;
                }

                //获取聊天内容
                var entries = context.Find(new SQLiteString(string.Format("SELECT c.user_id,u.username,u.name,c.created,c.entry_type,c.data,c.request_id FROM conversation_entries c,users u WHERE c.conversation_id = '{0}' AND c.entry_type IN (0,1) AND c.user_id = u.user_id ORDER BY c.created", conversation_id)));
                foreach (var entry in entries)
                {
                    tempChatNode.Items.Add(TwitterConverstationEntry.DyConvert(masterAccount, entry));
                }

                privateChatNode.TreeNodes.Add(tempChatNode);
            }

            //2.获取群聊
            var groupChats = context.Find(new SQLiteString("SELECT conversation_id,title FROM conversations WHERE type = 1 ORDER BY _id"));
            foreach (var groupChat in groupChats)
            {
                string conversation_id = DynamicConvert.ToSafeString(groupChat.conversation_id);//群聊id
                string conversation_name = DynamicConvert.ToSafeString(groupChat.title);//群聊名称

                TwitterGroupConverstation group = new TwitterGroupConverstation();
                group.ConversationId = conversation_id;
                group.GroupName = conversation_name;

                //获取用户列表
                var chatUsers = context.Find(new SQLiteString(string.Format("SELECT u.username,u.name,c.join_time FROM conversation_participants c,users u WHERE c.conversation_id = '{0}' AND c.user_id = u.user_id", conversation_id)));
                group.GroupMembersCount = chatUsers.Count();

                //构造群聊信息
                StringBuilder sb = new StringBuilder();
                foreach (var user in chatUsers)
                {
                    var username = DynamicConvert.ToSafeString(user.username);
                    var nickname = DynamicConvert.ToSafeString(user.name);

                    sb.AppendLine(string.Format("{0} {1}", username, nickname));
                }
                group.GroupMembers = sb.ToString();

                //获取聊天内容
                TreeNode tempGroupChatNode = new TreeNode() { Type = typeof(TwitterConverstationEntry), Items = new DataItems<TwitterConverstationEntry>(DbFilePath) };
                tempGroupChatNode.Text = string.Format("{0}({1})", conversation_name, conversation_id);

                var entries = context.Find(new SQLiteString(string.Format("SELECT c.user_id,u.username,u.name,c.created,c.entry_type,c.data,c.request_id FROM conversation_entries c,users u WHERE c.conversation_id = '{0}' AND c.entry_type IN (0,1) AND c.user_id = u.user_id ORDER BY c.created", conversation_id)));
                foreach (var entry in entries)
                {
                    tempGroupChatNode.Items.Add(TwitterConverstationEntry.DyConvert(masterAccount, entry));
                }

                groupChatNode.Items.Add(group);
                groupChatNode.TreeNodes.Add(tempGroupChatNode);
            }

            chatNode.TreeNodes.Add(privateChatNode);
            chatNode.TreeNodes.Add(groupChatNode);
        }

        /// <summary>
        /// 构造搜索记录节点
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchNode"></param>
        private void CreateSearchNode(SqliteContext context, TreeNode searchNode)
        {
            var list = context.Find(new SQLiteString("SELECT name,time,type FROM search_queries ORDER BY time DESC"));

            searchNode.Items.AddRange(list.Select(d => TwitterSearchEntry.DyConvert(d)).Cast<TwitterSearchEntry>());
        }

        /// <summary>
        /// 构造查询列表记录节点
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewNode"></param>
        private void CreateViewNode(SqliteContext context, TreeNode viewNode)
        {
            var list = context.Find(new SQLiteString("SELECT topics_ev_id,topics_ev_owner_id,topics_ev_query,topics_ev_title,topics_ev_subtitle FROM lists_view"));

            viewNode.Items.AddRange(list.Select(d => TwitterViewEntry.DyConvert(d)).Cast<TwitterViewEntry>());
        }

        /// <summary>
        /// 构造瞬间记录节点
        /// </summary>
        /// <param name="context"></param>
        /// <param name="momentNode"></param>
        private void CreateMomentNode(SqliteContext context, TreeNode momentNode)
        {
            var list = context.Find(new SQLiteString("SELECT _id as id,title,subcategory_string,subcategory_favicon_url,time_string,description,moment_url,capsule_content_version FROM moments"));

            momentNode.Items.AddRange(list.Select(d => TwitterMomentEntry.DyConvert(d)).Cast<TwitterMomentEntry>());
        }

        #region 其他方法

        /// <summary>
        /// 获取主用户
        /// </summary>
        /// <param name="accountDb"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private TwitterAccount GetMasterAccount(string accountDb, SqliteContext context)
        {
            var accountid = FindAccountId(accountDb);

            var res = context.Find(new SQLiteString(String.Format("SELECT * FROM users WHERE user_id = '{0}'", accountid)));
            if (res.IsValid())
            {
                return TwitterAccount.DyConvert(res.First());
            }
            else
            {
                return new TwitterAccount() { UserId = accountid };
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<TwitterAccount> GetAllTwitterAccount(SqliteContext context)
        {
            return context.Find(new SQLiteString("SELECT * FROM users")).Select(d => TwitterAccount.DyConvert(d)).Cast<TwitterAccount>().ToList();
        }

        /// <summary>
        /// 获取所有推文
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<TwitterStatuses> GetAllTwitterStatuses(SqliteContext context)
        {
            var items = context.Find(new SQLiteString("SELECT s.XLY_DataType,s.status_id,(SELECT username FROM users WHERE user_id = s.author_id) as username,(SELECT name FROM users WHERE user_id = s.author_id) as author_name,s.in_r_user_id,(SELECT name FROM users WHERE user_id = s.in_r_user_id) as in_r_user_name,s.in_r_status_id,s.content,s.created,s.source,s.favorited,s.retweeted,s.favorite_count,s.retweet_count FROM statuses s ORDER BY s.created")).Select(d => TwitterStatuses.DyConvert(d)).Cast<TwitterStatuses>().Where(i => i.StatusesId.IsValid() && i.StatusesId != "0").ToList();

            return DataTypeFilter(items, (ls, i) => ls.Any(ii => ii.StatusesId == i.StatusesId));
        }

        /// <summary>
        /// 获取主推文的子推文
        /// </summary>
        /// <param name="allTwitterStatuses"></param>
        /// <param name="masterTwitterStatuses"></param>
        /// <returns></returns>
        private List<TwitterStatuses> GetChildTwitterStatuses(List<TwitterStatuses> allTwitterStatuses, TwitterStatuses masterTwitterStatuses)
        {
            List<TwitterStatuses> childs = new List<TwitterStatuses>();

            //获取第一代
            var temp = allTwitterStatuses.Where(t => t.InRStatusId == masterTwitterStatuses.StatusesId).ToList();

            //获取第N代
            int count = 0;
            while (temp.IsValid() && count < 1024)
            {
                childs.AddRange(temp);
                temp = allTwitterStatuses.Where(t => temp.Any(tt => tt.StatusesId == t.InRStatusId)).ToList();
                count++;
            }

            //按时间排序
            childs.Sort((l, r) =>
            {
                return DateTime.Compare(l.CreateTime.Value, r.CreateTime.Value);
            });

            return childs;
        }

        /// <summary>
        /// 查找数据库，名字为XXX-47.db 其中XXX为一串数字
        /// </summary>
        /// <param name="databasesPath">\com.twitter.android\databases 文件夹所在路径</param>
        /// <returns></returns>
        private IList<string> FindDbs(string databasesPath)
        {
            return Directory.GetFiles(databasesPath, "*.db").Where(f => System.Text.RegularExpressions.Regex.IsMatch(f, @"\d+-47.db")).ToList();
        }

        /// <summary>
        /// 获取帐号ID
        /// </summary>
        /// <param name="accountDb"></param>
        /// <returns></returns>
        private string FindAccountId(string accountDb)
        {
            return System.Text.RegularExpressions.Regex.Match(accountDb, @"(\d+)-47.db").Groups[1].Value;
        }

        private readonly static string[] _tables = new string[] { "users", "statuses", "conversation_participants", "conversations", "conversation_entries" };

        private string GetRecoveryTables(string dbFile)
        {
            using (var context = new SqliteContext(dbFile))
            {
                return String.Join(",", _tables.Where(tablename => context.ExistTable(tablename)));
            }
        }

        private delegate bool IsExits<T>(List<T> items, T item);

        private List<T> DataTypeFilter<T>(List<T> items, IsExits<T> fun) where T : AbstractTwitterEntity
        {
            var md5List = items.Select(i => i.MD5).Distinct().ToList();

            if (md5List.Count != items.Count)
            {
                foreach (var md5 in md5List)
                {
                    var res = items.Where(c => c.MD5 == md5).ToList();
                    if (res.IsValid() && res.Count > 1)
                    {
                        foreach (var c in res.Skip(1))
                        {
                            items.Remove(c);
                        }
                    }
                }
            }

            var deletes = items.Where(i => i.DataState != EnumDataState.Normal).ToList();
            var normals = items.Where(i => i.DataState == EnumDataState.Normal).ToList();

            foreach (var del in deletes)
            {
                if (!fun(normals, del))
                {
                    normals.Add(del);
                }
            }

            return normals;
        }

        #endregion

    }
}
