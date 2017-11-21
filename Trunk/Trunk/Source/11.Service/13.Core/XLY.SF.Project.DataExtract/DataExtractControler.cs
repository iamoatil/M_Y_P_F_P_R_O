// ***********************************************************************
// Assembly:XLY.SF.Project.DataExtract
// Author:Songbing
// Created:2017-03-28 13:24:44
// Description:数据提取控制服务
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.DataPump;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DataExtract
{
    /// <summary>
    /// 数据提取控制器
    /// </summary>
    public class DataExtractControler
    {
        #region Fields

        private Task _mainTask;

        private CancellationTokenSource _cancelTokenSource;

        private CancellationToken _cancelToken;

        private IEnumerable<ExtractItem> _extractItems;

        private PluginAdapter _pluginAdapter;

        private Int32 _concurrentCount = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 DataExtractControler 实例。
        /// </summary>
        public DataExtractControler()
        {

        }

        #endregion

        #region 基础属性和构造器

        /// <summary>
        /// 提取信息。
        /// </summary>
        public Pump Pump { get; private set; }

        /// <summary>
        /// 多任务进度报告器。
        /// </summary>
        public MultiTaskReporterBase Reporter { get; set; }

        /// <summary>
        /// 当前是否正在执行任务。
        /// </summary>
        public Boolean IsBusy
        {
            get
            {
                Int32 count = Interlocked.CompareExchange(ref _concurrentCount, 0, 0);
                return count != 0;
            }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 开始。
        /// </summary>
        /// <param name="pump">提取信息。</param>
        /// <param name="extractItems">提取项列表。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(Pump pump, ExtractItem[] extractItems)
        {
            if (IsBusy) return;
            if (Initialization(pump, extractItems))
            {
                _concurrentCount = 0;
                _mainTask = Task.Factory.StartNew(() =>
                {
                    ExtractData();
                }, _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 停止数据提取
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (!IsBusy) return;
            //取消
            _cancelTokenSource.Cancel();
            try
            {
                _mainTask.Wait();
            }
            catch (AggregateException)
            {
                //注意：此处只捕获异常但不处理，
                //每个任务各自的异常将通过Reporter
                //报告给观察者
            }
            finally
            {
                _mainTask.Dispose();
                _mainTask = null;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pump">提取信息。</param>
        /// <param name="extractItems">提取项列表。</param>
        private Boolean Initialization(Pump pump, ExtractItem[] extractItems)
        {
            if (pump == null) return false;
            if (extractItems == null || extractItems.Length == 0) return false;
            _pluginAdapter = PluginAdapter.Instance;
            if (_pluginAdapter == null) return false;
            Pump = pump;
            _extractItems = extractItems;
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            FileHelper.CreateExitsDirectory(pump.SourceStorePath);
            Reporter?.Reset();
            return true;
        }

        /// <summary>
        /// 执行数据提取
        /// </summary>
        private void ExtractData()
        {
            /*
             * APP数据提取流程：
             * 1.根据设备类型和提取方式获取该APP的所有插件
             * 2.通过数据泵服务获取SourceFileItems
             * 3.通过特征匹配和版本匹配确定要执行的插件版本
             * 4.执行插件
             * 5.获取解析结果
             * 
             * */

            var items = _pluginAdapter.MatchPluginByPump(Pump, _extractItems);

            ExtractData(items);
        }

        private void ExtractData(Dictionary<ExtractItem, List<DataParsePluginInfo>> Items)
        {
            foreach (var item in Items)
            {
                Interlocked.Increment(ref _concurrentCount);
                Reporter?.Start(item.Key.GUID);
                //如果收到取消请求，则执行统一的扫尾工作，以此来标识一个任务的结束。
                if (_cancelToken.IsCancellationRequested)
                {
                    RoundOffwork(true, item.Key.GUID);
                }
                else
                {
                    try
                    {
                        DoDataPump(item.Key, item.Value);
                        Task.Factory.StartNew(() => DoDataPlugin(item.Key, item.Value), _cancelToken,
                            TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning,
                            TaskScheduler.Default).ContinueWith((t) => RoundOffwork(t, item.Key.GUID));
                    }//如果在执行数据泵期间捕获到异常，则执行统一的扫尾工作，以此来标识一个任务的结束。
                    catch (Exception ex)
                    {
                        RoundOffwork(ex, item.Key.GUID);
                    }
                }
            }
        }

        private void DoExtract(KeyValuePair<ExtractItem, List<DataParsePluginInfo>> item)
        {
            //注意：必须报告一次进度才能使状态切换到Runing
            Reporter?.ChangeProgress(item.Key.GUID, 0);
            if (_cancelToken.IsCancellationRequested) return;
            //在此统一检测CancelToken
            DoDataPump(item.Key, item.Value);
            if (!_cancelToken.IsCancellationRequested)
            {
                DoDataPlugin(item.Key, item.Value);
            }
        }

        private void DoDataPump(ExtractItem extractItem, IEnumerable<DataParsePluginInfo> plugins)
        {
            //执行数据泵服务
            foreach (var plugin in plugins)
            {
                foreach (var s in plugin.SourcePath)
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Pump.Execute(s, null, extractItem);
                }
            }
            Reporter?.ChangeProgress(extractItem.GUID, 0.33);
        }

        private void DoDataPlugin(ExtractItem extractItem, IEnumerable<DataParsePluginInfo> plugs)
        {
            if (_cancelToken.IsCancellationRequested) return;
            //1.匹配插件
            var plug = _pluginAdapter.MatchPluginByApp(plugs, Pump, Pump.SourceStorePath, GetAppVersion(extractItem));
            //2.执行插件
            plug.SaveDbPath = Pump.DbFilePath;
            plug.Phone = Pump.Source as Device;

            _pluginAdapter.ExecutePlugin(plug, null, (ds) =>
            {
                FinishExtractItem(extractItem, ds);
            });
            if (_cancelToken.IsCancellationRequested) return;
            Reporter?.ChangeProgress(extractItem.GUID, 0.85);
        }

        private void FinishExtractItem(ExtractItem extractItem, IDataSource ds)
        {
            if (ds != null)
            {
                //1.处理IDataSource
                String fileName = Path.Combine(Pump.ResultPath, $"{extractItem.GUID}_{extractItem.Name}.ds");
                Serializer.SerializeToBinary(ds, fileName);
            }
        }

        /// <summary>
        /// 执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffwork(Task t, String taskId)
        {
            if (t.IsFaulted)
            {
                RoundOffwork(t.Exception, taskId);
            }
            else
            {
                RoundOffwork(t.IsCanceled, taskId);
            }
        }

        /// <summary>
        /// 在非异常结束的情况下执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffwork(Boolean isCancelled, String taskId)
        {
            Interlocked.Decrement(ref _concurrentCount);
            if (isCancelled)
            {
                Reporter?.Stop(taskId);
            }
            else
            {
                Reporter?.ChangeProgress(taskId, 1);
                Reporter?.Finish(taskId);
            }
        }

        /// <summary>
        /// 在异常结束的情况下执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffwork(Exception ex, String taskId)
        {
            Interlocked.Decrement(ref _concurrentCount);
            Reporter.Defeat(taskId, ex);
        }

        private Version GetAppVersion(ExtractItem extractItem)
        {
            //TODO:获取APP版本信息
            return null;
        }

        #endregion

        #endregion

    }

}
