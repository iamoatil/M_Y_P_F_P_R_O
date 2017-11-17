using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;

/* ==============================================================================
* Description：MirrorViewModel  
* Author     ：litao
* Create Date：2017/11/3 11:37:01
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    [Export(ExportKeys.MirrorView, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CmdMirrorViewModel : ViewModelBase
    {
        public CmdMirrorViewModel()
        {
            _msgBox = new MessageBoxEx();
            StartCommand = new RelayCommand(new Action(Start));
            StopCommand = new RelayCommand(new Action(Stop));
            SelectAllCommand = new RelayCommand<bool>(new Action<bool>(SetAllCheckState));
        }

        protected override void LoadCore(object parameters)
        {
            base.LoadCore(parameters);
            IDevice device = parameters as IDevice;
            if (device != null)
            {
                SourcePosition.RefreshPartitions(device);
                Initialize(device.ID);
            }
        }

        public void Initialize(string deviceID)
        {
            StateReporter stateReporter = new StateReporter();

            int isHtc = 0;
            CmdSourcePosition.MirrorControlerBox mirrorControler = new CmdSourcePosition.MirrorControlerBox(deviceID, isHtc, stateReporter);

            SourcePosition.SetMirrorControler(mirrorControler);

            //进度事件           
            stateReporter.Reported += StateChanged;
        }

        /// <summary>
        /// 进度变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressChanged(long progress)
        {
            ProgressPosition.FinishedSize = progress;
            ProgressPosition.OnProgress(ProgressPosition.TotalSize - ProgressPosition.FinishedSize);
            SourcePosition.IsMirroring = true;
        }

        /// <summary>
        /// 状态变化事件
        /// </summary>
        private void StateChanged(CmdString state)
        {
            if (state.Match(CmdStrings.AllFinishState))
            {
                ProgressPosition.FinishedSize = ProgressPosition.TotalSize;
                ProgressPosition.Stop();
            }
            SourcePosition.IsMirroring = false;
            ProgressPosition.RemainTime = "";
            ProgressPosition.UsedTime = "";

            if (state.IsType(CmdStrings.Progress))
            {
                CmdString progress = state.GetChildCmd();
                int finisedSize = 0;
                if (int.TryParse(progress.ToString(), out finisedSize))
                {
                    ProgressChanged(finisedSize);
                }
            }
            else if (state.Match(CmdStrings.AllFinishState))
            {
                _msgBox.ShowNoticeMsg("镜像完成");
            }
            else if (state.IsType(CmdStrings.Exception))
            {
                _msgBox.ShowNoticeMsg("镜像失败" + state.GetChildCmd());
            }
            else if (state.Match(CmdStrings.StopMirror))
            {
                _msgBox.ShowNoticeMsg("镜像停止");
            }
        }

        IMessageBox _msgBox;

        readonly CmdSourcePosition _sourcePosition = new CmdSourcePosition();
        readonly CmdTargetPosition _targetPosition = new CmdTargetPosition();
        readonly CmdProgressPosition _progressPosition = new CmdProgressPosition();

        public CmdSourcePosition SourcePosition { get { return _sourcePosition; } }
        public CmdTargetPosition TargetPosition { get { return _targetPosition; } }
        public CmdProgressPosition ProgressPosition { get { return _progressPosition; } }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }

        private void Start()
        {
            if (SourcePosition.CurrentSelectedDisk.Items != null)
            {
                ProgressPosition.TotalSize = SourcePosition.CurrentSelectedDisk.SelectedTotalSize;
                ProgressPosition.Start();
            }
            SourcePosition.Start(TargetPosition.DirPath);
            SourcePosition.IsMirroring = true;
        }

        private void Stop()
        {
            SourcePosition.Stop();
        }

        private void SetAllCheckState(bool isChecked)
        {
            SourcePosition.CurrentSelectedDisk.SetAllCheckState(isChecked);
        }  
    }
}


