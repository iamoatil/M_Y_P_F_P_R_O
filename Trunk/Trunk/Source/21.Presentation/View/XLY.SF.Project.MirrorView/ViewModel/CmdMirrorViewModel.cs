using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.DataMirror;
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
            _msgBox = new MessageBoxX();
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            PauseCommand = new RelayCommand(Pause);
            ContinueCommand = new RelayCommand(Continue);
            SelectPartitionCommand = new RelayCommand<PartitionElement>(SelectPartition);
            ParseMirrorCommand = new RelayCommand(ParseMirror);
        }

        private Device MirrorDevice { get; set; }

        protected override void InitLoad(object parameters)
        {
            base.InitLoad(parameters);

            MirrorDevice = parameters as Device;
            if (MirrorDevice != null)
            {
                switch (MirrorDevice.OSType)
                {
                    case EnumOSType.Android:
                        SourcePosition.RefreshPartitions(MirrorDevice);
                        Initialize(MirrorDevice.ID);
                        break;
                    case EnumOSType.IOS:
                        SourcePosition.RefreshPartitions(MirrorDevice);
                        break;
                }
            }
        }

        public void Initialize(string deviceID)
        {
            StateReporter stateReporter = new StateReporter();

            int isHtc = 0;
            MirrorControlerBox mirrorControler = new MirrorControlerBox(deviceID, isHtc, stateReporter, _pauseInfo);

            SourcePosition.SetMirrorControler(mirrorControler);

            //进度事件           
            stateReporter.Reported += StateChanged;
        }

        private void SelectPartition(PartitionElement par)
        {
            foreach (var pa in SourcePosition.CurrentSelectedDisk.Items)
            {
                pa.IsChecked = par == pa;
            }
        }

        /// <summary>
        /// 进度变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressChanged(long progress)
        {
            ProgressPosition.FinishedSize += ProgressPosition.GetIntervalToLastTime(progress);
        }

        /// <summary>
        /// 状态变化事件
        /// 安卓镜像
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
                //生成Device文件
                MirrorDevice.InstalledApps = MirrorDevice.FindInstalledApp();
                var deviceFile = TargetPosition.TargetMirrorFile + ".device";
                Serializer.SerializeToBinary(MirrorDevice, deviceFile);

                _msgBox.ShowDialogSuccessMsg("镜像完成");
                SourcePosition.IsMirroring = false;
                ProgressPosition.FinishedSize = ProgressPosition.TotalSize;
                ProgressPosition.Stop();
            }
            else if (state.IsType(CmdStrings.Exception))
            {
                _msgBox.ShowDialogErrorMsg("镜像失败" + state.GetChildCmd());
                SourcePosition.IsMirroring = false;
                ProgressPosition.Stop();

                DeleteMirrorFile();
            }
            else if (state.Match(CmdStrings.StopMirror))
            {
                _msgBox.ShowDialogWarningMsg("镜像停止");
                SourcePosition.IsMirroring = false;
                ProgressPosition.Stop();

                DeleteMirrorFile();
            }
            else if (state.Match(CmdStrings.NoSelectedPartition))
            {
                _msgBox.ShowDialogWarningMsg("请选择至少一个分区");
                SourcePosition.IsMirroring = false;
            }
            else if (state.Match(CmdStrings.PauseMirror))
            {
                _msgBox.ShowDialogSuccessMsg("当前镜像已暂停");
            }
            //else if (state.Match(CmdStrings.ContinueMirror))
            //{

            //}
            _mirrorFile.SetState(state) ;
        }

        //镜像失败或者停止后需要删除镜像文件
        private void DeleteMirrorFile()
        {
            FileHelper.DeleteFileSafe(TargetPosition.TargetMirrorFile);
        }

        MessageBoxX _msgBox;
        MyDriverInfo _driverInfo = new MyDriverInfo();
        PauseInfo _pauseInfo = new PauseInfo();
        

        readonly CmdSourcePosition _sourcePosition = new CmdSourcePosition();
        readonly CmdTargetPosition _targetPosition = new CmdTargetPosition();
        readonly CmdProgressPosition _progressPosition = new CmdProgressPosition();
        // 镜像文件
        readonly MirrorFile _mirrorFile = new MirrorFile();

        public CmdSourcePosition SourcePosition { get { return _sourcePosition; } }
        public CmdTargetPosition TargetPosition { get { return _targetPosition; } }
        public CmdProgressPosition ProgressPosition { get { return _progressPosition; } }
        public MirrorFile MirrorFile { get { return _mirrorFile; } }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand PauseCommand { get; private set; }

        public ICommand ContinueCommand { get; private set; }

        public ICommand SelectPartitionCommand { get; private set; }

        /// <summary>
        /// 解析镜像命令
        /// </summary>
        public ICommand ParseMirrorCommand { get; private set; }

        /// <summary>
        /// 解析镜像是否有效
        /// </summary>
        public bool IsParseMirrorValidate { get { return !(Application.Current is App); } }

        private void Start()
        {
            if (null != MirrorDevice)
            {
                switch (MirrorDevice.OSType)
                {
                    case EnumOSType.Android:
                        AndroidDeviceMirror();
                        break;
                    case EnumOSType.IOS:
                        IosDevciceMirror();
                        break;
                }
            }
        }

        /// <summary>
        /// 安卓手机镜像
        /// </summary>
        private void AndroidDeviceMirror()
        {
            if (!Directory.Exists(TargetPosition.DirPath))
            {
                Directory.CreateDirectory(TargetPosition.DirPath);
            }

            //开始镜像前，检测剩余空间
            long freeValue = _driverInfo.GetTargetDriverFreeSpace(TargetPosition.DirPath);
            if (freeValue <= SourcePosition.CurrentSelectedDisk.SelectedSize * 1.1)
            {
                _msgBox.ShowDialogErrorMsg(string.Format("目录{0}所在的磁盘空间不足", TargetPosition.DirPath));
                return;
            }
            //开始镜像
            if (SourcePosition.CurrentSelectedDisk.Items != null)
            {
                ProgressPosition.TotalSize = SourcePosition.CurrentSelectedDisk.SelectedSize;
                ProgressPosition.Start();
            }
            
            SourcePosition.Start(TargetPosition.DirPath);
            SourcePosition.IsMirroring = true;

            TargetPosition.TargetMirrorFile = SourcePosition.TargetMirrorFile;

            _mirrorFile.Intialize(SourcePosition.TargetMirrorFile);
        }

        private IOSDeviceMirrorService IosMirrorService { get; set; }

        /// <summary>
        /// 苹果手机镜像
        /// </summary>
        private void IosDevciceMirror()
        {
            IosMirrorService = new IOSDeviceMirrorService();
            BUserStopIOSMirror = false;

            var mirror = new Mirror() { Source = MirrorDevice, Target = TargetPosition.DirPath, TargetFile = $"{MirrorDevice.Name}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.bin" };
            TargetPosition.TargetMirrorFile = mirror.Local;

            ProgressPosition.TotalSize = 0;
            ProgressPosition.Start();
            SourcePosition.IsMirroring = true;

            _mirrorFile.Intialize(mirror.Local);
            Task.Run(() =>
                {
                    DefaultAsyncTaskProgress dtp = new DefaultAsyncTaskProgress();

                    try
                    {
                        dtp.ProgressChanged += Dtp_ProgressChanged;

                        IosMirrorService.Execute(mirror, dtp);

                        if (FileHelper.IsValid(mirror.Local))
                        {
                            //生成MD5
                            string md5String = FileHelper.MD5FromFileUpper(mirror.Local);
                            var md5File = mirror.Local + ".md5";
                            File.WriteAllText(md5File, md5String, Encoding.UTF8);

                            //生成Device文件
                            MirrorDevice.InstalledApps = MirrorDevice.FindInstalledApp();
                            var deviceFile = mirror.Local + ".device";
                            Serializer.SerializeToBinary(MirrorDevice, deviceFile);

                            _msgBox.ShowDialogSuccessMsg("镜像完成");
                            SourcePosition.IsMirroring = false;
                            ProgressPosition.Stop();
                            _mirrorFile.SetState(CmdStrings.AllFinishState);
                        }
                        else
                        {
                            if (!BUserStopIOSMirror)
                            {//镜像失败
                                _msgBox.ShowDialogErrorMsg("镜像失败");
                                SourcePosition.IsMirroring = false;
                                ProgressPosition.Stop();
                                _mirrorFile.SetState(CmdStrings.StopMirror);
                            }
                        }
                    }
                    finally
                    {
                        dtp.ProgressChanged -= Dtp_ProgressChanged;
                    }
                });
        }

        private void Dtp_ProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            var ss = e.TaskId.ToSafeInt64();
            if (ss > 0)
            {
                ProgressPosition.FinishedSize = ss;
            }
            ProgressPosition.Msg = e.Message;
            ProgressPosition.Progress = e.Progress;
        }

        /// <summary>
        /// 用户停止IOS镜像
        /// </summary>
        private bool BUserStopIOSMirror = false;

        private void Stop()
        {
            if (_msgBox.ShowQuestionMsg("是否停止镜像？"))
            {
                switch (MirrorDevice.OSType)
                {
                    case EnumOSType.Android:
                        ProgressPosition?.Stop();
                        SourcePosition?.Stop();
                        break;
                    case EnumOSType.IOS:
                        BUserStopIOSMirror = true;
                        ProgressPosition?.Stop();
                        IosMirrorService?.Stop();
                        IosMirrorService = null;
                        SourcePosition.IsMirroring = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 暂停的位置
        /// </summary>
        private void Pause()
        {
            if (MirrorDevice.OSType == EnumOSType.Android)
            {
                SourcePosition.Pause();
                ProgressPosition.Pause();
            }
        }

        /// <summary>
        /// 继续的位置
        /// </summary>
        private void Continue()
        {
            if (MirrorDevice.OSType == EnumOSType.Android)
            {
                SourcePosition.Continue();
                ProgressPosition.Continue();
            }
        }

        /// <summary>
        /// 解析镜像
        /// </summary>
        private void ParseMirror()
        {
            MirrorFileParser mirrorFileParser = new MirrorFileParser(MessageAggregation);
            mirrorFileParser.ParseMirror(MirrorFile);
        }        
    }
}


