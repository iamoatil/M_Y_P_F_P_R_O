/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 14:20:41 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓QQ浏览器数据解析
    /// </summary>
    public class AndroidQQBrowseDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 安卓QQ浏览器数据解析
        /// </summary>
        public AndroidQQBrowseDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{4BF9F483-AC87-47BD-9E46-7B5B3CE8D25B}";
            pluginInfo.Name = "QQ浏览器";
            pluginInfo.Group = "Web痕迹";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.1";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 3;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "com.tencent.mtt";
            pluginInfo.Icon = "\\icons\\IOS_QQBrowser.png";
            pluginInfo.Description = "提取安卓设备QQ浏览器信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mtt/databases/database");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mtt/databases/default_user.db");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mtt/databases/webview_x5.db");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mtt/databases/webviewCache_x5.db");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;
                var databaseFilePath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValid(databaseFilePath))
                {
                    return ds;
                }

                BuildData(ds, pi.SaveDbPath, pi.SourcePath[0].Local, pi.SourcePath[1].Local, pi.SourcePath[2].Local, pi.SourcePath[3].Local);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓QQ浏览器数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        #region 数据解析

        private void BuildData(TreeDataSource datasource, string dbfilePath, string databaseFilePath, string default_userFilePath, string webview_x5FilePath, string webviewCache_x5FilePath)
        {

            var bookMarkTree = new TreeNode();
            bookMarkTree.Text = "书签";
            bookMarkTree.Type = typeof(BookMark);
            bookMarkTree.Items = new DataItems<BookMark>(dbfilePath);

            var historyTree = new TreeNode();
            historyTree.Text = "历史记录";
            historyTree.Type = typeof(History);
            historyTree.Items = new DataItems<History>(dbfilePath);

            var pwdTree = new TreeNode();
            pwdTree.Text = "站点密码";
            pwdTree.Type = typeof(WebSitPassword);
            pwdTree.Items = new DataItems<WebSitPassword>(dbfilePath);

            var cookieTree = new TreeNode();
            cookieTree.Text = "Cookie";
            cookieTree.Type = typeof(WebCookie);
            cookieTree.Items = new DataItems<WebCookie>(dbfilePath);

            var cacheTree = new TreeNode();
            cacheTree.Text = "缓存";
            cacheTree.Type = typeof(WebCache);
            cacheTree.Items = new DataItems<WebCache>(dbfilePath);

            //书签
            bookMarkTree.Items.AddRange(GetBookmarks(default_userFilePath));

            //历史记录
            historyTree.Items.AddRange(GetHistroies(databaseFilePath));

            //Cookie
            cookieTree.Items.AddRange(GetWebCookies(webview_x5FilePath));

            //缓存
            cacheTree.Items.AddRange(GetWebCaches(webviewCache_x5FilePath));

            datasource.TreeNodes.Add(bookMarkTree);
            datasource.TreeNodes.Add(historyTree);
            datasource.TreeNodes.Add(pwdTree);
            datasource.TreeNodes.Add(cookieTree);
            datasource.TreeNodes.Add(cacheTree);
        }

        private IEnumerable<History> GetHistroies(string databaseFile)
        {
            var histroies = new List<History>();
            if (string.IsNullOrEmpty(databaseFile))
            {
                return histroies;
            }

            string copyfiles = SqliteRecoveryHelper.DataRecovery(databaseFile, @"chalib\Android_QQBrowse_V5.1.0.780\database.charactor", "history");
            var context = new SqliteContext(copyfiles);
            try
            {
                var objList = context.FindByName("history");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        History history = new History();

                        history.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                        history.Name = DynamicConvert.ToSafeString(source.NAME);
                        history.Url = DynamicConvert.ToSafeString(source.URL);
                        history.VisitTime = DynamicConvert.ToSafeDateTime(source.DATETIME);
                        histroies.Add(history);
                    }
                }
            }
            catch
            {
            }

            return histroies;
        }

        private IEnumerable<BookMark> GetBookmarks(string default_userFile)
        {
            var bookmarks = new List<BookMark>();
            if (string.IsNullOrEmpty(default_userFile))
            {
                return bookmarks;
            }

            var copyFiles = SqliteRecoveryHelper.DataRecovery(default_userFile, @"chalib\Andorid_QQBrowse_V5.1.0.780\default_user.charactor", "mtt_bookmarks");
            var context = new SqliteContext(copyFiles);
            try
            {
                var objList = context.FindByName("mtt_bookmarks");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        string url = DynamicConvert.ToSafeString(source.url);
                        if (!url.IsNullOrEmpty())
                        {
                            var mark = new BookMark();

                            mark.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                            mark.Title = DynamicConvert.ToSafeString(source.title);
                            mark.Url = url;
                            mark.IsDeleted = (int)DynamicConvert.ToSafeInt(source.deleted) == 0 ? "正常" : "已删除";
                            mark.CreatedTime = DynamicConvert.ToSafeDateTime(source.created);
                            bookmarks.Add(mark);
                        }
                    }
                }
            }
            catch
            {
            }

            return bookmarks;
        }

        private IEnumerable<WebCookie> GetWebCookies(string webview_x5File)
        {
            var localWebCookie = new List<WebCookie>();
            if (string.IsNullOrEmpty(webview_x5File))
            {
                return localWebCookie;
            }


            var copyFiles = SqliteRecoveryHelper.DataRecovery(webview_x5File, @"chalib\Andorid_QQBrowse_V5.1.0.780\webview_x5.db.charactor", "cookies");
            var context = new SqliteContext(copyFiles);
            try
            {
                var objList = context.FindByName("cookies");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        var cookie = new WebCookie();
                        cookie.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                        cookie.Name = DynamicConvert.ToSafeString(source.name);
                        cookie.Value = DynamicConvert.ToSafeString(source.value);
                        cookie.Domain = DynamicConvert.ToSafeString(source.domain);
                        cookie.ExpresTime = DynamicConvert.ToSafeDateTime(source.expires);
                        localWebCookie.Add(cookie);
                    }
                }
            }
            catch
            {
            }

            return localWebCookie;
        }

        private IEnumerable<WebCache> GetWebCaches(string webviewCache_x5File)
        {
            var qqcWebCache = new List<WebCache>();
            if (string.IsNullOrEmpty(webviewCache_x5File))
            {
                return qqcWebCache;
            }

            var copyFiles = SqliteRecoveryHelper.DataRecovery(webviewCache_x5File, @"chalib\Andorid_QQBrowse_V5.1.0.780\webviewCache_x5.db.charactor", "cache");
            var context = new SqliteContext(copyFiles);
            try
            {
                var objList = context.FindByName("cache");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        var cache = new WebCache();
                        cache.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                        cache.Url = DynamicConvert.ToSafeString(source.url);
                        cache.ExpresTime = DynamicConvert.ToSafeDateTime(source.expires);
                        cache.MimeType = DynamicConvert.ToSafeString(source.mimetype);
                        cache.Encoding = DynamicConvert.ToSafeString(source.encoding);
                        cache.HttpSatus = DynamicConvert.ToSafeInt(source.httpstatus);
                        cache.Contentlength = DynamicConvert.ToSafeString(source.contentlength);
                        qqcWebCache.Add(cache);
                    }
                }
            }
            catch
            {
            }

            return qqcWebCache;
        }

        #endregion
    }
}
