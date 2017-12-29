using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.EarlyWarningView
{
    [Export(ExportKeys.AutoWarningProgressViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class EarlyWarningProgressViewModel : ViewModelBase
    {
        [Import(typeof(IRecordContext<Inspection>))]
        private IRecordContext<Inspection> InspectionService { get; set; }

        EarlyWarningPluginAdapter _adapter = new EarlyWarningPluginAdapter();

        private string _id;
        private IDevice _device;
        internal String Path { get; set; }
        private bool _isStarted;

        /// <summary>
        /// 当前状态
        /// </summary>
        ProgressState _curState=ProgressState.Unknow;

        /// <summary>
        /// 使用结果界面替换内容的事件
        /// </summary>
        public event Action<bool> ReplaceViewContentAction;

        protected override void InitLoad(object parameters)
        {
            Tuple<String, string, IDevice> args = (Tuple<String, string, IDevice>)parameters;
            Path = args.Item1;
            _id = args.Item2;
            _device = args.Item3;
            _adapter.Initialize(InspectionService);
            _adapter.TwoProgressReporter.ProgresssChanged -= Progress_ProgresssChanged;
            _adapter.TwoProgressReporter.ProgresssChanged += Progress_ProgresssChanged;
            _adapter.SetDevice(_device);
        }

        public override void ReceiveParameters(object parameters)
        {
            if (parameters is Boolean b)
            {
                if (b)
                {
                    _curState = ProgressState.Unknow;
                    ProgressValue = 0;
                    MaxProgressValue = 100;
                    OnReplaceViewContent(true);
                    _adapter.Detect(Path);
                    if (!_isStarted)
                    {
                        SendDeviceStateMsg(true);
                    }
                }
                else
                {
                    if (_curState != ProgressState.IsProgressing)
                    {
                        OnReplaceViewContent(false);
                    }
                    else
                    {
                        OnReplaceViewContent(true);
                    }
                }
            }
        }
       
        private void Progress_ProgresssChanged(object sender, IProgressEventArg e)
        {
            ProgressStater stater = e.Parameter as ProgressStater;
            
            if (stater.State == ProgressState.IsProgressing)
            {
                _curState = ProgressState.IsProgressing;

                SystemContext.Instance.AsyncOperation.SynchronizationContext.Post(state =>
                {
                    ProgressValue += (e.ProgressValue * MaxProgressValue);
                }, null);
            }
            else if (stater.State == ProgressState.IsFinished)
            {
                OnReplaceViewContent(false);
                _curState = ProgressState.IsFinished;
                SystemContext.Instance.AsyncOperation.Post(state =>
                {
                    ProgressValue = MaxProgressValue;
                }, null);
                if (_isStarted)
                {
                    SendDeviceStateMsg(false);
                }
            }
        }

        /// <summary>
        /// 使用结果视图替换现有进度视图
        /// </summary>
        private void OnReplaceViewContent(bool isOriginal)
        {
            if(ReplaceViewContentAction != null)
            {
                ReplaceViewContentAction(isOriginal);
            }
        }

        /// <summary>
        /// 当前进度值
        /// </summary>
        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }
        private double _progressValue;

        /// <summary>
        /// 最大的进度值
        /// </summary>
        public double MaxProgressValue
        {
            get { return _maxProgressValue; }
            set
            {
                _maxProgressValue = value;
                OnPropertyChanged();
            }
        }
        private double _maxProgressValue =100;

        /// <summary>
        /// 停止智能预警命令
        /// </summary>
        public ICommand StopEarlyWarningCommand
        {
            get
            {
                if (_stopEarlyWarningCommand == null)
                {
                    _stopEarlyWarningCommand = new RelayCommand(StopEarlyWarning);
                }
                return _stopEarlyWarningCommand;
            }
        }
        
        private ICommand _stopEarlyWarningCommand;

        /// <summary>
        /// 停止智能预警
        /// </summary>
        private void StopEarlyWarning()
        {
            _adapter.StopDetect();
            _curState = ProgressState.IsFinished;
            ReceiveParameters(null);
        }

        /// <summary>
        /// 发送设备状态消息
        /// </summary>
        /// <param name="isStarted"></param>
        private void SendDeviceStateMsg(bool isStarted)
        {
            var msg = new GeneralArgs<KeyValuePair<string,bool>>(GeneralKeys.ExtractDeviceStateMsg);
            msg.Parameters = new KeyValuePair<string, bool>(_id, !isStarted);
            MessageAggregation.SendGeneralMsg(msg);
            _isStarted = isStarted;
        }

        protected override void Closed()
        {
            StopEarlyWarning();
        }
    }
}
