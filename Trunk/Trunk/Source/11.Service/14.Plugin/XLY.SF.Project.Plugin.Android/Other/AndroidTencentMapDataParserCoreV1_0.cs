/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/16 15:50:43 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓腾讯地图数据解析类
    /// </summary>
    internal class AndroidTencentMapDataParserCoreV1_0
    {
        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// /com.tencent.map/databases/ 路径
        /// </summary>
        private string DatabasesPath { get; set; }

        /// <summary>
        /// /com.tencent.map/shared_prefs/ 路径
        /// </summary>
        private string SharedprefsPath { get; set; }

        /// <summary>
        /// /com.tencent.map/files/ 路径
        /// </summary>
        private string FilesPath { get; set; }

        /// <summary>
        /// 安卓腾讯地图数据解析类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="databasesPath">/com.tencent.map/databases/ 路径</param>
        /// <param name="sharedprefsPath">/com.tencent.map/shared_prefs/ 路径</param>
        /// <param name="filesPath">/com.tencent.map/files/ 路径</param>
        public AndroidTencentMapDataParserCoreV1_0(string savedatadbpath, string databasesPath, string sharedprefsPath, string filesPath)
        {
            DbFilePath = savedatadbpath;
            DatabasesPath = databasesPath;
            SharedprefsPath = sharedprefsPath;
            FilesPath = filesPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(TreeDataSource datasource)
        {
            TreeNode accountNode = new TreeNode() { Text = "最近登录用户", Type = typeof(TencentMapAccount) };
            TreeNode searchNode = new TreeNode() { Text = "历史搜索" };
            TreeNode favoriteNode = new TreeNode() { Text = "收藏信息" };
            TreeNode tongxingNode = new TreeNode() { Text = "同行信息" };

            datasource.TreeNodes.Add(accountNode);
            datasource.TreeNodes.Add(searchNode);
            datasource.TreeNodes.Add(favoriteNode);
            datasource.TreeNodes.Add(tongxingNode);

            CreateAccountTreeNode(SharedprefsPath, accountNode);
            CreateHistorySearch(DatabasesPath, searchNode);
            CreateFavoriteSearch(DatabasesPath, favoriteNode);
            CreateTongxing(FilesPath, tongxingNode);
        }

        /// <summary>
        /// 构造最近登录用户信息节点
        /// </summary>
        private void CreateAccountTreeNode(string sharedprefsPath, TreeNode accountNode)
        {
            string settingFile = Path.Combine(sharedprefsPath, "settings.xml");
            if (!File.Exists(settingFile))
            {
                return;
            }

            var doc = new XmlDocument();
            doc.Load(settingFile);
            XmlNode useridNode = doc.SelectSingleNode("map//long[@name='USER_DATA_COUNT_USRER_ID']");
            XmlNode nameNode = doc.SelectSingleNode("map//string[@name='USER_ACCOUNT_NICKNAME']");
            XmlNode timeNode = doc.SelectSingleNode("map//long[@name='LAST_SYNC_TIME']");
            XmlNode cityNode = doc.SelectSingleNode("map//string[@name='CITY']");
            XmlNode lonNode = doc.SelectSingleNode("map//int[@name='CENTER_LON']");
            XmlNode latNode = doc.SelectSingleNode("map//int[@name='CENTER_LAT']");

            TencentMapAccount account = new TencentMapAccount();
            if (null != useridNode)
            {
                account.UserId = useridNode.Attributes["value"].Value;
            }
            if (null != nameNode)
            {
                account.NickName = nameNode.InnerText;
            }
            if (null != timeNode)
            {
                account.LastLoginTime = ToSafeFromUnixTime(UInt64.Parse(timeNode.Attributes["value"].Value));
            }

            var address = new TencentMapAddress();

            if (null != cityNode)
            {
                address.Address = cityNode.InnerText;
            }
            if (null != lonNode)
            {
                address.Lon = ToTrueLatAndLon(Int64.Parse(lonNode.Attributes["value"].Value));
            }
            if (null != latNode)
            {
                address.Lat = ToTrueLatAndLon(Int64.Parse(latNode.Attributes["value"].Value));
            }

            account.LastLoginCity = address.ToString();

            accountNode.Items.Add(account);
        }

        /// <summary>
        /// 构造历史搜索节点
        /// </summary>
        private void CreateHistorySearch(string databasesPath, TreeNode searchNode)
        {
            try
            {
                TreeNode addSearchNode = new TreeNode() { Text = "地点搜索", Type = typeof(TencentMapSearchAddress), Items = new DataItems<TencentMapSearchAddress>(DbFilePath) };
                TreeNode addLineSearchNode = new TreeNode() { Text = "路线搜索", Type = typeof(TencentMapSearchRoute), Items = new DataItems<TencentMapSearchRoute>(DbFilePath) };

                searchNode.TreeNodes.Add(addSearchNode);
                searchNode.TreeNodes.Add(addLineSearchNode);

                string dbFile = Path.Combine(databasesPath, "search_history.db");
                if (!File.Exists(dbFile))
                {
                    return;
                }

                var newDb = SqliteRecoveryHelper.DataRecovery(dbFile, "", "search_history_tab");

                CreateAddSearchTreeNode(newDb, addSearchNode);
                CreateAddlineSearchTreeNode(newDb, addLineSearchNode);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 构造地点搜索记录节点
        /// </summary>
        private void CreateAddSearchTreeNode(string dbFile, TreeNode addSearchNode)
        {
            try
            {
                IEnumerable<dynamic> datas = null;

                using (var context = new SqliteContext(dbFile))
                {
                    datas = context.Find(new SQLiteString("SELECT _data AS datastr,XLY_DataType FROM search_history_tab WHERE _history_type = 1 ORDER BY _lasted_used DESC"));
                }

                if (datas.IsInvalid())
                {
                    return;
                }

                foreach (var data in datas)
                {
                    var search = ToTencentMapSearchAddress(data);
                    if (null != search)
                    {
                        addSearchNode.Items.Add(search);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 构造路线搜索记录节点
        /// </summary>
        private void CreateAddlineSearchTreeNode(string dbFile, TreeNode addLineSearchNode)
        {
            try
            {
                IEnumerable<dynamic> datas = null;

                using (var context = new SqliteContext(dbFile))
                {
                    datas = context.Find(new SQLiteString("SELECT _data AS datastr,XLY_DataType FROM search_history_tab WHERE _history_type = 2 ORDER BY _lasted_used DESC"));
                }

                if (datas.IsInvalid())
                {
                    return;
                }

                foreach (var data in datas)
                {
                    var search = ToTencentMapSearchRoute(data);
                    if (null != search)
                    {
                        addLineSearchNode.Items.Add(search);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 构造收藏信息节点
        /// </summary>
        private void CreateFavoriteSearch(string databasesPath, TreeNode searchNode)
        {
            try
            {
                string dbFile = Path.Combine(databasesPath, "favorite.db");
                if (!File.Exists(dbFile))
                {
                    return;
                }

                dbFile = SqliteRecoveryHelper.DataRecovery(dbFile, "", "FavoritePOIEntity,FavoriteRouteEntity,FavoriteStreetEntity");

                IEnumerable<dynamic> addrNodeDatas = null;
                IEnumerable<dynamic> routeNodeDatas = null;
                IEnumerable<dynamic> streetNodeDatas = null;

                TreeNode addrNode = new TreeNode() { Text = "收藏地点", Type = typeof(TencentMapFavoriteAddr), Items = new DataItems<TencentMapFavoriteAddr>(DbFilePath) };
                TreeNode routeNode = new TreeNode() { Text = "收藏路线", Type = typeof(TencentMapFavoriteRoute), Items = new DataItems<TencentMapFavoriteRoute>(DbFilePath) };
                TreeNode streetNode = new TreeNode() { Text = "收藏街景", Type = typeof(TencentMapFavoriteStreet), Items = new DataItems<TencentMapFavoriteStreet>(DbFilePath) };

                searchNode.TreeNodes.Add(addrNode);
                searchNode.TreeNodes.Add(routeNode);
                searchNode.TreeNodes.Add(streetNode);

                //收藏地点
                using (var context = new SqliteContext(dbFile))
                {
                    addrNodeDatas = context.Find(new SQLiteString("SELECT creatTime,name,obj,XLY_DataType FROM FavoritePOIEntity ORDER BY creatTime DESC"));
                }
                foreach (var data in addrNodeDatas)
                {
                    var addr = ToTencentMapFavoriteAddr(data);
                    if (null != addr)
                    {
                        addrNode.Items.Add(addr);
                    }
                }

                //收藏路线
                using (var context = new SqliteContext(dbFile))
                {
                    routeNodeDatas = context.Find(new SQLiteString("SELECT name,fromPoi,toPoi,time,routeType,XLY_DataType FROM FavoriteRouteEntity ORDER BY creatTime DESC"));
                }
                foreach (var data in routeNodeDatas)
                {
                    var addr = ToTencentMapFavoriteRoute(data);
                    if (null != addr)
                    {
                        routeNode.Items.Add(addr);
                    }
                }

                //收藏街景
                using (var context = new SqliteContext(dbFile))
                {
                    streetNodeDatas = context.Find(new SQLiteString("SELECT name,streetData,lastEditTime,XLY_DataType FROM FavoriteStreetEntity ORDER BY lastEditTime DESC"));
                }
                foreach (var data in streetNodeDatas)
                {
                    var addr = ToTencentMapFavoriteStreet(data);
                    if (null != addr)
                    {
                        streetNode.Items.Add(addr);
                    }
                }
            }
            finally
            {

            }
        }

        /// <summary>
        /// 构造同行信息节点
        /// </summary>
        private void CreateTongxing(string filesPath, TreeNode searchNode)
        {
            try
            {
                var dbs = Directory.GetFiles(filesPath, "*.msgstore").Where(f => System.Text.RegularExpressions.Regex.IsMatch(f, @"\d+_v4.msgstore"));
                if (dbs.IsInvalid())
                {
                    return;
                }

                foreach (var msgstore in dbs)
                {
                    var newDbFile = SqliteRecoveryHelper.DataRecovery(msgstore, "", "message");

                    string userid = System.Text.RegularExpressions.Regex.Match(msgstore, @"(\d+)_v4.msgstore").Groups[1].Value;

                    IEnumerable<dynamic> msgs = null;

                    using (SqliteContext context = new SqliteContext(newDbFile))
                    {
                        msgs = context.Find(new SQLiteString("SELECT sid,time,sender,content,XLY_DataType FROM message WHERE sender != '@TIM#SYSTEM' ORDER BY time"));
                    }

                    if (msgs.IsInvalid())
                    {
                        continue;
                    }

                    TreeNode msgNode = new TreeNode() { Text = userid, Type = typeof(TencentMapTongxingMsg), Items = new DataItems<TencentMapTongxingMsg>(DbFilePath) };
                    foreach (var msg in msgs)
                    {
                        var res = ToTencentMapTongxingMsg(msg);
                        if (null != res)
                        {
                            msgNode.Items.Add(res);
                        }
                    }

                    searchNode.TreeNodes.Add(msgNode);
                }
            }
            catch
            {

            }
        }

        #region 辅助方法

        private static DateTime? ToSafeFromUnixTime(UInt64 value, UInt64 length = 1000)
        {
            var unixTime = value / length;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime).AddHours(8);
        }

        private static double ToTrueLatAndLon(Int64 value)
        {
            // 经纬度数据都是乘以1000000后的数据
            return value / 1000000.0;
        }

        private static TencentMapSearchAddress ToTencentMapSearchAddress(dynamic data)
        {
            try
            {
                TencentMapSearchAddress search = new TencentMapSearchAddress();
                search.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                JObject jo = JObject.Parse(DynamicConvert.ToSafeString(data.datastr));
                var address = new TencentMapAddress();

                search.Key = jo["name"].ToString();
                search.LastTime = ToSafeFromUnixTime(UInt64.Parse(jo["lastedUse"].ToString()));
                search.SearchCount = int.Parse(jo["count"].ToString());

                address.Address = jo["address"].ToString();
                var geopoint = jo["geopoint"].ToString();
                if (geopoint.IsValid())
                {
                    address.Lat = ToTrueLatAndLon(Int64.Parse(geopoint.Split(',')[0]));
                    address.Lon = ToTrueLatAndLon(Int64.Parse(geopoint.Split(',')[1]));
                }

                search.ResultAddress = address.ToString();

                return search;
            }
            catch
            {
                return null;
            }
        }

        private static TencentMapSearchRoute ToTencentMapSearchRoute(dynamic data)
        {
            try
            {
                TencentMapSearchRoute search = new TencentMapSearchRoute();

                search.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                JObject jo = JObject.Parse(DynamicConvert.ToSafeString(data.datastr));

                search.Key = jo["name"].ToString();
                search.LastTime = ToSafeFromUnixTime(UInt64.Parse(jo["lastedUse"].ToString()));
                search.SearchCount = int.Parse(jo["count"].ToString());

                var route_from = jo["route_from"].ToString().Replace("\\\"", "\"");
                var route_to = jo["route_to"].ToString().Replace("\\\"", "\"");

                var from = new TencentMapAddress();
                var to = new TencentMapAddress();

                JObject joFrom = JObject.Parse(route_from);
                from.Address = joFrom["addr"].ToString();
                from.Lat = ToTrueLatAndLon(Int64.Parse(joFrom["lat"].ToString()));
                from.Lon = ToTrueLatAndLon(Int64.Parse(joFrom["lon"].ToString()));

                JObject joTo = JObject.Parse(route_to);
                to.Address = joTo["addr"].ToString();
                to.Lat = ToTrueLatAndLon(Int64.Parse(joTo["lat"].ToString()));
                to.Lon = ToTrueLatAndLon(Int64.Parse(joTo["lon"].ToString()));

                search.RouteFrom = from.ToString();
                search.RouteTo = to.ToString();

                return search;
            }
            catch
            {
                return null;
            }
        }

        private static TencentMapFavoriteAddr ToTencentMapFavoriteAddr(dynamic data)
        {
            try
            {
                TencentMapFavoriteAddr addr = new TencentMapFavoriteAddr();

                addr.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                addr.Time = ToSafeFromUnixTime(UInt64.Parse(DynamicConvert.ToSafeString(data.creatTime)), 1);
                addr.Name = DynamicConvert.ToSafeString(data.name);

                JObject jo = JObject.Parse(DynamicConvert.ToSafeString(data.obj));
                addr.Lon = ToTrueLatAndLon(Int64.Parse(jo["lon"].ToString()));
                addr.Lat = ToTrueLatAndLon(Int64.Parse(jo["lat"].ToString()));

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("详细信息:{0}", jo["addr"].ToString()));
                sb.AppendLine(string.Format("联系方式:{0}", jo["phone"].ToString()));
                sb.AppendLine(string.Format("类型:{0}", jo["classes"].ToString()));

                addr.Tag = sb.ToString();

                return addr;
            }
            catch
            {
                return null;
            }
        }

        private static TencentMapFavoriteRoute ToTencentMapFavoriteRoute(dynamic data)
        {
            try
            {
                TencentMapFavoriteRoute route = new TencentMapFavoriteRoute();

                route.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                route.Name = DynamicConvert.ToSafeString(data.name);
                route.TakeTime = string.Format("{0}分钟", DynamicConvert.ToSafeString(data.time));
                switch ((string)DynamicConvert.ToSafeString(data.routeType))
                {
                    case "0":
                        route.RouteType = "公交";
                        break;
                    case "1":
                        route.RouteType = "驾车";
                        break;
                    case "2":
                        route.RouteType = "步行";
                        break;
                    default:
                        route.RouteType = "未知";
                        break;
                }

                StringBuilder sb = new StringBuilder();
                //起点
                JObject fromPoi = JObject.Parse(DynamicConvert.ToSafeString(data.fromPoi));
                sb.AppendLine(string.Format("地点:{0}", fromPoi["name"].ToString()));
                sb.AppendLine(string.Format("详细信息:{0}", fromPoi["addr"].ToString()));
                sb.AppendLine(string.Format("经度:{0}", ToTrueLatAndLon(Int64.Parse(fromPoi["lon"].ToString()))));
                sb.AppendLine(string.Format("纬度:{0}", ToTrueLatAndLon(Int64.Parse(fromPoi["lat"].ToString()))));
                route.From = sb.ToString();

                sb.Clear();
                //终点
                JObject toPoi = JObject.Parse(DynamicConvert.ToSafeString(data.toPoi));
                sb.AppendLine(string.Format("地点:{0}", toPoi["name"].ToString()));
                sb.AppendLine(string.Format("详细信息:{0}", toPoi["addr"].ToString()));
                sb.AppendLine(string.Format("经度:{0}", ToTrueLatAndLon(Int64.Parse(toPoi["lon"].ToString()))));
                sb.AppendLine(string.Format("纬度:{0}", ToTrueLatAndLon(Int64.Parse(toPoi["lat"].ToString()))));
                route.To = sb.ToString();

                return route;
            }
            catch
            {
                return null;
            }
        }

        private static TencentMapFavoriteStreet ToTencentMapFavoriteStreet(dynamic data)
        {
            try
            {
                TencentMapFavoriteStreet street = new TencentMapFavoriteStreet();

                street.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                street.Name = DynamicConvert.ToSafeString(data.name);
                street.Time = ToSafeFromUnixTime(UInt64.Parse(DynamicConvert.ToSafeString(data.lastEditTime)), 1);

                JObject streetData = JObject.Parse(DynamicConvert.ToSafeString(data.streetData));
                street.FileInfo = string.Format("/com.tencent.map/files/street/{0}", streetData["svid"].ToString());

                return street;
            }
            catch
            {
                return null;
            }
        }

        private static TencentMapTongxingMsg ToTencentMapTongxingMsg(dynamic data)
        {
            try
            {
                TencentMapTongxingMsg msg = new TencentMapTongxingMsg();

                msg.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

                msg.GroupId = DynamicConvert.ToSafeString(data.sid);
                msg.Sender = DynamicConvert.ToSafeString(data.sender);
                msg.Time = ToSafeFromUnixTime(UInt64.Parse(DynamicConvert.ToSafeString(data.time)), 1);

                byte[] content = data.content as byte[];
                if (content.Length > 8 && content[6] == 0X0A)
                {
                    int length = content[7];
                    if (content.Length >= 8 + length)
                    {
                        msg.Msg = Encoding.UTF8.GetString(content, 8, length);
                    }
                    else
                    {
                        msg.Msg = Encoding.UTF8.GetString(content.Skip(8).ToArray());
                    }
                }
                else
                {
                    msg.Msg = Encoding.UTF8.GetString(content);
                }

                return msg;
            }
            catch
            {
                return null;
            }
        }

        #endregion

    }
}
