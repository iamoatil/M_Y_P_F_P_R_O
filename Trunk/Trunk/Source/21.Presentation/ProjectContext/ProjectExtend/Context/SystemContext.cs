using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Models.Entities;
using System.Windows.Data;

/*
 * 创建人：Bob
 * 创建时间：2017/9/4
 * 
 * 说明：当前系统信息，用于存放当前项目中公用信息
 * 
 * 目前内容：
 *      1.当前系统保存文件的基础路径
 * 
 */

namespace ProjectExtend.Context
{
    public partial class SystemContext : NotifyPropertyBase
    {
        #region 基础定义

        /// <summary>
        /// 系统配置文件服务
        /// </summary>
        private ISystemConfigService _configService;
        /// <summary>
        /// 数据库服务
        /// </summary>
        private ILogicalDataContext _dbService;
        /// <summary>
        /// 已使用过的盘符
        /// </summary>
        private List<string> _usedPathRoot;

        #endregion

        #region 单例构建

        private volatile static SystemContext _instance;

        private static object _objLock = new object();

        static SystemContext()
        {
            Settings = IocManagerSingle.Instance.GetPart<ISettings>();
            LanguageProvider = new XmlDataProvider { XPath = "LanguageResource" };
            LanguageManager = LanguageManager.Current;
            LanguageManager.Switched += (a, b) => LanguageProvider.Document = LanguageManager.Document;
        }

        private SystemContext()
        {
            _usedPathRoot = new List<string>();
            _configService = IocManagerSingle.Instance.GetPart<ISystemConfigService>(CoreExportKeys.SysConfigHelper);
            _dbService = IocManagerSingle.Instance.GetPart<ILogicalDataContext>();
            CurCacheViews = new XLY.SF.Framework.Core.Base.MessageBase.Navigation.NavigationCacheManager<XLY.SF.Project.ViewDomain.Model.PresentationNavigationElement.PreCacheToken>();

            //获取默认文件夹
            if (String.IsNullOrWhiteSpace(SaveDefaultFolderName))
            {
                SaveDefaultFolderName = @"XLY\SpfData";
            }
        }

        /// <summary>
        /// 当前系统信息实例
        /// </summary>
        public static SystemContext Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new SystemContext();
                return _instance;
            }
        }

        public static ISettings Settings { get; }

        #endregion
    }
}
