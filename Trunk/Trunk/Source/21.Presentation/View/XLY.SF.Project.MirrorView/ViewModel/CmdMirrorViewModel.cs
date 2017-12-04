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
            PauseCommand = new RelayCommand(new Action(Pause));
            ContinueCommand = new RelayCommand(new Action(Continue));
            SelectAllCommand = new RelayCommand<bool>(new Action<bool>(SetAllCheckState));
        }

        protected override void InitLoad(object parameters)
        {
            base.InitLoad(parameters);
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
            MirrorControlerBox mirrorControler = new MirrorControlerBox(deviceID, isHtc, stateReporter,_pauseInfo);

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
            ProgressPosition.FinishedSize += ProgressPosition.GetIntervalToLastTime(progress) ;
            ProgressPosition.OnProgress(ProgressPosition.TotalSize - ProgressPosition.FinishedSize);            
        }

        /// <summary>
        /// 状态变化事件
        /// </summary>
        private void StateChanged(CmdString state)
        {
            if (state.IsType(CmdStrings.Progress))
            {
                CmdString progress = state.GetChildCmd();
                long finisedSize = 0;
                if (long.TryParse(progress.ToString(), out finisedSize))
                {                   
                    ProgressChanged(finisedSize);
                    _pauseInfo.SetPausePos(finisedSize);
                }                
            }
            else if (state.Match(CmdStrings.StartMirror))
            {
                SourcePosition.IsMirroring = true;
            }
            else if (state.Match(CmdStrings.AllFinishState))
            {
                _msgBox.ShowDialogSuccessMsg("镜像完成");
                SourcePosition.IsMirroring = false;
                ProgressPosition.FinishedSize = ProgressPosition.TotalSize;
                ProgressPosition.Stop();
            }
            else if (state.IsType(CmdStrings.Exception))
            {
                _msgBox.ShowErrorMsg("镜像失败" + state.GetChildCmd());
                SourcePosition.IsMirroring = false;
            }
            else if (state.Match(CmdStrings.StopMirror))
            {
                _msgBox.ShowDialogSuccessMsg("镜像停止");
                SourcePosition.IsMirroring = false;
            }
            else if(state.Match(CmdStrings.NoSelectedPartition))
            {
                _msgBox.ShowDialogSuccessMsg("请选择至少一个分区");
                SourcePosition.IsMirroring = false;
            }
            else if(state.Match(CmdStrings.PauseMirror)
                || state.Match(CmdStrings.ContinueMirror))
            {
                _msgBox.ShowDialogSuccessMsg(state.ToString());
            }
        }

        IMessageBox _msgBox;
        MyDriverInfo _driverInfo = new MyDriverInfo();
        PauseInfo _pauseInfo = new PauseInfo();

        readonly CmdSourcePosition _sourcePosition = new CmdSourcePosition();
        readonly CmdTargetPosition _targetPosition = new CmdTargetPosition();
        readonly CmdProgressPosition _progressPosition = new CmdProgressPosition();

        public CmdSourcePosition SourcePosition { get { return _sourcePosition; } }
        public CmdTargetPosition TargetPosition { get { return _targetPosition; } }
        public CmdProgressPosition ProgressPosition { get { return _progressPosition; } }      

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand PauseCommand { get; private set; }

        public ICommand ContinueCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }

       

        private void Start()
        {
            //开始镜像前，检测剩余空间
            long freeValue=_driverInfo.GetTargetDriverFreeSpace(TargetPosition.DirPath);
            if (freeValue <= SourcePosition.CurrentSelectedDisk.SelectedTotalSize * 1.1)
            {
                _msgBox.ShowErrorMsg(string.Format("目录{0}所在的磁盘空间不足以存放镜像后的数据", TargetPosition.DirPath));
                return;
            }
            //开始镜像
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

        /// <summary>
        /// 暂停的位置
        /// </summary>
        private void Pause()
        {
            SourcePosition.Pause();
        }

        /// <summary>
        /// 继续的位置
        /// </summary>
        private void Continue()
        {          
            SourcePosition.Continue();
        }

        private void SetAllCheckState(bool isChecked)
        {
            SourcePosition.CurrentSelectedDisk.SetAllCheckState(isChecked);
        }  
    }
}


