/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 14:48:54 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    internal class AndroidUCBrowseDataParserCoreV11_2_0_880
    {
        private string DbFilePath { get; set; }

        private string AccountPath { get; set; }

        private string DatabasesPath { get; set; }

        private string Input_historyFile { get; set; }

        private string Novel_searchFile { get; set; }

        private string DownloadsPath { get; set; }

        public AndroidUCBrowseDataParserCoreV11_2_0_880(string dbFilePath, string accountPath, string databasesPath, string input_historyFile, string novel_searchFile, string downloadsPath)
        {
            DbFilePath = dbFilePath;
            AccountPath = accountPath;
            DatabasesPath = databasesPath;
            Input_historyFile = input_historyFile;
            Novel_searchFile = novel_searchFile;
            DownloadsPath = downloadsPath;
        }

        // 每条记录标记（每条记录有一个标记）：8bit[十六进制]
        private static readonly byte[] SEARCHHISTORY_ROW_FLAG = new byte[] { 0x04, 0x00, 0x01, 0xA9, 0x1C, 0x21, 0x00, 0x00 };

        // 搜索URL标记（每条记录一般都会有一个标记）：9bit[十六进制]
        private static readonly byte[] SEARCHHISTORY_URL_FLAG = new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x00, 0x06, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 二进制操作
        /// </summary>s
        //private BlobHelper Blob { get; set; }

        /// <summary>
        /// 当前登陆账号
        /// </summary>
        private List<string> ListNumber { get; set; }

        public void BuildData(TreeDataSource browseDs)
        {
            var bookMarkTree = new TreeNode();
            bookMarkTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_BookMark);
            bookMarkTree.Type = typeof(BookMark);
            bookMarkTree.Items = new DataItems<BookMark>(DbFilePath);

            var favoriteTree = new TreeNode();
            favoriteTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_Favorite);
            favoriteTree.Type = typeof(Favorite);
            favoriteTree.Items = new DataItems<Favorite>(DbFilePath);

            var historyTree = new TreeNode();
            historyTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_History);
            historyTree.Type = typeof(History);
            historyTree.Items = new DataItems<History>(DbFilePath);

            var headLineTree = new TreeNode();
            headLineTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_HeadLine);
            headLineTree.Type = typeof(HeadLine);
            headLineTree.Items = new DataItems<HeadLine>(DbFilePath);

            var downloadFileTree = new TreeNode();
            downloadFileTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_DowloadFile);
            downloadFileTree.Type = typeof(DownloadFile);
            downloadFileTree.Items = new DataItems<DownloadFile>(DbFilePath);

            var searchHistoryTree = new TreeNode();
            searchHistoryTree.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_SearchHistory);
            searchHistoryTree.Type = typeof(SearchHistory);
            searchHistoryTree.Items = new DataItems<SearchHistory>(DbFilePath);

            var novels = new TreeNode();
            novels.Text = LanguageHelper.GetString(Languagekeys.PluginBrowser_Novels);
            novels.Type = typeof(Novels);
            novels.Items = new DataItems<Novels>(DbFilePath);

            browseDs.TreeNodes.Add(bookMarkTree);
            browseDs.TreeNodes.Add(favoriteTree);
            browseDs.TreeNodes.Add(historyTree);
            browseDs.TreeNodes.Add(headLineTree);
            browseDs.TreeNodes.Add(downloadFileTree);
            browseDs.TreeNodes.Add(searchHistoryTree);
            browseDs.TreeNodes.Add(novels);

            ListNumber = GetNumber();

            //书签
            bookMarkTree.Items.AddRange(GetBookmarks());

            //收藏
            favoriteTree.Items.AddRange(GetFavorite());

            //浏览记录
            historyTree.Items.AddRange(GetHistroies());

            //头条
            headLineTree.Items.AddRange(GetHeadLine());

            //下载/文件
            downloadFileTree.Items.AddRange(GetDownloadFile());

            //搜索记录
            //searchHistoryTree.Items.AddRange(GetSearchHistory());

            //小说书架
            novels.Items.AddRange(GetNovels());
        }

        /// <summary>
        /// 获取登陆账号
        ///  路径：/data/data/com.UCMobile/UCMobile/userdata/account
        ///  数据：目录下所有文件文件名为登陆账号
        /// </summary>
        /// <returns></returns>
        private List<string> GetNumber()
        {
            List<string> items = new List<string>();
            try
            {
                if (FileHelper.IsValidDictory(AccountPath))
                {
                    return items;
                }

                DirectoryInfo folder = new DirectoryInfo(AccountPath);
                foreach (FileInfo file in folder.GetFiles("*.*"))
                {
                    items.Add(file.Name);
                }
            }
            catch
            {
            }
            return items;
        }

        /// <summary>
        /// 书签
        /// 1，公共书签
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[bookmark.db].bookmark
        /// 
        /// 2，登陆账号书签
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[账号.db].bookmark
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BookMark> GetBookmarks()
        {
            var items = new List<BookMark>();
            try
            {
                // 公共书签
                string dataDir1 = Path.Combine(DatabasesPath, "bookmark.db");
                if (dataDir1.IsValid() && File.Exists(dataDir1))
                {
                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir1, "bookmark.db.charactor", "bookmark");

                    GetBookmarks(items, newPath);
                }

                // 登陆账号书签
                foreach (var num in ListNumber)
                {
                    string dbName = num + ".db";
                    string dataDir2 = Path.Combine(DatabasesPath, dbName);
                    if (!dataDir2.IsValid() || !File.Exists(dataDir2))
                    {
                        dataDir2 = string.Empty;
                        continue;
                    }

                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir2, dbName + ".charactor", "bookmark");

                    GetBookmarks(items, newPath);
                }
            }
            catch
            {

            }

            return items;
        }

        private void GetBookmarks(List<BookMark> items, string newPath)
        {
            using (SqliteContext _sqlite = new SqliteContext(newPath))
            {
                BookMark obj;

                var list_dynamic = _sqlite.FindByName("bookmark");
                foreach (var item in list_dynamic)
                {
                    long property = item.property;
                    // property = 1收藏信息，folder=1为文件夹
                    if (item.property != 1 || item.folder == 1) continue;

                    //16位，最后6位为微妙：1465002512825919
                    long millisecounds = item.create_time;
                    //去除最后3位000
                    string str_millisecounds = millisecounds.ToString().TrimEnd('0');
                    //取13位日期进行转换
                    if (str_millisecounds.Length > 13)
                        str_millisecounds = str_millisecounds.Substring(0, 13);
                    long.TryParse(str_millisecounds, out millisecounds);
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime result = epoch.AddMilliseconds(millisecounds);

                    obj = new BookMark()
                    {
                        Path = string.IsNullOrWhiteSpace(item.path) ? "根目录" : item.path,
                        Title = item.title,
                        Url = item.url,
                        CreatedTime = result,
                        DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                    };
                    items.Add(obj);
                }
            }
        }

        /// <summary>
        /// 收藏
        /// 1，公共收藏
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[favorite_database].favorite_common_table
        /// 
        /// 2，登陆账号收藏
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[favorite].favorite_账号ID
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Favorite> GetFavorite()
        {
            var items = new List<Favorite>();

            #region // 公共收藏

            try
            {
                string dataDir1 = Path.Combine(DatabasesPath, "favorite_database");
                if (dataDir1.IsValid() && File.Exists(dataDir1))
                {
                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir1, "favorite_database.charactor", "favorite_common_table");
                    using (SqliteContext _sqlite = new SqliteContext(newPath))
                    {
                        Favorite obj;

                        var list_dynamic = _sqlite.FindByName("favorite_common_table");
                        foreach (var item in list_dynamic)
                        {
                            obj = new Favorite()
                            {
                                Title = item.title,
                                URL = item.url,
                                OriginalURL = item.original_url,
                                Source = item.source,
                                IconPath = item.icon_path,
                                IconUrl = item.icon_url,
                                AddTime = DynamicConvert.ToSafeDateTime(item.add_time),
                                DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                            };

                            items.Add(obj);
                        }
                    }
                }
            }
            catch
            {
            }

            #endregion

            #region // 登陆账号收藏

            try
            {
                string dataDir2 = Path.Combine(DatabasesPath, "favorite");
                if (dataDir2.IsValid() && File.Exists(dataDir2))
                {
                    // 获取需要恢复的表名称
                    string tableNames = string.Empty;
                    ListNumber.ForEach(t => { tableNames += tableNames.IsValid() ? (",favorite_" + t) : "favorite_" + t; });

                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir2, "favorite.charactor", tableNames);
                    using (SqliteContext _sqlite = new SqliteContext(newPath))
                    {
                        Favorite obj;

                        foreach (var num in ListNumber)
                        {
                            var list_dynamic = _sqlite.Find(new SQLiteString(string.Format("select f.content,f.add_time from favorite_{0} f", num)));
                            foreach (var item in list_dynamic)
                            {
                                var favorite_Root = new Favorite_Root();
                                try { favorite_Root = JsonConvert.DeserializeObject<Favorite_Root>(item.content); }
                                catch { }
                                if (null == favorite_Root || null == favorite_Root.data || !favorite_Root.data.title.IsValid())
                                {
                                    continue;
                                }
                                Favorite_Data f_data = favorite_Root.data;

                                obj = new Favorite()
                                {
                                    Title = f_data.title,
                                    URL = f_data.url,
                                    OriginalURL = f_data.cmt_url,
                                    Source = f_data.source_name,
                                    IconPath = string.Empty,
                                    IconUrl = f_data.site_logo_url,
                                    AddTime = DynamicConvert.ToSafeDateTime(item.add_time),
                                    DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                                };

                                items.Add(obj);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            #endregion

            return items;
        }

        #region // 账号收藏实体
        public class Favorite_Data
        {
            /// <summary>
            /// Window_type
            /// </summary>
            public string window_type { get; set; }
            /// <summary>
            /// Is_wemedia
            /// </summary>
            public string is_wemedia { get; set; }
            /// <summary>
            /// http://m.ce.cn/uc/sh/201612/21/t20161221_18921506.shtml
            /// </summary>
            public string ori_url { get; set; }
            /// <summary>
            /// Cmt_cnt
            /// </summary>
            public string cmt_cnt { get; set; }
            /// <summary>
            /// Site_logo_type
            /// </summary>
            public string site_logo_type { get; set; }
            /// <summary>
            /// Site_id
            /// </summary>
            public string site_id { get; set; }
            /// <summary>
            /// Flv_switch
            /// </summary>
            public string flv_switch { get; set; }
            /// <summary>
            /// 中国经济网
            /// </summary>
            public string site_title { get; set; }
            /// <summary>
            /// Ch_id
            /// </summary>
            public string ch_id { get; set; }
            /// <summary>
            /// http://m.ce.cn/uc/sh/201612/21/t20161221_18921506.shtml?zzd_from=uc-iflow&sm_article_id=8615035545579684875&dl_type=3&uc_biz_str=S%3Acustom%7CC%3Aiflow_site&rd_type=reco&recoid=12681381849257101205&sp_gz=0&cid=100&app=uc-iflow
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// Site_logo_height
            /// </summary>
            public string site_logo_height { get; set; }
            /// <summary>
            /// 8615035545579684875
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// Daoliu_type
            /// </summary>
            public string daoliu_type { get; set; }
            /// <summary>
            /// 野猪正在思考猪生, 因为太投入而差点丢了性命!
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// Site_logo_width
            /// </summary>
            public string site_logo_width { get; set; }
            /// <summary>
            /// Publish_time
            /// </summary>
            public string publish_time { get; set; }
            /// <summary>
            /// Recom_ch_id
            /// </summary>
            public string recom_ch_id { get; set; }
            /// <summary>
            /// 12681381849257101205
            /// </summary>
            public string reco_id { get; set; }
            /// <summary>
            /// http://m.uczzd.cn/webview/xissAllComments?app=uc-iflow&aid=8615035545579684875&cid=100&zzd_from=uc-iflow&uc_param_str=dndsvebichfrntcpgipf&uc_biz_str=S:custom|C:comment|N:true
            /// </summary>
            public string cmt_url { get; set; }
            /// <summary>
            /// 中国经济网
            /// </summary>
            public string source_name { get; set; }
            /// <summary>
            /// http://image.uczzd.cn/11896833271544702617.jpg?id=0
            /// </summary>
            public string site_logo_url { get; set; }
        }

        public class Favorite_Root
        {
            /// <summary>
            /// Favorite_Data
            /// </summary>
            public Favorite_Data data { get; set; }
        }
        #endregion

        /// <summary>
        /// 历史记录
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[history].history
        /// </summary>
        /// <returns></returns>
        private IEnumerable<History> GetHistroies()
        {
            var items = new List<History>();

            try
            {
                string dataDir = Path.Combine(DatabasesPath, "history");
                if (!dataDir.IsValid() || !File.Exists(dataDir))
                {
                    return items;
                }

                var newPath = SqliteRecoveryHelper.DataRecovery(dataDir, "history.charactor", "history");
                using (SqliteContext _sqlite = new SqliteContext(newPath))
                {
                    History obj;

                    var list_dynamic = _sqlite.FindByName("history");
                    foreach (var item in list_dynamic)
                    {
                        obj = new History()
                        {
                            Name = item.name,
                            Url = item.url,
                            OriginalURL = item.original_url,
                            Source = item.source,
                            Visits = System.Convert.ToString(item.visited_count),
                            VisitTime = DynamicConvert.ToSafeDateTime(item.visited_time),
                            DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                        };

                        items.Add(obj);
                    }
                }
            }
            catch
            {
            }

            return items;
        }

        /// <summary>
        /// UC头条
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[info_flow].news_list、[info_flow].channel_list
        /// </summary>
        /// <returns></returns>
        private IEnumerable<HeadLine> GetHeadLine()
        {
            var items = new List<HeadLine>();

            try
            {
                string dataDir = Path.Combine(DatabasesPath, "info_flow");
                if (!dataDir.IsValid() || !File.Exists(dataDir))
                {
                    return items;
                }

                var newPath = SqliteRecoveryHelper.DataRecovery(dataDir, "info_flow.charactor", "news_list,channel_list");
                using (SqliteContext _sqlite = new SqliteContext(newPath))
                {
                    HeadLine obj;

                    var list_dynamic = _sqlite.Find(new SQLiteString(@"select cl.name,n.title,n.read_status,n.url,n.up_time
                                                                    from news_list n left join channel_list cl on n.chl_id = cl.id
                                                                    where cl.status = 1
                                                                    order by cl.channel_order,n.id"));
                    foreach (var item in list_dynamic)
                    {
                        obj = new HeadLine()
                        {
                            Name = item.name,
                            Title = item.title,
                            ReadStatus = item.read_status,
                            Url = item.url,
                            UpTime = DynamicConvert.ToSafeDateTime(item.up_time),
                            DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                        };

                        items.Add(obj);
                    }
                }
            }
            catch
            {
            }

            return items;
        }

        /// <summary>
        /// 下载/文件
        /// 1，下载/文件
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：file_mgmt_detail
        /// 
        /// 2，本地书籍：
        /// 路径：/data/data/com.UCMobile/databases
        /// 表：[NovelImport.db].novel_import_detail
        /// 
        /// 3，下载文件：/storage/emulated/legacy/UCDownloads
        /// 4，下载视频：/storage/emulated/legacy/UCDownloads/VideoData
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DownloadFile> GetDownloadFile()
        {
            var items = new List<DownloadFile>();

            #region // FileMgmt.db

            try
            {
                string dataDir = Path.Combine(DatabasesPath, "FileMgmt.db");
                if (dataDir.IsValid() && System.IO.File.Exists(dataDir))
                {
                    DownloadFile obj;

                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir, "FileMgmt.db.charactor", "file_mgmt_detail");
                    using (SqliteContext _sqlite = new SqliteContext(newPath))
                    {
                        var list_dynamic = _sqlite.FindByName("file_mgmt_detail");
                        foreach (var item in list_dynamic)
                        {
                            obj = new DownloadFile()
                            {
                                ID = item._id,
                                Name = item.name,
                                Type = item.type,
                                Size = item.size,
                                LastModified = DynamicConvert.ToSafeDateTime(item.last_modified),
                                DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                            };

                            items.Add(obj);
                        }
                    }
                }
            }
            catch
            {
            }

            #endregion

            #region // 本地书籍：[NovelImport.db].[novel_import_detail]

            try
            {
                string dataDir = Path.Combine(DatabasesPath, "NovelImport.db");
                if (dataDir.IsValid() && System.IO.File.Exists(dataDir))
                {
                    DownloadFile obj;

                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir, "NovelImport.db.charactor", "novel_import_detail");
                    using (SqliteContext _sqlite = new SqliteContext(newPath))
                    {
                        var list_dynamic = _sqlite.FindByName("novel_import_detail");
                        foreach (var item in list_dynamic)
                        {
                            obj = new DownloadFile()
                            {
                                ID = item._id,
                                Name = item.name,
                                Type = 9,
                                Path = item.path,
                                Size = item.size,
                                LastModified = DynamicConvert.ToSafeDateTime(item.last_modified),
                                DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                            };

                            items.Add(obj);
                        }
                    }
                }
            }
            catch
            {
            }

            #endregion

            #region // 下载文件：/storage/emulated/legacy/UCDownloads

            try
            {
                string dataDir = DownloadsPath;
                if (FileHelper.IsValidDictory(dataDir))
                {
                    DownloadFile obj;

                    DirectoryInfo folder = new DirectoryInfo(dataDir);
                    foreach (FileInfo item in folder.GetFiles("*.*"))
                    {
                        obj = new DownloadFile()
                        {
                            Name = item.Name,
                            Type = 10,
                            Size = item.Length,
                            LastModified = item.CreationTime,
                            DataState = EnumDataState.Normal
                        };

                        items.Add(obj);
                    }
                }
            }
            catch
            {
            }

            #endregion

            #region // 下载视频：/storage/emulated/legacy/UCDownloads/VideoData

            try
            {
                string dataDir = Path.Combine(DownloadsPath, "VideoData");
                if (FileHelper.IsValidDictory(dataDir))
                {
                    DownloadFile obj;

                    DirectoryInfo folder = new DirectoryInfo(dataDir);
                    foreach (FileInfo item in folder.GetFiles("*.*"))
                    {
                        obj = new DownloadFile()
                        {
                            Name = item.Name,
                            Type = 11,
                            Size = item.Length,
                            LastModified = item.CreationTime,
                            DataState = EnumDataState.Normal
                        };

                        items.Add(obj);
                    }
                }
            }
            catch
            {
            }

            #endregion

            return items;
        }

        ///// <summary>
        ///// 搜索记录
        ///// 路径：/data/data/com.UCMobile/user/history
        ///// 二进制文件：input_history.ucmd
        ///// </summary>
        ///// <returns></returns>
        //private IEnumerable<SearchHistory> GetSearchHistory()
        //{
        //    /*
        //     * 搜索内容：不在家，搜索URL：，搜索时间：2016/10/26 15:19:41
        //     * 搜索内容：好看，搜索URL：，搜索时间：2016/10/26 15:19:47
        //     * 搜索内容：看你们，搜索URL：，搜索时间：2016/10/26 15:19:55
        //     * 搜索内容：下载，搜索URL：，搜索时间：2016/11/19 17:01:26
        //     * 搜索内容：510107，搜索URL：，搜索时间：2016/11/11 11:20:45
        //     * 搜索内容：http://eww.com/，搜索URL：http://eww.com/，搜索时间：2016/11/21 15:16:25
        //     * 搜索内容：uc云，搜索URL：，搜索时间：2016/11/22 10:20:02
        //     * 搜索内容：http://txz.qq.com/p?k=k56rMX99*jCRbRpepK480QKScgOIXsgj&f=716027609，搜索URL：http://txz.qq.com/p?k=k56rMX99*jCRbRpepK480QKScgOIXsgj&f=716027609，搜索时间：2016/11/22 10:22:23
        //     * 搜索内容：http://mydiskm.uc.cn，搜索URL：http://mydiskm.uc.cn，搜索时间：2016/11/22 10:24:20
        //     * 搜索内容：酷狗音乐和音乐在一起，搜索URL：http://m.kugou.com，搜索时间：2016/11/22 10:44:18
        //     * **/

        //    var items = new List<SearchHistory>();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(Input_historyFile))
        //        {
        //            return items;
        //        }

        //        string filePath = Input_historyFile + "_Decrypted";

        //        int result = UCBrowseDllCore.UcFileDecrypted(Input_historyFile, filePath);
        //        if (result != 0)
        //        {
        //            return items;
        //        }

        //        byte[] bytes = System.Utility.Helper.File.FileToBytes(filePath);//this.LocalSourcePath[1]);
        //        if (!bytes.IsValid())
        //        {
        //            return items;
        //        }

        //        int offset = 0;
        //        while (true)
        //        {
        //            // 每条记录标记
        //            int index = offset = Blob.FindBytes(bytes, offset, SEARCHHISTORY_ROW_FLAG);
        //            if (index <= 0)
        //            {
        //                break;
        //            }

        //            // 修改偏移量：记录标记长度8bit[十六进制]
        //            offset += 8;

        //            string search_context = string.Empty;
        //            string search_url = string.Empty;
        //            DateTime? search_datetime = null;

        //            // 修改偏移量：固定长度32bit[十六进制]，偏移量50[十进制]
        //            offset += 50;

        //            #region [--------------------搜索内容[Begin]--------------------------------]
        //            // 搜索内容长度
        //            int[] bytes_data_leng = Blob.GetBytes(bytes, offset, 1);
        //            // 修改偏移量：搜索内容长度1bit
        //            offset += 1;
        //            if (bytes_data_leng.IsValid() && bytes_data_leng[0] > 0)
        //            {
        //                // 搜索内容
        //                int data_leng = bytes_data_leng[0];
        //                int[] bytes_Context = Blob.GetBytes(bytes, offset, data_leng);
        //                // 修改偏移量：搜索内容长度
        //                offset += data_leng;
        //                if (bytes_Context.IsValid())
        //                {
        //                    search_context = Blob.ToString(bytes_Context);
        //                }
        //            }
        //            #endregion [--------------------搜索内容[End]--------------------------------]

        //            #region [--------------------搜索URL[Begin]---------------------------------]
        //            // 搜索URL标记
        //            index = Blob.FindBytes(bytes, offset, SEARCHHISTORY_URL_FLAG);
        //            if (index > 0)
        //            {
        //                int tempoffset = offset + 9;

        //                // 搜索URL长度
        //                bytes_data_leng = Blob.GetBytes(bytes, tempoffset, 1);
        //                // 修改偏移量：搜索URL长度
        //                tempoffset += 1;
        //                if (bytes_data_leng.IsValid() && bytes_data_leng[0] > 0)
        //                {
        //                    // 搜索内容
        //                    int data_leng = bytes_data_leng[0];
        //                    int[] bytes_url = Blob.GetBytes(bytes, tempoffset, data_leng);
        //                    search_url = Blob.ToString(bytes_url);
        //                    if (search_url.IsValid())
        //                    {
        //                        tempoffset += data_leng;
        //                    }

        //                    offset = tempoffset;
        //                }
        //            }
        //            #endregion [--------------------搜索URL[End]---------------------------------]

        //            // 修改偏移量：固定长度16bit[十六进制]，偏移量22[十进制]
        //            offset += 22;

        //            #region [--------------------搜索时间[Begin]--------------------------------]
        //            // 搜索时间
        //            int[] bytes_time = Blob.GetBytes(bytes, offset, 6);
        //            string time = string.Empty;
        //            bytes_time.ForEach(t => { time += string.Format("{0:X2}", t); });
        //            // 修改偏移量：搜索时间6bit
        //            offset += 6;
        //            if (time.IsValid() && System.Convert.ToInt64(time, 16) >= 10)
        //            {
        //                long long_time = System.Convert.ToInt64(time, 16);
        //                search_datetime = DynamicConvert.ToSafeDateTime(long_time.ToString().Substring(0, 10));
        //            }
        //            #endregion [--------------------搜索时间L[End]--------------------------------]

        //            items.Add(new SearchHistory()
        //            {
        //                DataState = EnumDataState.None,
        //                Search_Context = search_context,
        //                Search_Url = search_url,
        //                Search_Datetime = search_datetime
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("读取搜索记录信息发生异常", ex);
        //    }

        //    return items;
        //}

        /// <summary>
        /// 小说书架
        /// 提取路径：/storage/emulated/legacy/UCDownloads/novels/com.UCMobile_catalog
        /// 数据库表：[com.UCMobile_catalog].CATALOG_TABLE
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Novels> GetNovels()
        {
            var items = new List<Novels>();
            try
            {
                //[com.UCMobile_catalog].CATALOG_TABLE
                string dataDir2 = Path.Combine(DownloadsPath, "novels\\com.UCMobile_catalog");
                if (!dataDir2.IsValid())
                {
                    return items;
                }

                using (SqliteContext _sqlite1 = new SqliteContext(dataDir2))
                {
                    var list_dynamic1 = _sqlite1.FindByName("CATALOG_TABLE");

                    // 获取需要恢复的表名称
                    string tableNames = "CATALOG_TABLE";
                    list_dynamic1.ForEach(t => { tableNames += ("," + t.catalog_table_name); });
                    var newPath = SqliteRecoveryHelper.DataRecovery(dataDir2, "com.UCMobile_catalog.charactor", tableNames);

                    using (SqliteContext _sqlite2 = new SqliteContext(newPath))
                    {
                        Novels obj;

                        var list_dynamic2 = _sqlite2.FindByName("CATALOG_TABLE");
                        foreach (var item in list_dynamic2)
                        {
                            obj = new Novels()
                            {
                                Book_ID = item.book_id,
                                Novel_Name = item.novel_name,
                                Novel_Author = item.novel_author,
                                Catalog_Table_Name = item.catalog_table_name,
                                Expire_Time = item.expire_time,
                                Update_Time = DynamicConvert.ToSafeDateTime(item.update_time),
                                DataState = DynamicConvert.ToEnumByValue(item.XLY_DataType, EnumDataState.None)
                            };

                            var list_dynamic_summery = _sqlite2.Find(new SQLiteString(string.Format("select f.id,f.chapter_name,update_time from {0} f order by f.id", obj.Catalog_Table_Name)));
                            if (list_dynamic_summery.IsValid())
                            {
                                obj.Summary = string.Format("{0}章", list_dynamic_summery.Count());

                                StringBuilder sbSummery = new StringBuilder();
                                sbSummery.Append("-------------------------------------------------------------------");
                                foreach (var sum in list_dynamic_summery)
                                {
                                    sbSummery.AppendFormat("\r\n{0}，更新时间：{1}", sum.chapter_name, DynamicConvert.ToSafeDateTime(sum.update_time));
                                }
                                sbSummery.Append("\r\n-------------------------------------------------------------------");

                                obj.SummaryDetial = sbSummery.ToString();
                            }
                            items.Add(obj);
                        }
                    }
                }
            }
            catch
            {
            }
            return items;
        }

        #region // 订阅号实体_1

        protected class RawData_1
        {
            /// <summary>
            /// Data
            /// </summary>
            public List<Data_1> data { get; set; }
            /// <summary>
            /// Metadata
            /// </summary>
            //public Metadata metadata { get; set; }
        }

        protected class Msgs
        {
            /// <summary>
            /// 5252d82d63e54598938fe3bc075c581e
            /// </summary>
            //public string object_id { get; set; }
            /// <summary>
            /// Display_type
            /// </summary>
            //public string display_type { get; set; }
            /// <summary>
            /// 刘恒: 史上第一个靠装傻充愣登台的皇帝!
            /// </summary>
            public string article_title { get; set; }
            /// <summary>
            /// 若要问中国古代第一个盛世叫什么，那绝对是文景之治！汉文帝刘恒（前203年—前157年），汉高祖刘邦第四子，母薄姬，西汉第
            /// </summary>
            public string article_sub_title { get; set; }
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/16120709094973c6a599d9d6f37ba4a6162ff9e730x605x375x46.jpeg
            /// </summary>
            public string article_pic_url { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/article.html?from=msg&uc_param_str=frdnsnpfvecpntnwprdsssnikt#!wm_aid=5252d82d63e54598938fe3bc075c581e!!wm_id=4f482f58467d4dd4bf09fc267de786aa
            /// </summary>
            public string article_url { get; set; }
            /// <summary>
            /// Article_publish_time
            /// </summary>
            public string article_publish_time { get; set; }
            /// <summary>
            /// Object_type
            /// </summary>
            //public string object_type { get; set; }
        }

        protected class Data_1
        {
            /// <summary>
            /// 1b825b1fea6242b78f80374cb286c36a
            /// </summary>
            //public string msg_id { get; set; }
            /// <summary>
            /// Msg_type
            /// </summary>
            //public string msg_type { get; set; }
            /// <summary>
            /// Created_time
            /// </summary>
            public string created_time { get; set; }
            /// <summary>
            /// Msgs
            /// </summary>
            public List<Msgs> msgs { get; set; }
            /// <summary>
            /// Pos
            /// </summary>
            //public string pos { get; set; }
            /// <summary>
            /// 1481075176307
            /// </summary>
            //public string pos_str { get; set; }
        }

        protected class Metadata
        {
            /// <summary>
            /// 128029699004694528
            /// </summary>
            //public string last_command_pos_str { get; set; }
            /// <summary>
            /// Last_command_pos
            /// </summary>
            //public string last_command_pos { get; set; }
            /// <summary>
            /// dw07fe5f814f34f3e80f8653be32db8c99
            /// </summary>
            //public string user_id { get; set; }
            /// <summary>
            /// Is_whole
            /// </summary>
            //public string is_whole { get; set; }
        }

        protected class Root_1
        {
            /// <summary>
            /// RawData
            /// </summary>
            public RawData_1 rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }
        #endregion

        #region // 订阅号实体类_2
        protected class RawData_2
        {
            /// <summary>
            /// Data
            /// </summary>
            public Data_2 data { get; set; }
            /// <summary>
            /// Metadata
            /// </summary>
            //public Metadata metadata { get; set; }
        }

        protected class Sub_button
        {
            /// <summary>
            /// 歷史騙局
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// Type
            /// </summary>
            //public string type { get; set; }
            /// <summary>
            /// http://api.mp.uc.cn/api/v1/users/proxy/wemedias/4f482f58467d4dd4bf09fc267de786aa?uc_biz_str=S%3Acustom%7CC%3Aiflow_wm2&pos=11&name=%E5%8E%86%E5%8F%B2%E7%8C%9B%E6%96%87&bver=2016120415&subname=%E6%AD%B7%E5%8F%B2%E9%A8%99%E5%B1%80&uc_param_str=frdnsnpfvecpntnwprdsssnikt&redirect=http%3A%2F%2Fa.mp.uc.cn%2Farticle.html%3Ffrom%3Dbutton%23%21wm_aid%3Db9be0557258e4cfab1281a54aee04892%21%21wm_id%3D4f482f58467d4dd4bf09fc267de786aa
            /// </summary>
            public string url { get; set; }
        }

        protected class Buttons
        {
            /// <summary>
            /// 历史猛文
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// Type
            /// </summary>
            //public string type { get; set; }
            /// <summary>
            /// Sub_button
            /// </summary>
            public List<Sub_button> sub_button { get; set; }
        }

        protected class Columns
        {
            /// <summary>
            /// 4a8c5811b826ba467d0f34d74774b7a9
            /// </summary>
            //public string col_id { get; set; }
            /// <summary>
            /// 穿越一下
            /// </summary>
            //public string col_name { get; set; }
            /// <summary>
            /// Col_type
            /// </summary>
            //public string col_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            //public string col_pic_url { get; set; }
            /// <summary>
            /// Col_entity_numbs
            /// </summary>
            //public string col_entity_numbs { get; set; }
        }

        protected class Hot_articles
        {
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/16120609353024911a68d2030354d755319a259631x375x375x20.jpeg
            /// </summary>
            public string article_pic_url { get; set; }
            /// <summary>
            /// Article_publish_time
            /// </summary>
            public string article_publish_time { get; set; }
            /// <summary>
            /// 生活在北京的人对一些北京地标耳熟能详，故宫、鸟巢、国家大剧院、央视新址……对一些著名单位也多有了解，比如随口能说公安部在
            /// </summary>
            public string article_sub_title { get; set; }
            /// <summary>
            /// 北京不挂牌神秘单位, 你懂几个?
            /// </summary>
            public string article_title { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/article.html?from=frontpage_hot_articles&uc_param_str=frdnsnpfvecpntnwprdsssnikt#!wm_aid=4a35e07a6e884c70a2c25c3ff5ea8dd1!!wm_id=4f482f58467d4dd4bf09fc267de786aa
            /// </summary>
            public string article_url { get; set; }
            /// <summary>
            /// Display_type
            /// </summary>
            //public string display_type { get; set; }
            /// <summary>
            /// 4a35e07a6e884c70a2c25c3ff5ea8dd1
            /// </summary>
            //public string object_id { get; set; }
            /// <summary>
            /// Pos
            /// </summary>
            //public string pos { get; set; }
            /// <summary>
            /// 1480989123597
            /// </summary>
            //public string pos_str { get; set; }
            /// <summary>
            /// Read_times
            /// </summary>
            public string read_times { get; set; }
        }

        protected class Data_2
        {
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/161116083244d33e4b5f8e94552002678ffb42346bx200x200x10.png
            /// </summary>
            public string avatar_url { get; set; }
            /// <summary>
            /// Buttons
            /// </summary>
            public List<Buttons> buttons { get; set; }
            /// <summary>
            /// Columns
            /// </summary>
            //public List<Columns> columns { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contact { get; set; }
            /// <summary>
            /// 蹲马桶时来点历史，拉的舒坦！
            /// </summary>
            public string introduction { get; set; }
            /// <summary>
            /// 
            /// </summary>
            //public string location { get; set; }
            /// <summary>
            /// Status
            /// </summary>
            //public string status { get; set; }
            /// <summary>
            /// 来了坐！每天猛料历史等你揭秘～
            /// </summary>
            public string welcome { get; set; }
            /// <summary>
            /// 马桶历史
            /// </summary>
            public string wm_name { get; set; }
            /// <summary>
            /// 4f482f58467d4dd4bf09fc267de786aa
            /// </summary>
            //public string wm_id { get; set; }
            /// <summary>
            /// Is_default
            /// </summary>
            //public string is_default { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/media.html?mid=4f482f58467d4dd4bf09fc267de786aa&client=ucweb&uc_param_str=frdnsnpfvecpntnwprdsssnikt&uc_biz_str=S:custom%7CC:iflow_ncmt
            /// </summary>
            public string homepage_url { get; set; }
            /// <summary>
            /// Type
            /// </summary>
            //public string type { get; set; }
            /// <summary>
            /// Hot_articles
            /// </summary>
            public List<Hot_articles> hot_articles { get; set; }
        }

        protected class Metadata_2
        {
            /// <summary>
            /// dw07fe5f814f34f3e80f8653be32db8c99
            /// </summary>
            //public string user_id { get; set; }
            /// <summary>
            /// Is_followed
            /// </summary>
            //public string is_followed { get; set; }
        }

        protected class Root_2
        {
            /// <summary>
            /// RawData
            /// </summary>
            public RawData_2 rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }
        #endregion

        #region // 订阅号实体_3
        protected class Hot_articles_3
        {
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/16120110030ffd0a54d6a9c8c1e4c81079fd61ec0bx453x340x20.jpeg
            /// </summary>
            public string article_pic_url { get; set; }
            /// <summary>
            /// Article_publish_time
            /// </summary>
            public string article_publish_time { get; set; }
            /// <summary>
            /// 小时候生活在老家的农村里，那个地方还是比较落后的那种小村。我们那交情不错的人家会给孩子定娃娃亲，仿佛一种传下来的风俗，又
            /// </summary>
            public string article_sub_title { get; set; }
            /// <summary>
            /// 弟弟替我娶了定娃娃亲的农村丫头, 婚礼上见到她我十分后悔
            /// </summary>
            public string article_title { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/article.html?from=frontpage_hot_articles&uc_param_str=frdnsnpfvecpntnwprdsssnikt#!wm_aid=bb808e5137d24b55a9e7ea6d6458c144!!wm_id=49ad95c4c8b643b2b509bc97a5bc2c9e
            /// </summary>
            public string article_url { get; set; }
            /// <summary>
            /// Display_type
            /// </summary>
            //public string display_type { get; set; }
            /// <summary>
            /// bb808e5137d24b55a9e7ea6d6458c144
            /// </summary>
            //public string object_id { get; set; }
            /// <summary>
            /// Pos
            /// </summary>
            //public string pos { get; set; }
            /// <summary>
            /// 1480570148289
            /// </summary>
            //public string pos_str { get; set; }
            /// <summary>
            /// Read_times
            /// </summary>
            public string read_times { get; set; }
        }

        protected class Data_3
        {
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016919/16091916392ce626e2b21abff3b8c9401b00b0a4a9x200x200x29.png
            /// </summary>
            public string avatar_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            //public string contact { get; set; }
            /// <summary>
            /// 倾听身边人的情感故事，感悟百态人生。
            /// </summary>
            public string introduction { get; set; }
            /// <summary>
            /// 
            /// </summary>
            //public string location { get; set; }
            /// <summary>
            /// Status
            /// </summary>
            //public string status { get; set; }
            /// <summary>
            /// 您好，欢迎关注“情感小树林”共同聆听你我身边的故事！
            /// </summary>
            public string welcome { get; set; }
            /// <summary>
            /// 情感小树林
            /// </summary>
            public string wm_name { get; set; }
            /// <summary>
            /// 49ad95c4c8b643b2b509bc97a5bc2c9e
            /// </summary>
            //public string wm_id { get; set; }
            /// <summary>
            /// Is_default
            /// </summary>
            //public string is_default { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/media.html?mid=49ad95c4c8b643b2b509bc97a5bc2c9e&client=ucweb&uc_param_str=frdnsnpfvecpntnwprdsssnikt&uc_biz_str=S:custom%7CC:iflow_ncmt
            /// </summary>
            public string homepage_url { get; set; }
            /// <summary>
            /// Type
            /// </summary>
            //public string type { get; set; }
            /// <summary>
            /// Hot_articles
            /// </summary>
            public List<Hot_articles_3> hot_articles { get; set; }
        }

        protected class Metadata_3
        {
            /// <summary>
            /// Followed_time
            /// </summary>
            //public string followed_time { get; set; }
            /// <summary>
            /// dw07fe5f814f34f3e80f8653be32db8c99
            /// </summary>
            //public string user_id { get; set; }
            /// <summary>
            /// Is_followed
            /// </summary>
            //public string is_followed { get; set; }
        }

        protected class RawData_3
        {
            /// <summary>
            /// Data
            /// </summary>
            public Data_3 data { get; set; }
            /// <summary>
            /// Metadata
            /// </summary>
            //public Metadata_3 metadata { get; set; }
        }

        protected class Root_3
        {
            /// <summary>
            /// RawData
            /// </summary>
            public RawData_3 rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }

        #endregion

        #region // 订阅实体_5
        protected class RawData_5
        {
            /// <summary>
            /// Unread_msg_count
            /// </summary>
            //public string unread_msg_count { get; set; }
            /// <summary>
            /// Latest_msg_time
            /// </summary>
            public string latest_msg_time { get; set; }
            /// <summary>
            /// 2c35e489307d4b57b401a8a6ae05a458
            /// </summary>
            //public string wm_id { get; set; }
            /// <summary>
            /// 128371411474449408
            /// </summary>
            //public string pos_str { get; set; }
            /// <summary>
            /// Unread_letter_count
            /// </summary>
            //public string unread_letter_count { get; set; }
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/1612021803521d8096cf2863a91aea113cc49ab87dx200x200x32.png
            /// </summary>
            public string avatar_url { get; set; }
            /// <summary>
            /// Is_default
            /// </summary>
            //public string is_default { get; set; }
            /// <summary>
            /// Pos
            /// </summary>
            //public string pos { get; set; }
            /// <summary>
            /// 虎口夺金
            /// </summary>
            public string wm_name { get; set; }
            /// <summary>
            /// 老兵隐姓埋名40年 真实身份震惊总参
            /// </summary>
            public string latest_article_title { get; set; }
            /// <summary>
            /// Type
            /// </summary>
            //public string type { get; set; }
        }

        protected class Root_5
        {
            /// <summary>
            /// RawData
            /// </summary>
            public List<RawData_5> rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }
        #endregion

        #region // 订阅实体_6

        protected class Site_logo_6
        {
            /// <summary>
            /// Id
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// Style
            /// </summary>
            public string style { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string desc { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string link { get; set; }
            /// <summary>
            /// Img
            /// </summary>
            public Img_6 img { get; set; }
        }

        protected class News_poi_mark_6
        {
        }

        protected class Img_6
        {
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// Width
            /// </summary>
            public string width { get; set; }
            /// <summary>
            /// Height
            /// </summary>
            public string height { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// Preload
            /// </summary>
            public string preload { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string daoliu_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string daoliu_title { get; set; }
        }

        protected class Author_icon_6
        {
            /// <summary>
            /// http://image.zzd.sm.cn/16604464544417616519.jpg?id=0
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// Width
            /// </summary>
            public string width { get; set; }
            /// <summary>
            /// Height
            /// </summary>
            public string height { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// Preload
            /// </summary>
            public string preload { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string daoliu_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string daoliu_title { get; set; }
        }

        protected class Wemedias_6
        {
            /// <summary>
            /// 
            /// </summary>
            public string subhead { get; set; }
            /// <summary>
            /// Grab_time
            /// </summary>
            public string grab_time { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string title_icon { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string editor_nickname { get; set; }
            /// <summary>
            /// Politics
            /// </summary>
            public string politics { get; set; }
            /// <summary>
            /// Site_logo
            /// </summary>
            public Site_logo_6 site_logo { get; set; }
            /// <summary>
            /// f4eb9101d32c47e39e27d815e842f25f
            /// </summary>
            public string wm_id { get; set; }
            /// <summary>
            /// Item_type
            /// </summary>
            public string item_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string post_like_url { get; set; }
            /// <summary>
            /// 分享与讨论实用的婚姻生活知识，共享优质生活！
            /// </summary>
            public string summary { get; set; }
            /// <summary>
            /// Style_type
            /// </summary>
            public string style_type { get; set; }
            /// <summary>
            /// Follower_cnt
            /// </summary>
            public string follower_cnt { get; set; }
            /// <summary>
            /// Enable_dislike
            /// </summary>
            public bool enable_dislike { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string app_download_desc { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string editor_icon { get; set; }
            /// <summary>
            /// Dislike_infos
            /// </summary>
            public List<string> dislike_infos { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_mark { get; set; }
            /// <summary>
            /// 婚姻那些事
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_info { get; set; }
            /// <summary>
            /// Hyperlinks
            /// </summary>
            public List<string> hyperlinks { get; set; }
            /// <summary>
            /// Author_icon
            /// </summary>
            public Author_icon_6 author_icon { get; set; }
            /// <summary>
            /// Op_mark_icolor
            /// </summary>
            public string op_mark_icolor { get; set; }
            /// <summary>
            /// News_poi_mark
            /// </summary>
            public News_poi_mark_6 news_poi_mark { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/media?mid=f4eb9101d32c47e39e27d815e842f25f&uc_biz_str=S:custom|C:iflow_ncmt&uc_param_str=frdnsnpfvecplabtbmntnwpvsslibieisiniprds
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// Is_drop_down_style
            /// </summary>
            public bool is_drop_down_style { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/media?mid=f4eb9101d32c47e39e27d815e842f25f&uc_biz_str=S:custom|C:iflow_ncmt&uc_param_str=frdnsnpfvecplabtbmntnwpvsslibieisiniprds
            /// </summary>
            public string home_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string download_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string reco_desc { get; set; }
            /// <summary>
            /// 两性情感圈热门关注
            /// </summary>
            public string desc { get; set; }
            /// <summary>
            /// Clickable_url
            /// </summary>
            public bool clickable_url { get; set; }
            /// <summary>
            /// Strategy
            /// </summary>
            public string strategy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string post_dislike_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string app_download_url { get; set; }
            /// <summary>
            /// 婚姻那些事
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_mark_iurl { get; set; }
            /// <summary>
            /// 14250768370492828945
            /// </summary>
            public string recoid { get; set; }
            /// <summary>
            /// 23285
            /// </summary>
            public string id { get; set; }
            public string wm_name { get; set; }
            public string rank_msg { get; set; }
            public string rank_star { get; set; }
            public string latest_article_title { get; set; }
            public string avatar_url { get; set; }
        }

        protected class RawData_6
        {
            /// <summary>
            /// 
            /// </summary>
            public string subhead { get; set; }
            /// <summary>
            /// Grab_time
            /// </summary>
            public string grab_time { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string title_icon { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string editor_nickname { get; set; }
            /// <summary>
            /// Change_fold_count
            /// </summary>
            public string change_fold_count { get; set; }
            /// <summary>
            /// Content_type
            /// </summary>
            public string content_type { get; set; }
            /// <summary>
            /// Politics
            /// </summary>
            public string politics { get; set; }
            /// <summary>
            /// Site_logo
            /// </summary>
            public Site_logo_6 site_logo { get; set; }
            /// <summary>
            /// Item_type
            /// </summary>
            public string item_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string post_like_url { get; set; }
            /// <summary>
            /// Style_type
            /// </summary>
            public string style_type { get; set; }
            /// <summary>
            /// Enable_dislike
            /// </summary>
            public bool enable_dislike { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string app_download_desc { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string editor_icon { get; set; }
            /// <summary>
            /// Dislike_infos
            /// </summary>
            public List<string> dislike_infos { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_mark { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_info { get; set; }
            /// <summary>
            /// Hyperlinks
            /// </summary>
            public List<string> hyperlinks { get; set; }
            /// <summary>
            /// Op_mark_icolor
            /// </summary>
            public string op_mark_icolor { get; set; }
            /// <summary>
            /// News_poi_mark
            /// </summary>
            public News_poi_mark_6 news_poi_mark { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// Is_drop_down_style
            /// </summary>
            public bool is_drop_down_style { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string download_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string reco_desc { get; set; }
            /// <summary>
            /// Clickable_url
            /// </summary>
            public bool clickable_url { get; set; }
            /// <summary>
            /// Strategy
            /// </summary>
            public string strategy { get; set; }
            /// <summary>
            /// Is_fold
            /// </summary>
            public bool is_fold { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string post_dislike_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string app_download_url { get; set; }
            /// <summary>
            /// 推荐订阅号
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string enter_desc { get; set; }
            /// <summary>
            /// Tags
            /// </summary>
            public List<string> tags { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string op_mark_iurl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string recoid { get; set; }
            /// <summary>
            /// 14669876842315109551
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// Wemedias
            /// </summary>
            public List<Wemedias_6> wemedias { get; set; }
            /// <summary>
            /// Articles
            /// </summary>
            public List<string> articles { get; set; }
        }

        protected class Root_6
        {
            /// <summary>
            /// RawData
            /// </summary>
            public List<RawData_6> rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }
        #endregion

        #region // 订阅实体_7
        public class Data_7
        {
            /// <summary>
            /// http://image.uc.cn/s/wemedia/s/upload/2016/161124183650c5227f2c786e1e897929642f18f9b8x688x314x33.jpeg
            /// </summary>
            public string article_pic_url { get; set; }
            /// <summary>
            /// Article_publish_time
            /// </summary>
            public string article_publish_time { get; set; }
            /// <summary>
            /// Thanksgiving Day11月24日是感恩节渐渐地，这个日子不单是一个欢聚的理由它提醒我们再忙，再累不要吝惜自己
            /// </summary>
            public string article_sub_title { get; set; }
            /// <summary>
            /// 感恩节|一份薄礼, 感恩有你。
            /// </summary>
            public string article_title { get; set; }
            /// <summary>
            /// http://a.mp.uc.cn/article.html?from=column&uc_param_str=frdnsnpfvecpntnwprdsssnikt#!wm_aid=294eee7625044971856afa681b021c2e!!wm_id=18b01a903acc4670bd6e794eedb98014
            /// </summary>
            public string article_url { get; set; }
            /// <summary>
            /// Display_type
            /// </summary>
            public string display_type { get; set; }
            /// <summary>
            /// 294eee7625044971856afa681b021c2e
            /// </summary>
            public string object_id { get; set; }
            /// <summary>
            /// Pos
            /// </summary>
            public string pos { get; set; }
            /// <summary>
            /// 1479987986335
            /// </summary>
            public string pos_str { get; set; }
            /// <summary>
            /// Read_times
            /// </summary>
            public string read_times { get; set; }
        }

        public class Metadata_7
        {
            /// <summary>
            /// dw07fe5f814f34f3e80f8653be32db8c99
            /// </summary>
            public string user_id { get; set; }
            /// <summary>
            /// Is_whole
            /// </summary>
            public string is_whole { get; set; }
        }

        public class RawData_7
        {
            /// <summary>
            /// Data
            /// </summary>
            public List<Data_7> data { get; set; }
            /// <summary>
            /// Metadata
            /// </summary>
            public Metadata_7 metadata { get; set; }
        }

        public class Root_7
        {
            /// <summary>
            /// RawData
            /// </summary>
            public RawData_7 rawData { get; set; }
            /// <summary>
            /// Expires
            /// </summary>
            public string expires { get; set; }
        }
        #endregion

        /// <summary>
        /// 从字节数组中查询子数组，未找到返回-1
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">起始查询偏移</param>
        /// <param name="subBytes">子数组</param>
        /// <returns>查询到则返回序号，否则返回-1</returns>
        private int FindBytes(byte[] blob, int offset, byte[] subBytes)
        {
            return GetIndexOf(blob, offset, subBytes);
        }

        /// <summary>
        /// 查询子数组
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="subBytes"></param>
        /// <returns></returns>
        private int GetIndexOf<T1, T2>(T1[] buf, int offset, T2[] subBytes) where T1 : IComparable
        {
            if (buf == null || subBytes == null || buf.Length == 0 || subBytes.Length == 0 || buf.Length < subBytes.Length)
                return -1;
            if (offset < 0)
            {
                offset = 0;
            }
            if (offset + subBytes.Length > buf.Length)
                return -1;

            int i, j;
            if (typeof(T1) == typeof(T2))
            {
                for (i = offset; i < buf.Length - subBytes.Length + 1; i++)
                {
                    if (buf[i].CompareTo(subBytes[0]) == 0)
                    {
                        for (j = 1; j < subBytes.Length; j++)
                        {
                            if (buf[i + j].CompareTo(subBytes[j]) != 0)
                                break;
                        }
                        if (j == subBytes.Length)
                            return i;
                    }
                }
            }
            else
            {
                for (i = offset; i < buf.Length - subBytes.Length + 1; i++)
                {
                    if (buf[i].CompareTo(System.Convert.ChangeType(subBytes[0], typeof(T1))) == 0)
                    {
                        for (j = 1; j < subBytes.Length; j++)
                        {
                            if (buf[i + j].CompareTo(System.Convert.ChangeType(subBytes[j], typeof(T1))) != 0)
                                break;
                        }
                        if (j == subBytes.Length)
                            return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 从字节数组中截取子数组
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        private int[] GetBytes(byte[] blob, int offset, int length)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset + length > blob.Length)       //超过文件长度
            {
                return new int[0];
            }
            int[] bs2 = new int[length];
            Array.Copy(blob, offset, bs2, 0, bs2.Length);
            return bs2;
        }

        /// <summary>
        /// 字节数组转字符串，编码格式自定义，比如"utf-8"、"ascii"、"base64"
        /// </summary>
        /// <param name="blob">需要转换的字节数组</param>
        /// <param name="code">编码格式,比如"utf-8"、"ascii"</param>
        /// <returns></returns>
        private string ToString(int[] blob, string code)
        {
            if (code.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToBase64String(blob.ToList().ConvertAll(s => (byte)s).ToArray());
            }
            else
            {
                Encoding c = Encoding.GetEncoding(code);
                return c.GetString(blob.ToList().ConvertAll(s => (byte)s).ToArray());
            }
        }

    }
}
