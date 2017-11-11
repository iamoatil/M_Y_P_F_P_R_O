using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using System.Windows.Input;
using XLY.SF.Project.Domains;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;


/* ==============================================================================
* Description：MirrorViewModel  
* Author     ：litao
* Create Date：2017/11/3 11:37:01
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.MirrorView, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CmdMirrorViewModel : ViewModelBase
    {
        public CmdMirrorViewModel()
        {
            StartCommand = new RelayCommand(new Action(() =>
            {
                if (SourcePosition != null
                || SourcePosition.CurrentSelectedDisk != null)
                {
                    long totalSize = 0;
                    foreach (var item in SourcePosition.CurrentSelectedDisk.Items)
                    {
                        if(item.IsChecked)
                        {
                            totalSize += item.Size;
                        }                        
                    }
                    ProgressPosition.TotalSize = totalSize;
                    CaculateTime(0);
                }
                SourcePosition.Start(TargetPosition.DirPath);
            }));
            StopCommand = new RelayCommand(new Action(() => { SourcePosition.Stop(); }));
            SelectAllCommand = new RelayCommand<bool>(new Action<bool>(parmeter =>
            {
                foreach (var item in SourcePosition.CurrentSelectedDisk.Items)
                {
                    item.IsChecked = parmeter;
                }
            }));
        }

        
        public void Initialize(SPFTask task)
        {
            IAsyncTaskProgress asyncProgress = new MyDefaultSingleTaskReporter();
            int isHtc = 0;
            CmdSourcePosition.MirrorControlerBox mirrorControler = new CmdSourcePosition.MirrorControlerBox(task.Device.ID, isHtc, asyncProgress);           

            SourcePosition.SetMirrorControler(mirrorControler);

            //设置滚动条进度
            asyncProgress.ProgressChanged += (o, e) =>
            {
                ProgressPosition.FinishedSize = (int)e.Progress;
                CaculateTime(ProgressPosition.TotalSize-ProgressPosition.FinishedSize);
                SourcePosition.IsMirroring = true;
            };
            asyncProgress.Ternimated += (o, e) =>
            {
                if(e.IsCompleted)
                {
                    ProgressPosition.FinishedSize = ProgressPosition.TotalSize;
                    CaculateTime(0);                    
                }
                SourcePosition.IsMirroring = false;
                RemainTime = "";
            };
        }

        readonly CmdSourcePosition _sourcePosition = new CmdSourcePosition();
        readonly CmdTargetPosition _targetPosition = new CmdTargetPosition();
        readonly CmdProgressPosition _progressPosition = new CmdProgressPosition();

        public CmdSourcePosition SourcePosition { get { return _sourcePosition; } }
        public CmdTargetPosition TargetPosition { get { return _targetPosition; } }
        public CmdProgressPosition ProgressPosition { get { return _progressPosition; } }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        public string RemainTime
        {
            get
            {
                return _remainTime;
            }
            set
            {
                _remainTime = value;
                OnPropertyChanged();
            }
        }
        private string _remainTime;      

        /// <summary>
        /// 通过数据量的大小，得出镜像他们所需的时间
        /// </summary>
        /// <param name="size"></param>
        private void CaculateTime(long size)
        {
            long estimatedTime = size / (1024 * 1024 * 5);
            RemainTime = TimeSpan.FromSeconds(estimatedTime).ToString();
        }        

        public class MyDefaultSingleTaskReporter : DefaultSingleTaskReporter
        {
            public MyDefaultSingleTaskReporter()
            {
                State = TaskState.Running;
            }
        }
    }

    public class CmdSourcePosition: NotifyPropertyBase
    {
        public CmdSourcePosition()
        {
            PhoneDisks = new CmdDiskPatitions("手机");
            SimDisk = new CmdDiskPatitions("Sim");
            SDDisk = new CmdDiskPatitions("Sd");
            CurrentSelectedDisk = PhoneDisks;
        }

        MirrorControlerBox _mirrorControlerBox;

        public CmdDiskPatitions PhoneDisks { get; private set; }
        public CmdDiskPatitions SimDisk { get; private set; }
        public CmdDiskPatitions SDDisk { get; private set; }
        
        public CmdDiskPatitions CurrentSelectedDisk { get; set; }

        /// <summary>
        /// 是否正在镜像
        /// </summary>
        public bool IsMirroring
        {
            get { return _isMirroring; }
            set
            {
                _isMirroring = value;
                OnPropertyChanged();
            }
        }

        private bool _isMirroring = false;

        internal void SetMirrorControler(MirrorControlerBox mirrorControler)
        {
            _mirrorControlerBox = mirrorControler;         
        }

        /// <summary>
        /// 刷新分区信息
        /// </summary>
        public void RefreshPartitions(IDevice dev)
        {
            Domains.Device device = (Domains.Device)dev;
            if (device == null)
            {
                return;
            }

            List<Partition> partitions = device.GetPartitons();
            List<CmdDiskPatitions.PartitionElement> partitionList = new List<CmdDiskPatitions.PartitionElement>();
            foreach (var item in partitions)
            {
                partitionList.Add(new CmdDiskPatitions.PartitionElement(item));
            }

            if(device.DeviceType == EnumDeviceType.Phone)
            {
                PhoneDisks.Items = partitionList;                
            }
            else if (device.DeviceType == EnumDeviceType.SIM)
            {
                SimDisk.Items = partitionList;
            }
            else if(device.DeviceType == EnumDeviceType.SDCard)
            {
                SDDisk.Items = partitionList;
            }            
        }

        /// <summary>
        /// 开始执行镜像
        /// 注意：因为执行镜像是一个耗时的过程，可能2,3个小时，所以不能使用主线程调用。所以此处用线程池中的一个线程来执行
        /// </summary>
        public void Start(string targetDir)
        {
            if(_mirrorControlerBox != null)
            {
                List<MirrorBlockInfo> list = new List<MirrorBlockInfo>();
                if (CurrentSelectedDisk != null)
                {
                    foreach (var item in CurrentSelectedDisk.Items)
                    {
                        if(item.IsChecked)
                        {
                            list.Add(new MirrorBlockInfo(targetDir+DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")+".bin",item.Path));
                        }                        
                    }                    
                }
                ThreadPool.QueueUserWorkItem((o)=> { _mirrorControlerBox.Start(list); });     
            }
        }

        public void Stop()
        {
            if (_mirrorControlerBox != null)
            {
                _mirrorControlerBox.Stop();
            }
        }

        /// <summary>
        /// 无它，MirrorControler
        /// 因为：个人觉得这种封装更符合一般认识
        /// </summary>
        internal class MirrorControlerBox
        {
            MirrorBackgroundProcess _mirrorBackgroundProcess;
            string _deviceID;
            int _isHtc;          
            IAsyncTaskProgress _asyn;
            public MirrorControlerBox(string deviceID, int isHtc, IAsyncTaskProgress asyn)
            {
                _deviceID = deviceID;
                _isHtc = isHtc;                
                _asyn = asyn;
            }

            //此处封装了关键的方法：Start，Stop，Continue，Pause
            internal void Start(List<MirrorBlockInfo> mirrorBlockInfos)
            {
                foreach (var item in mirrorBlockInfos)
                {
                    //以下是构建参数
                    string arg = string.Format(@"StartMirror|{0}|{1}|{2}|{3}", _deviceID, _isHtc, item.TargetMirrorFile, item.SourceBlockPath);

                    //设置反馈，以及执行命令
                    if (_mirrorBackgroundProcess != null)
                    {
                        _mirrorBackgroundProcess.CallBack -= OnCallBack;
                        _mirrorBackgroundProcess.Close();
                    }
                    _mirrorBackgroundProcess = new MirrorBackgroundProcess();
                    _mirrorBackgroundProcess.CallBack += OnCallBack;
                    _mirrorBackgroundProcess.ExcuteCmd(arg);
                }                
            }

            private void OnCallBack(string info)
            {
                DefaultSingleTaskReporter defalutAsyn = (DefaultSingleTaskReporter)_asyn;
                if (info.StartsWith("Operate"))
                {
                    string operateStr = info.Substring("Operate|".Length);
                    if (operateStr.StartsWith("Stop"))
                    {
                        string argStr = operateStr.Substring("Stop|".Length);                        
                        if (argStr == "Success")
                        {
                            defalutAsyn.Finish();
                        }
                        else if(argStr == "UserStoped")
                        {
                            defalutAsyn.Stop();                           
                        }

                        _mirrorBackgroundProcess.CallBack -= OnCallBack;
                        _mirrorBackgroundProcess.Close();
                    }
                }
                else if (info.StartsWith("Progress"))
                {
                    string finisedSizeStr = info.Substring("Progress|".Length);
                    int finisedSize = 0;
                    if (int.TryParse(finisedSizeStr, out finisedSize))
                    {
                        defalutAsyn.ChangeProgress(finisedSize);
                    }
                }
                else if(info.StartsWith("Exception|"))
                {
                    defalutAsyn.Defeat("Exception");
                    _mirrorBackgroundProcess.CallBack -= OnCallBack;
                    _mirrorBackgroundProcess.Close();
                    //todo 此处应该有提示
                }
            }
            internal void Stop()
            {
                if(_mirrorBackgroundProcess != null)
                {
                    _mirrorBackgroundProcess.ExcuteCmd("StopMirror");
                    _mirrorBackgroundProcess.Close();
                }
            }

            internal void Continue()
            {
                if (_mirrorBackgroundProcess != null)
                {
                    _mirrorBackgroundProcess.ExcuteCmd("ContinueMirror");
                }
            }

            internal void Pause()
            {
                if (_mirrorBackgroundProcess != null)
                {
                    _mirrorBackgroundProcess.ExcuteCmd("PauseMirror");
                }
            }            
        }

        public class MirrorBlockInfo
        {
            public MirrorBlockInfo(string mirrorFile,string blockPath)
            {
                TargetMirrorFile = mirrorFile;
                SourceBlockPath = blockPath;
            }
            public string TargetMirrorFile { get; private set; }
            public string SourceBlockPath { get; private set; }
        }
    }

    /// <summary>
    /// 磁盘分区
    /// </summary>
    public class CmdDiskPatitions : NotifyPropertyBase
    {
        public CmdDiskPatitions(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 磁盘分区的名字
        /// </summary>
        private string _name;

        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 磁盘分区
        /// </summary>
        private List<PartitionElement> _items;

        public List<PartitionElement> Items
        {
            get { return _items; }
            set
            {
                if (_items == null
                    && value != null
                    && value.Count > 0)
                {
                    value[0].IsChecked = true;
                }
                _items = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 分区界面元素
        /// </summary>
        public class PartitionElement : NotifyPropertyBase
        {
            public PartitionElement(Partition partition)
            {
                Path = partition.Block.ToString().Replace("\\", @"/");//此处把windows的反斜杠替换成linux的斜杠，否则，镜像时出现size全为0的回调数据
                Size = partition.Size;
                ClickCommand = new RelayCommand(new Action(() => { IsChecked = !IsChecked; }));
            }

            /// <summary>
            /// 分区的路径
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// 是否选中
            /// </summary>
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }

            private bool _isChecked = false;

            /// <summary>
            /// 分区的大小
            /// </summary>
            public long Size
            {
                get { return _size; }
                set
                {
                    _size = value;
                    //把size转换成G或M单位
                    const int G1 = 1024 * 1024 * 1024;
                    const int M1 = 1024 * 1024 ;
                    if (_size > G1)
                    {
                        SizeInfo = Math.Round((double)_size / G1,2).ToString()+"G" ;
                    }
                    else
                    {
                        SizeInfo = Math.Round((double)_size / M1, 2).ToString()+"M";
                    }
                    OnPropertyChanged();
                }
            }
            private long _size;

            /// <summary>
            /// 分区
            /// </summary>
            public string SizeInfo
            {
                get { return _sizeInfo; }
                set
                {
                    _sizeInfo = value;
                    OnPropertyChanged();
                }
            }
            private string _sizeInfo;

            /// <summary>
            /// 点击命令
            /// </summary>
            public ICommand ClickCommand { get; private set; }
        }
    }

    /// <summary>
    /// 目标位置
    /// </summary>
    public class CmdTargetPosition : NotifyPropertyBase
    {
        public CmdTargetPosition()
        {
            SetTargetPathCommand = new RelayCommand(new Action(() => Set()));
        }

        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        private string _dirPath= @"C:\XLYSFTasks\";

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetTargetPathCommand { get; private set; }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void Set()
        {
            FolderBrowserDialog folderBrowserDialog  = new FolderBrowserDialog();
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string filePath = folderBrowserDialog.SelectedPath;
                DirPath = filePath;
            }
        }
    }

    /// <summary>
    /// 进度条位置
    /// </summary>
    public class CmdProgressPosition : NotifyPropertyBase
    {
        /// <summary>
        /// 已经完成的大小
        /// </summary>
        public long FinishedSize
        {
            get { return _finishedSize; }
            set
            {
                _finishedSize = value;
                //只有在进度变化1000分之一时才通知
                if (Math.Abs(_lastChangedValue - _finishedSize) > TotalSize / 1000)
                {
                    OnPropertyChanged();
                    _lastChangedValue = _finishedSize;
                }
            }
        }
        private long _finishedSize =0;

        private long _lastChangedValue = 0;

        /// <summary>
        /// 总共大小
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
            set
            {
                _totalSize = value;
                OnPropertyChanged();
            }
        }
        private long _totalSize =100;
    }
}


