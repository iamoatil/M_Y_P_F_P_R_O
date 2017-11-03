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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;


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
            SetTargetPathCommand = new RelayCommand(new Action(() => _targetPosition.Set()));
            StartCommand = new RelayCommand(new Action(() => { MessageBox.Show("Start"); }));
            StopCommand = new RelayCommand(new Action(() => { MessageBox.Show("Stop"); }));
        }

        SourcePosition _sourcePosition = new SourcePosition();
        TargetPosition _targetPosition = new TargetPosition();

        public SourcePosition SourcePosition { get { return _sourcePosition; } }
        public TargetPosition TargetPosition { get { return _targetPosition; } }

        public ICommand SetTargetPathCommand { get; private set; }

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
        public DiskPatitions PhoneDisks { get; private set; }
        public DiskPatitions SimDisk { get; private set; }
        public DiskPatitions SDDisk { get; private set; }
    }

    /// <summary>
    /// 磁盘分区
    /// </summary>
    public class DiskPatitions : NotifyPropertyBase
    {
        public DiskPatitions(string name)
        {
            Name = name;
            Items = new ObservableCollection<string>() {"1","2" };
        }

        /// <summary>
        /// 磁盘分区的名字
        /// </summary>
        private string _name;

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        /// <summary>
        /// 磁盘分区
        /// </summary>
        private ObservableCollection<string> _items;

        public ObservableCollection<string> Items
        {
            get { return _items; }
            private set { _items = value; }
        }

        public void Load()
        {

        }
    }

    public class TargetPosition : NotifyPropertyBase
    {
        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void Set()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            bool? isOk = saveFileDialog.ShowDialog();
            if (isOk == true)
            {
                string filePath = saveFileDialog.FileName;
                FilePath = filePath;
            }
        }
    }
}


