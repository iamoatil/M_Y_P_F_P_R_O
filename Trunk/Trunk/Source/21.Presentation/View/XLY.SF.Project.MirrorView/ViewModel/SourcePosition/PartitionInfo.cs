using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：Partition  
* Author     ：litao
* Create Date：2017/11/20 16:27:32
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{

    internal class MirrorBlockInfo
    {
        public MirrorBlockInfo(string targetDir, string blockPath)
        {
            _targetDir = targetDir;
            SourceBlockPath = blockPath;
        }

        private string _targetDir;

        public string TargetMirrorFile
        {
            get
            {
                return _targetDir + SourceBlockPath.TrimStart('/').Replace("/", "_") + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".bin";
            }
        }

        public string SourceBlockPath { get; private set; }
    }

    /// <summary>
    /// 磁盘分区
    /// </summary>
    public class CmdDiskPartitions : NotifyPropertyBase
    {
        public CmdDiskPartitions(string name)
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
