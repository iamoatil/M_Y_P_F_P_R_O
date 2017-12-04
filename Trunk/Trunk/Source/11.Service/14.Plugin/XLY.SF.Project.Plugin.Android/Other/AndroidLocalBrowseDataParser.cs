/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 15:36:47 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓内置浏览器数据提取
    /// </summary>
    public class AndroidLocalBrowseDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 安卓内置浏览器数据提取
        /// </summary>
        public AndroidLocalBrowseDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{78C57F51-EA80-4DC7-9B20-8BA4B822BC0A}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_LocalBrowse);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_WebMark);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 3;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.android.browser";
            pluginInfo.Icon = "\\icons\\browser.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidLocalBrowse);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.browser/databases/browser2.db");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.browser/databases/webviewCookiesChromium.db");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.browser/databases/webview.db");

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

                BuildData(pi.SaveDbPath, ds, pi.SourcePath[0].Local, pi.SourcePath[1].Local, pi.SourcePath[2].Local);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓内置浏览器数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private void BuildData(string dbfilePath, TreeDataSource datasource, string browser2File, string webviewCookiesChromiumFile, string webviewFile)
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

            //书签
            bookMarkTree.Items.AddRange(GetBookmarks(browser2File));

            //历史记录
            historyTree.Items.AddRange(GetHistroies(browser2File));

            //Cookie
            cookieTree.Items.AddRange(GetWebCookies(webviewCookiesChromiumFile));

            //站点密码
            pwdTree.Items.AddRange(GetSitePwds(webviewFile));

            datasource.TreeNodes.Add(bookMarkTree);
            datasource.TreeNodes.Add(historyTree);
            datasource.TreeNodes.Add(pwdTree);
            datasource.TreeNodes.Add(cookieTree);
        }

        private IEnumerable<History> GetHistroies(string browser2File)
        {
            //data/data/com.android.browser/databases/browser2.db
            var histroies = new List<History>();
            if (string.IsNullOrEmpty(browser2File))
            {
                return histroies;
            }

            string copyfiles = SqliteRecoveryHelper.DataRecovery(browser2File, @"chalib\Android_LocalBrowse\browser2.charactor", "history");
            var sqliteContext = new SqliteContext(copyfiles);
            try
            {
                var objList = sqliteContext.FindByName("history");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        var history = new History();
                        history.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                        history.Name = DynamicConvert.ToSafeString(source.title);
                        history.Url = DynamicConvert.ToSafeString(source.url);
                        history.VisitTime = DynamicConvert.ToSafeDateTime(source.date);
                        history.Visits = DynamicConvert.ToSafeString(source.visits);
                        histroies.Add(history);
                    }
                }
            }
            catch
            {
            }

            return histroies;
        }

        private IEnumerable<BookMark> GetBookmarks(string browser2File)
        {
            //data/data/com.android.browser/databases/browser2.db
            var bookmarks = new List<BookMark>();
            if (string.IsNullOrEmpty(browser2File))
            {
                return bookmarks;
            }

            string copyfiles = SqliteRecoveryHelper.DataRecovery(browser2File, @"chalib\Android_LocalBrowse\browser2.charactor", "bookmarks");
            var context = new SqliteContext(copyfiles);
            try
            {
                var objList = context.FindByName("bookmarks");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        string url = DynamicConvert.ToSafeString(source.url);
                        if (!url.IsNullOrEmpty())
                        {
                            var book = new BookMark();
                            book.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                            book.Title = DynamicConvert.ToSafeString(source.title);
                            book.Url = url;
                            book.CreatedTime = DynamicConvert.ToSafeDateTime(source.created);
                            book.IsDeleted = DynamicConvert.ToSafeInt(source.deleted) == 0 ? "正常" : "已删除";
                            bookmarks.Add(book);
                        }
                    }
                }
            }
            catch
            {
            }

            return bookmarks;
        }

        private IEnumerable<WebCookie> GetWebCookies(string webviewCookiesChromiumFile)
        {
            //data/data/com.android.browser/databases/webviewCookiesChromium.db
            var localWebCookie = new List<WebCookie>();
            if (string.IsNullOrEmpty(webviewCookiesChromiumFile))
            {
                return localWebCookie;
            }

            string copyfiles = SqliteRecoveryHelper.DataRecovery(webviewCookiesChromiumFile, @"chalib\Android_LocalBrowse\webviewCookiesChromium.charactor", "cookies");
            var context = new SqliteContext(copyfiles);
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
                        cookie.Domain = DynamicConvert.ToSafeString(source.host_key);
                        cookie.ExpresTime = DynamicConvert.ToSafeDateTime(source.expires_utc);
                        localWebCookie.Add(cookie);
                    }
                }
            }
            catch
            {
            }

            return localWebCookie;
        }

        private IEnumerable<WebSitPassword> GetSitePwds(string webviewFile)
        {
            //data/data/com.android.browser/databases/webview.db
            var localSitePwd = new List<WebSitPassword>();
            if (string.IsNullOrEmpty(webviewFile))
            {
                return localSitePwd;
            }

            string copyfiles = SqliteRecoveryHelper.DataRecovery(webviewFile, @"chalib\Android_LocalBrowse\webview.charactor", "password");
            var context = new SqliteContext(copyfiles);
            try
            {
                var objList = context.FindByName("password");
                if (objList != null)
                {
                    foreach (dynamic source in objList)
                    {
                        var pwd = new WebSitPassword();
                        pwd.DataState = DynamicConvert.ToEnumByValue(source.XLY_DataType, EnumDataState.Normal);
                        pwd.Host = DynamicConvert.ToSafeString(source.host);
                        pwd.UserName = DynamicConvert.ToSafeString(source.username);
                        pwd.Password = DynamicConvert.ToSafeString(source.password);

                        localSitePwd.Add(pwd);
                    }
                }
            }
            catch
            {
            }

            return localSitePwd;
        }
    }
}
