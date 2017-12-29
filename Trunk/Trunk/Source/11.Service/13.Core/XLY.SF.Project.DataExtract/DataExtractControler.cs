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
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
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

        private CancellationTokenSource _cancelTokenSource;

        private CancellationToken _cancelToken;

        private PluginAdapter _pluginAdapter;


        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 DataExtractControler 实例。
        /// </summary>
        public DataExtractControler(TaskReporterAggregation reporter)
        {
            Reporter = reporter;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 多任务进度报告器。
        /// </summary>
        public TaskReporterAggregation Reporter { get; }

        /// <summary>
        /// 当前是否正在执行任务。
        /// </summary>
        public Boolean IsBusy => Reporter.State == TaskState.Running;

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
            if (InitState(pump, extractItems))
            {
                Task.Factory.StartNew(() =>
                {
                    DataPumpBase dp = InitDataPump(pump);
                    ExtractData(dp, extractItems);
                }, _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith(t => t.Exception.Handle(x => true), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        /// <summary>
        /// 停止数据提取
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (!IsBusy) return;
            _cancelTokenSource.Cancel();
        }

        #endregion

        #region Private

        #region Init

        private DataPumpBase InitDataPump(Pump pump)
        {
            if (_cancelToken.IsCancellationRequested) return null;
            using (CancellationTokenRegistration registration = _cancelToken.Register(StopAtDataPumpInitialization))
            {
                return pump.GetDataPump();
            }
        }


        private Boolean InitState(Pump pump, ExtractItem[] extractItems)
        {
            Reporter?.ResetAll();
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;

            if (extractItems == null || extractItems.Length == 0) return false;
            foreach (ExtractItem item in extractItems)
            {
                if (_cancelToken.IsCancellationRequested) return false;
                Reporter?.Wait(item.GUID);
            }
            _pluginAdapter = PluginAdapter.Instance;
            if (_pluginAdapter == null) return false;
            if (_cancelToken.IsCancellationRequested) return false;
            FileHelper.CreateExitsDirectory(pump.SavePath);
            FileHelper.CreateExitsDirectory(pump.SourceStorePath);
            return !_cancelToken.IsCancellationRequested;
        }

        private void StopAtDataPumpInitialization()
        {
            Reporter?.CancellingAll("在初始化数据泵时收到取消操作的请求");
        }

        #endregion

        #region Extract

        /// <summary>
        /// 执行数据提取
        /// </summary>
        private void ExtractData(DataPumpBase pump, ExtractItem[] items)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                if (Reporter == null) return;
                foreach (ITaskProgressReporter reporter in Reporter.Reporters)
                {
                    reporter.Finish(false, "在提取之前已取消该任务");
                }
                return;
            }

            pump.Phone = GetPhone(pump);

            var dic = _pluginAdapter.MatchPluginByPump(pump.PumpDescriptor, items);
            ExtractData(pump, dic);
        }

        //获取设备信息
        private Device GetPhone(DataPumpBase pump)
        {
            if (pump.PumpDescriptor.Type == EnumPump.Mirror)
            {
                var deviceFile = $"{pump.PumpDescriptor.Source.ToSafeString()}.device";
                if (FileHelper.IsValid(deviceFile))
                {
                    return Serializer.DeSerializeFromBinary<Device>(deviceFile);
                }
            }
            else
            {
                return pump.PumpDescriptor.Source as Device;
            }

            return null;
        }

        private void ExtractData(DataPumpBase pump, Dictionary<ExtractItem, List<AbstractDataParsePlugin>> Items)
        {
            foreach (var item in Items)
            {
                //如果收到取消请求，则执行统一的扫尾工作，以此来标识一个任务的结束。
                if (_cancelToken.IsCancellationRequested)
                {
                    RoundOffWork(false, item.Key.GUID, 0);
                }
                else
                {
                    try
                    {
                        DoDataPump(pump, item.Key, item.Value);
                        Reporter?.Start(item.Key.GUID);
                        if (_cancelToken.IsCancellationRequested)
                        {
                            RoundOffWork(false, item.Key.GUID, 0);
                            continue;
                        }
                        Reporter?.ChangeProgress(item.Key.GUID, 0.33);

                        Task<Int32> task = Task.Factory.StartNew(() => DoDataPlugin(pump, item.Key, item.Value), _cancelToken,
                            TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

                        //正常完成的任务
                        task.ContinueWith(t => RoundOffWork(t, item.Key.GUID, t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

                        //异常终止的任务
                        task.ContinueWith((t) => AbortTaskByException(t, item.Key.GUID), TaskContinuationOptions.OnlyOnFaulted);
                    }
                    catch (Exception ex)
                    {
                        RoundOffWork(ex, item.Key.GUID);
                    }
                }
            }
        }

        private void DoDataPump(DataPumpBase pump, ExtractItem extractionItem, IEnumerable<AbstractDataParsePlugin> plugins)
        {
            //执行数据泵服务
            DataPumpExecutionContext context = null;
            foreach (var plugin in plugins)
            {
                var et = _pluginAdapter.GetExtractItems(plugin.DataParsePluginInfo).ToArray();
                foreach (var s in (plugin.PluginInfo as DataParsePluginInfo).SourcePath)
                {
                    if (_cancelToken.IsCancellationRequested) return;
                    context = pump.CreateContext(s, extractionItem);
                    context.Reporter = Reporter[extractionItem.GUID];
                    context.CancellationToken = _cancelToken;    
                    pump.Execute(context);
                }
            }
        }

        private void AbortTaskByException(Task t, String taskId)
        {
            t.Exception.Handle(x =>
            {
                RoundOffWork(x, taskId);
                return true;
            });
        }

        private Int32 DoDataPlugin(DataPumpBase pump, ExtractItem extractItem, IEnumerable<AbstractDataParsePlugin> plugs)
        {
            if (_cancelToken.IsCancellationRequested) return 0;

            //1.匹配插件
            var plug = _pluginAdapter.MatchPluginByApp(plugs, pump.PumpDescriptor, pump.PumpDescriptor.SourceStorePath, GetAppVersion(extractItem));
            IDataSource ds = null;

            if (null != plug)
            {
                //2.执行插件
                plug.DataParsePluginInfo.SaveDbPath = pump.PumpDescriptor.DbFilePath;
                plug.DataParsePluginInfo.Phone = pump.Phone;

                Reporter?.ChangeProgress(extractItem.GUID, 0.45);
                using (CancellationTokenRegistration ctr = _cancelToken.Register(StopAtPluginExecution, extractItem.GUID))
                {
                    ds = _pluginAdapter.ExecutePlugin(plug, Reporter[extractItem.GUID]);
                }
                if (_cancelToken.IsCancellationRequested) return 0;
            }
            return FinishExtractItem(pump, extractItem, ds ?? new SimpleDataSource { PluginInfo = plug.DataParsePluginInfo });
        }

        private void StopAtPluginExecution(Object state)
        {
            String taskId = (String)state;
            Reporter?[taskId].Cancelling($"任务[{taskId}]在执行插件时收到取消操作的请求");
        }

        #endregion

        #region RoundOffWork

        private Int32 FinishExtractItem(DataPumpBase pump, ExtractItem extractItem, IDataSource ds)
        {
            if (ds != null)
            {
                //1.处理IDataSource
                String fileName = Path.Combine(pump.PumpDescriptor.ResultPath, $"{extractItem.GUID}_{extractItem.Name}.ds");
                Serializer.SerializeToBinary(ds, fileName);
                return ds.Total;
            }
            return 0;
        }

        /// <summary>
        /// 执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffWork(Task t, String taskId, Int32 count)
        {
            if (t.IsFaulted)
            {
                RoundOffWork(t.Exception, taskId);
            }
            else
            {
                RoundOffWork(!t.IsCanceled, taskId, count);
            }
        }

        /// <summary>
        /// 在非异常结束的情况下执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffWork(Boolean isCompleted, String taskId, Int32 count)
        {
            if (isCompleted)
            {
                Reporter?.Finish(taskId, true, count.ToString());
            }
            else
            {
                Reporter?.Finish(taskId, false);
            }
        }

        /// <summary>
        /// 在异常结束的情况下执行统一的扫尾工作。向前端汇报完成。
        /// </summary>
        /// <param name="t">任务。</param>
        /// <param name="taskId">任务Id。</param>
        private void RoundOffWork(Exception ex, String taskId)
        {
            Reporter.Defeat(taskId, ex);
        }

        #endregion

        private Version GetAppVersion(ExtractItem extractItem)
        {
            //TODO:获取APP版本信息
            return null;
        }

        #endregion

        #endregion

    }

}
