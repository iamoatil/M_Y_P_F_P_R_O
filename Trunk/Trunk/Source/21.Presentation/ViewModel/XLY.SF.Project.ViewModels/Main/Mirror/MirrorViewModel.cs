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
            StartCommand = new RelayCommand(new Action(() => { System.Windows.MessageBox.Show("Start"); }));
            StopCommand = new RelayCommand(new Action(() => { System.Windows.MessageBox.Show("Stop"); }));
           
        }
       
        SourcePosition _sourcePosition = new SourcePosition();
        TargetPosition _targetPosition = new TargetPosition();

        public SourcePosition SourcePosition { get { return _sourcePosition; } }
        public TargetPosition TargetPosition { get { return _targetPosition; } }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }
    }

    public class SourcePosition
    {
        public SourcePosition()
        {
            PhoneDisks = new DiskPatitions("手机");
            SimDisk = new DiskPatitions("Sim");
            SDDisk = new DiskPatitions("Sd");
            
        }

        private Domains.Device _device;

        public DiskPatitions PhoneDisks { get; private set; }
        public DiskPatitions SimDisk { get; private set; }
        public DiskPatitions SDDisk { get; private set; }
        
        /// <summary>
        /// 初始化设备
        /// </summary>
        public void InitializeDevice(IDevice device)
        {
            _device = (Domains.Device)device;
            if (_device == null)
            {
                return;
            }
            List<Partition> partitions = _device.GetPartitons();
            if(_device.DeviceType == EnumDeviceType.Phone)
            {
                PhoneDisks.Items = partitions;                
            }
            else if (_device.DeviceType == EnumDeviceType.SIM)
            {
                SimDisk.Items = partitions;
            }
            else if(_device.DeviceType == EnumDeviceType.SDCard)
            {
                SDDisk.Items = partitions;
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
                _items = value;
                OnPropertyChanged();
            }
        }

        public void Load()
        {

        }
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


