using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：MirrorViewModel  
* Author     ：litao
* Create Date：2017/11/3 11:37:01
* ==============================================================================*/

namespace XLY.SF.Project.MirrorTools
{
    //[Export(ExportKeys.MirrorView, typeof(ViewModelBase))]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class WcfMirrorViewModel : ViewModelBase
    {
        public WcfMirrorViewModel()
        {
            StartCommand = new RelayCommand(new Action(() => {
                if (SourcePosition != null
                || SourcePosition.CurrentSelectedDisk != null
                || SourcePosition.CurrentSelectedDisk.CurrentSelectedItem != null)
                {
                    ProgressPosition.TotalSize=(int)SourcePosition.CurrentSelectedDisk.CurrentSelectedItem.Size;
                }
                SourcePosition.Start();
            }));
            StopCommand = new RelayCommand(new Action(() => { SourcePosition.Stop(); }));           
        }

        
        public void Initialize(SPFTask task)
        {
            IAsyncTaskProgress asyncProgress = new MyDefaultSingleTaskReporter();

            Mirror mirror = GetNewMirror(task);
            SourcePosition.MirrorControlerBox mirrorControler = new SourcePosition.MirrorControlerBox(task, mirror, asyncProgress);           

            SourcePosition.SetMirrorControler(mirrorControler);

            //设置滚动条进度
            asyncProgress.ProgressChanged += (o, e) =>
            {
                ProgressPosition.FinishedSize = (int)e.Progress;
            };
            asyncProgress.Terminated += (o, e) =>
            {
                if (e.IsCompleted)
                {
                    ProgressPosition.FinishedSize = ProgressPosition.TotalSize;
                }
            };
        }

        readonly SourcePosition _sourcePosition = new SourcePosition();
        readonly TargetPosition _targetPosition = new TargetPosition();
        readonly ProgressPosition _progressPosition = new ProgressPosition();

        public SourcePosition SourcePosition { get { return _sourcePosition; } }
        public TargetPosition TargetPosition { get { return _targetPosition; } }
        public ProgressPosition ProgressPosition { get { return _progressPosition; } }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        /// <summary>
        /// 根据Task生成一个Mirror
        /// </summary>
        private Mirror GetNewMirror(SPFTask task)
        {
            Mirror mirror = new Mirror();
            mirror.Block = SourcePosition.CurrentSelectedDisk.CurrentSelectedItem;
            mirror.Source = task.Device;
            mirror.Target = TargetPosition.DirPath;
            mirror.TargetFile = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")+".bin";
            EnumDeviceType deviceType = task.Device.DeviceType; 
            if(deviceType == EnumDeviceType.Chip)
            {
                mirror.Type = EnumMirror.Chip;
            }
            else if (deviceType == EnumDeviceType.SIM)
            {
                mirror.Type = EnumMirror.SIM;
            }
            else if (deviceType == EnumDeviceType.SDCard)
            {
                mirror.Type = EnumMirror.SDCard;
            }
            else 
            {
                mirror.Type = EnumMirror.Device;
            }
            mirror.MirrorFlag = MirrorFlag.NewMirror;
            return mirror;
        }

        public class MyDefaultSingleTaskReporter : DefaultSingleTaskReporter
        {
            public MyDefaultSingleTaskReporter()
            {
                State = TaskState.Running;
            }
        }
    }    

    public class SourcePosition
    {
        public SourcePosition()
        {
            PhoneDisks = new DiskPatitions("手机");
            SimDisk = new DiskPatitions("Sim");
            SDDisk = new DiskPatitions("Sd");
            CurrentSelectedDisk = PhoneDisks;
        }

        MirrorControlerBox _mirrorControlerBox;

        public DiskPatitions PhoneDisks { get; private set; }
        public DiskPatitions SimDisk { get; private set; }
        public DiskPatitions SDDisk { get; private set; }
        
        public DiskPatitions CurrentSelectedDisk { get; set; }

        internal void SetMirrorControler(MirrorControlerBox mirrorControler)
        {
            _mirrorControlerBox = mirrorControler;
            //要先登录，否则回调为空
            X86DLLClientSingle.Instance.CoreChannel.Login();
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
            if(device.DeviceType == EnumDeviceType.Phone)
            {
                PhoneDisks.Items = partitions;                
            }
            else if (device.DeviceType == EnumDeviceType.SIM)
            {
                SimDisk.Items = partitions;
            }
            else if(device.DeviceType == EnumDeviceType.SDCard)
            {
                SDDisk.Items = partitions;
            }            
        }

        /// <summary>
        /// 开始执行镜像
        /// 注意：因为执行镜像是一个耗时的过程，可能2,3个小时，所以不能使用主线程调用。所以此处用线程池中的一个线程来执行
        /// </summary>
        public void Start()
        {
            if(_mirrorControlerBox != null)
            {
                if (CurrentSelectedDisk != null
                    && CurrentSelectedDisk.CurrentSelectedItem != null)
                {
                    _mirrorControlerBox.UpdateData(CurrentSelectedDisk.CurrentSelectedItem);
                }
                ThreadPool.QueueUserWorkItem((o)=> { _mirrorControlerBox.Start(); });     
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
            readonly MirrorControler _mirrorControler;
            SPFTask _task;
            Mirror _mirror;
            IAsyncTaskProgress _asyn;
            public MirrorControlerBox(SPFTask task, Mirror mirror, IAsyncTaskProgress asyn)
            {
                _task = task;
                _mirror = mirror;
                _asyn = asyn;
                _mirrorControler = new MirrorControler();
            }

            //此处封装了关键的方法：Start，Stop，Continue，Pause
            internal void Start()
            {
                _mirrorControler.Execute(_task, _mirror, _asyn);
            }

            internal void Stop()
            {
                _mirrorControler.Stop(_asyn);
            }

            internal void Continue()
            {
                _mirrorControler.Continue(_asyn);
            }

            internal void Pause()
            {
                _mirrorControler.Pause(_asyn);
            }

            //每次开始镜像的之前都需要更新数据
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="currentSelectedItem"></param>
            internal void UpdateData(Partition currentSelectedItem)
            {
                _mirror.Block= currentSelectedItem;
            }
        }
    }

    /// <summary>
    /// 磁盘分区
    /// </summary>
    public class DiskPatitions : NotifyPropertyBase
    {
        public DiskPatitions(string name)
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
        private List<Partition> _items;

        public List<Partition> Items
        {
            get { return _items; }
            set
            {
                if(_items == null
                    && value != null
                    && CurrentSelectedItem == null
                    && value.Count > 0)
                {
                    CurrentSelectedItem = value[0];
                }
                _items = value;        
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前选择的项目
        /// </summary>
        public Partition CurrentSelectedItem { get; set; }
    }

    /// <summary>
    /// 目标位置
    /// </summary>
    public class TargetPosition : NotifyPropertyBase
    {
        public TargetPosition()
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
                string filePath = folderBrowserDialog.SelectedPath+"\\";                
                DirPath = filePath;
            }
        }
    }

    /// <summary>
    /// 进度条位置
    /// </summary>
    public class ProgressPosition : NotifyPropertyBase
    {
        /// <summary>
        /// 已经完成的大小
        /// </summary>
        public int FinishedSize
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
        private int _finishedSize=0;

        private int _lastChangedValue = 0;

        /// <summary>
        /// 总共大小
        /// </summary>
        public int TotalSize
        {
            get { return _totalSize; }
            set
            {
                _totalSize = value;
                OnPropertyChanged();
            }
        }
        private int _totalSize=100;
    }
}


