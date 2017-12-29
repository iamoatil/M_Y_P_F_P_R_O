using System;
using System.Collections.Generic;
using System.Threading;
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
            PhoneDisks = new CmdDiskPartitions("手机");
            SimDisk = new CmdDiskPartitions("Sim");
            SDDisk = new CmdDiskPartitions("Sd");
            CurrentSelectedDisk = PhoneDisks;
        }

        MirrorControlerBox _mirrorControlerBox;

        public CmdDiskPartitions PhoneDisks { get; private set; }
        public CmdDiskPartitions SimDisk { get; private set; }
        public CmdDiskPartitions SDDisk { get; private set; }

        public CmdDiskPartitions CurrentSelectedDisk { get; set; }

        /// <summary>
        /// 是否正在镜像
        /// </summary>
        public bool IsMirroring
        {
            get { return _isMirroring; }
            set
            {
                if(_isMirroring == value)
                {
                    return;
                }
                _isMirroring = value;
                if (value == true)
                {
                    CanPause = true;
                    CanContinue = false;
                }
                else
                {
                    CanPause = false;
                    CanContinue = false;
                }

                OnPropertyChanged();
            }
        }

        private bool _isMirroring = false;

        /// <summary>
        /// 正处于暂停状态
        /// </summary>
        public bool IsPausing
        {
            get
            {
                return _isPausing;
            }
            private set
            {
                if(_isPausing == value)
                {
                    return;
                }
                if (!IsMirroring)
                {
                    CanPause = false;
                    CanContinue = false;
                    return;
                }
                _isPausing = value;
                if (_isPausing)
                {
                    CanContinue = true;
                    CanPause = false;
                }
                else
                {
                    CanContinue = false;
                    CanPause = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _isPausing;

        /// <summary>
        /// 镜像设备是否支持暂停
        /// </summary>
        private bool DeviceCanPause { get; set; }

        public bool CanPause
        {
            get { return _canPause; }
            private set
            {
                if (value && DeviceCanPause)
                {//只有设备支持暂停镜像才设置为可暂停
                    _canPause = value;
                }
                else
                {
                    _canPause = false;
                }
                OnPropertyChanged();
            }
        }
        private bool _canPause;

        public bool CanContinue
        {
            get { return _canContinue; }
            private set
            {
                _canContinue = value;
                OnPropertyChanged();
            }
        }
        private bool _canContinue;


        internal void SetMirrorControler(MirrorControlerBox mirrorControler)
        {
            _mirrorControlerBox = mirrorControler;
        }

        /// <summary>
        /// 刷新分区信息
        /// </summary>
        public void RefreshPartitions(Device device)
        {
            if (device == null)
            {
                return;
            }

            List<Partition> partitions = device.GetPartitons();
            List<PartitionElement> partitionList = new List<PartitionElement>();
            foreach (var item in partitions)
            {
                partitionList.Add(new PartitionElement(item));
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

            DeviceCanPause = device.OSType == EnumOSType.Android;
        }

        public string TargetMirrorFile { get; set; }

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
                            list.Add(new MirrorBlockInfo(targetDir, item.Path));

                            TargetMirrorFile = list[0].TargetMirrorFile;
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

            IsPausing = false;
            IsMirroring = false;
            CanPause = false;
            CanContinue = false;
        }

        internal void Pause()
        {
            if (_mirrorControlerBox != null)
            {
                IsPausing = true;
                _mirrorControlerBox.Pause();
            }
        }

        internal void Continue()
        {
            if (_mirrorControlerBox != null)
            {
                IsPausing = false;
                ThreadPool.QueueUserWorkItem((o) => { _mirrorControlerBox.Continue(); });
            }
        }
    }
}
