using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.Main;
using System.IO;
using XLY.SF.Framework.Language;
using System.Windows.Input;
using XLY.SF.Project.ViewModels.Tools;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using XLY.SF.Project.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XLY.SF.Project.DataMirror;
using DllClient;


/* ==============================================================================
* Description：MirrorViewModel  
* Author     ：litao
* Create Date：2017/11/3 11:37:01
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.MirrorView, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MirrorViewModel : ViewModelBase
    {
        public MirrorViewModel()
        {
            StartCommand = new RelayCommand(new Action(() => { SourcePosition.Start(); }));
            StopCommand = new RelayCommand(new Action(() => { SourcePosition.Stop(); }));           
        }
       
        public void Initialize(SPFTask task,IAsyncProgress async)
        {
            Mirror mirror=GetNewMirror(task);
            SourcePosition.MirrorControlerBox mirrorControler = new SourcePosition.MirrorControlerBox(task,mirror,async, SourcePosition);
            SourcePosition.SetMirrorControler(mirrorControler);
        }

        SourcePosition _sourcePosition = new SourcePosition();
        TargetPosition _targetPosition = new TargetPosition();

        public SourcePosition SourcePosition { get { return _sourcePosition; } }
        public TargetPosition TargetPosition { get { return _targetPosition; } }

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

        public void Start()
        {
            if(_mirrorControlerBox != null)
            {
                _mirrorControlerBox.Start();
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
            IAsyncProgress _asyn;
            SourcePosition _sourcePosition;
            public MirrorControlerBox(SPFTask task, Mirror mirror, IAsyncProgress asyn, SourcePosition sourcePosition)
            {
                _task = task;
                _mirror = mirror;
                _asyn = asyn;
                _sourcePosition = sourcePosition;
                _mirrorControler = new MirrorControler();
            }

            public void Start()
            {
                //todo 此处因Mirror结构，不太好。
                _mirror.Block=_sourcePosition.CurrentSelectedDisk.CurrentSelectedItem;
                _mirrorControler.Execute(_task, _mirror, _asyn);
            }

            public void Stop()
            {
                _mirrorControler.Stop(_asyn);
            }

            public void Continue()
            {
                _mirrorControler.Continue(_asyn);
            }

            public void Pause()
            {
                _mirrorControler.Pause(_asyn);
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
                string filePath = folderBrowserDialog.SelectedPath;
                DirPath = filePath;
            }
        }
    }
}


