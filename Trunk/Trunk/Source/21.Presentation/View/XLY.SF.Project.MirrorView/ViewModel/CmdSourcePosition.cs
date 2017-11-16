using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：CmdSourcePosition  
* Author     ：litao
* Create Date：2017/11/16 9:49:53
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    public class CmdSourcePosition : NotifyPropertyBase
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

            if (device.DeviceType == EnumDeviceType.Phone)
            {
                PhoneDisks.Items = partitionList;
            }
            else if (device.DeviceType == EnumDeviceType.SIM)
            {
                SimDisk.Items = partitionList;
            }
            else if (device.DeviceType == EnumDeviceType.SDCard)
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
            if (_mirrorControlerBox != null)
            {
                List<MirrorBlockInfo> list = new List<MirrorBlockInfo>();
                if (CurrentSelectedDisk != null)
                {
                    foreach (var item in CurrentSelectedDisk.Items)
                    {
                        if (item.IsChecked)
                        {
                            list.Add(new MirrorBlockInfo(targetDir + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".bin", item.Path));
                        }
                    }
                }
                ThreadPool.QueueUserWorkItem((o) => { _mirrorControlerBox.Start(list); });
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
                //设置反馈，以及执行命令
                if (_mirrorBackgroundProcess != null)
                {
                    _mirrorBackgroundProcess.CallBack -= OnCallBack;
                    _mirrorBackgroundProcess.Close();
                }
                _mirrorBackgroundProcess = new MirrorBackgroundProcess();
                _mirrorBackgroundProcess.CallBack += OnCallBack;
                //todo 此处要
                ((CmdMirrorViewModel.MyDefaultSingleTaskReporter)_asyn).PrepareStart();
                ((CmdMirrorViewModel.MyDefaultSingleTaskReporter)_asyn).ChangeProgress(0);

                foreach (var item in mirrorBlockInfos)
                {
                    //以下是构建参数
                    string arg = string.Format(@"StartMirror|{0}|{1}|{2}|{3}", _deviceID, _isHtc, item.TargetMirrorFile, item.SourceBlockPath);
                    _mirrorBackgroundProcess.ExcuteCmd(arg);
                    //todo 此处有问题 等待任务完成
                    TaskState state = ((CmdMirrorViewModel.MyDefaultSingleTaskReporter)_asyn).State;
                    while (state != TaskState.Completed
                        && state != TaskState.Failed
                        && state != TaskState.Stopped)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            private void OnCallBack(string info)
            {
                DefaultSingleTaskReporter defalutAsyn = (DefaultSingleTaskReporter)_asyn;
                if (info == null)
                {
                    info = "Exception|MirrorBackgroundProcessError";
                }
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
                        else if (argStr == "UserStoped")
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
                else if (info.StartsWith("Exception|"))
                {
                    defalutAsyn.Defeat(info);
                    _mirrorBackgroundProcess.CallBack -= OnCallBack;
                    _mirrorBackgroundProcess.Close();
                    //todo 此处应该有提示
                }
            }
            internal void Stop()
            {
                if (_mirrorBackgroundProcess != null)
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
            public MirrorBlockInfo(string mirrorFile, string blockPath)
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
        /// 选择的分区的总共大小
        /// </summary>
        public long SelectedTotalSize
        {
            get
            {
                long totalSize = 0;
                foreach (var item in Items)
                {
                    if (item.IsChecked)
                    {
                        totalSize += item.Size;
                    }
                }
                return totalSize;
            }
        }

        /// <summary>
        /// 设置所有选择的分区的Check状态
        /// </summary>
        public void SetAllCheckState(bool isCheck)
        {
            if (Items == null)
            {
                return;
            }
            foreach (var item in Items)
            {
                item.IsChecked = isCheck;
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
                                                                      //此处不能把Path转换成全部大写，否则出现镜像时返回的字节数组长度一直为0情况。
                PathWithUpperStyle = Path.ToUpper();
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
                    const int M1 = 1024 * 1024;
                    if (_size > G1)
                    {
                        SizeInfo = Math.Round((double)_size / G1, 2).ToString() + "G";
                    }
                    else
                    {
                        SizeInfo = Math.Round((double)_size / M1, 2).ToString() + "M";
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
            /// 大写的路径信息
            /// </summary>
            public string PathWithUpperStyle { get; private set; }

            /// <summary>
            /// 点击命令
            /// </summary>
            public ICommand ClickCommand { get; private set; }
        }
    }
}
