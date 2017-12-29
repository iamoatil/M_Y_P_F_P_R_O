using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Models;
using XLY.SF.Project.Plugin.Adapter;

/* ==============================================================================
* Description：EarlyWarningPluginAdapter  
* Author     ：litao
* Create Date：2017/12/2 10:14:15
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    public class EarlyWarningPluginAdapter
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 此适配器支持的所有插件
        /// </summary>
        internal IEnumerable<AbstractEarlyWarningPlugin> Plugins { get; private set; }

        /// <summary>
        /// 关键字预警插件
        /// </summary>
        internal AbstractEarlyWarningPlugin KeyWordPlugin { get; private set; }

        /// <summary>
        ///文件的Md5预警对象
        /// </summary>
        internal Md5WarningManager Md5WarningManager { get; private set; }

        /// <summary>
        /// 总体进度报告器
        /// </summary>
        internal TwoProgressReporter TwoProgressReporter { get; private set; }       

        /// <summary>
        /// 插件进度报告器
        /// </summary>
        private PluginProgressReporter PluginReporter;

        private DbFromConfigData _dbFromConfigData;

        /// <summary>
        /// 当前的所有提取目录
        /// </summary>
        List<ExtractDir> _extractDirs;

        /// <summary>
        /// 停止检测状态
        /// </summary>
        private bool _isStop;

        /// <summary>
        /// 配置数据的过滤
        /// </summary>
        private readonly ConfigDataFilter ConfigDataFilter = new ConfigDataFilter();

        public void Initialize(IRecordContext<Models.Entities.Inspection> setting)
        {
            if(IsInitialized)
            {
                return;
            }
            Md5WarningManager = new Md5WarningManager();

            PluginReporter = new PluginProgressReporter();
            TwoProgressReporter = new TwoProgressReporter();
            TwoProgressReporter.Initialize(PluginReporter, Md5WarningManager.Progress);

            Plugins = PluginAdapter.Instance.GetPluginsByType<EarlyWarningPluginInfo>(PluginType.SpfEarlyWarning).ToList().ConvertAll(p => (AbstractEarlyWarningPlugin)p.Value);
            KeyWordPlugin = Plugins.Where(it => it.PluginInfo is KeyWordEarlyWarningPluginInfo).FirstOrDefault();

            ConfigDataFilter.Initialize(setting);

            IsInitialized = true;
        }

        private void TwoProgressReporter_ProgresssChanged(object sender, IProgressEventArg e)
        {
            ProgressStater stater = e.Parameter as ProgressStater;
            if (stater.State == ProgressState.IsFinished)
            {
                _dbFromConfigData.Reset();
                //拷贝智能预警配置文件到指定的位置
                foreach (var extractDir in _extractDirs)
                {
                    ConfigDataFilter.CopyValidateConfigTo(extractDir.Dir + @"\EarlyWarningConfig\");
                }
                //向指定路径上的配置文件eg：PublicSafety.xml写入数量
                foreach (var extractDir in _extractDirs)
                {
                    if (_isStop)
                    {
                        break;
                    }
                    //获取dbfile，并且初始化DataDotDbFile
                    SqliteDataBaseFile dataDotDbFile = new SqliteDataBaseFile();
                    dataDotDbFile.Initialize(extractDir.DbFile);
                    //获取配置文件
                    string baseDir = extractDir.Dir + @"\EarlyWarningConfig\";
                    ConfigFileDir configFileDir = new ConfigFileDir();
                    configFileDir.Initialize(baseDir);
                    List<ConfigFile> allFiles = configFileDir.GetAllFiles();
                    foreach (var file in allFiles)
                    {
                        int warningCount = 0;
                        int sensitiveId = 0;
                        bool ret = int.TryParse(file.SensitiveId, out sensitiveId);
                        if (ret)
                        {
                            warningCount=dataDotDbFile.GetWarningCount(sensitiveId);
                        }
                        file.SetWarningCount(warningCount);
                    }
                }
                TwoProgressReporter.ProgresssChanged -= TwoProgressReporter_ProgresssChanged;
            }
        }

        /// <summary>
        /// 检测
        /// </summary>
        public void Detect(string sourceDir)
        {
            if (!IsInitialized)
            {
                return;
            }
            _isStop = false;

            //进度报告重置
            TwoProgressReporter.Reset();
            //先根据设置（读取数据库获得设置选项），过滤配置文件中的内容
            ConfigDataFilter.UpdateValidateData();

            // 此处需要修正一下sourceDir
            if (!sourceDir.EndsWith("\\"))
            {
                sourceDir += "\\";
            }
            //把所有提取目录转换为对象
            string[] allSubDirs = Directory.GetDirectories(sourceDir);
            _extractDirs = new List<ExtractDir>();
            foreach (var subDir in allSubDirs)
            {
                ExtractDir extractDir = new ExtractDir();
                extractDir.Initialize(subDir);
                if(extractDir.Initialized)
                {
                    _extractDirs.Add(extractDir);
                    extractDir.LoadDataSource();
                }
            }

            //把过滤后的配置写到数据库configTmp.db文件，并把configTmp.db放在所有提取目录的父目录中
            _dbFromConfigData = new DbFromConfigData();
            _dbFromConfigData.Initialize(sourceDir);
            _dbFromConfigData.GenerateDbFile(ConfigDataFilter.ValidateDataNodes.Select(it => it.SensitiveData));

            //设置插件报告器的插件个数
            int allPluginCount = 0;
            foreach (var item in _extractDirs)
            {
                allPluginCount+=item.DataSources.Count();
            }
            PluginReporter.SetPluginCount(allPluginCount);

            Task.Run(() => 
            {
                TwoProgressReporter.ProgresssChanged += TwoProgressReporter_ProgresssChanged;
                //检测
                PluginDetect(_extractDirs, _dbFromConfigData.DbPath);
                //使用FileMd5Plugin下载文件(异步的)，并且检测文件的Md5
                List<DataNode> lst = ConfigDataFilter.ValidateDataNodes.Where(i => i.SensitiveData.CategoryName == ConstDefinition.FileMd5SubCategory).ToList();
                Md5WarningManager.Intialize(_extractDirs, lst);
                Md5WarningManager.Detect();
            });
        }

        /// <summary>
        /// 停止检测
        /// </summary>
        internal void StopDetect()
        {
            if (!IsInitialized
                || Md5WarningManager == null)
            {
                return;
            }
            _isStop = true;
            Md5WarningManager.StopDetect();
        }

        private void PluginDetect(List<ExtractDir> extractDirs, string configFile)
        {
            foreach (var item in extractDirs)
            {
                Detect(item, configFile);
            }

            PluginReporter.Report(new ProgressStater(ProgressState.IsFinished), 0);
        }

        /// <summary>
        /// 具体检测
        /// </summary>
        private void Detect(ExtractDir extractDir, string configFile)
        {
            if (_isStop)
            {
                return;
            }

            //获取dbfile，并且初始化DataDotDbFile
            SqliteDataBaseFile dataDotDbFile = new SqliteDataBaseFile();
            dataDotDbFile.Initialize(extractDir.DbFile);
            dataDotDbFile.ColumnUpdater.Initialize(dataDotDbFile, configFile);

            //检测提取的数据
            foreach (DeviceDataSource item in extractDir.DataSources)
            {
                if (item.DataSource is FileMd5DataSource)
                {
                    continue;
                }

                if(_isStop)
                {
                    return;
                }
                Match(item, dataDotDbFile.ColumnUpdater);
                
                PluginReporter.Report(new ProgressStater(ProgressState.IsProgressing), -1);
            }

            dataDotDbFile.Dispose();
        }

        /// <summary>
        /// 设置IDevice。此方法只是一个把IDevice传递到 Md5WarningManager.FileDownloader的一个方法
        /// </summary>
        /// <param name="dev"></param>
        public void SetDevice(IDevice dev)
        {
            if (!IsInitialized)
            {
                return;
            }

            Md5WarningManager.FileDownloader.SetDevice(dev);
        }
        
        private void Match(DeviceDataSource ds,  DataDbColumnUpdater columnUpdater)
        {
            IDataSource dataSource = ds.DataSource;           

            //寻找恰当的插件处理
            AbstractEarlyWarningPlugin plugin = Plugins.Where(p => ((EarlyWarningPluginInfo)p.PluginInfo).Match(dataSource)).FirstOrDefault(); 
            if(plugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds);
                plugin.SetColumnUpdater(columnUpdater);
                plugin.Execute(arg,null);
            }
            //所有数据都经过关键字处理
            if(KeyWordPlugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds);
                KeyWordPlugin.SetColumnUpdater(columnUpdater);
                KeyWordPlugin.Execute(arg,null);
            }
        }
    }
}
