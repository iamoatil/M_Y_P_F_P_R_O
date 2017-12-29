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
            _targetMirrorFile = System.IO.Path.Combine(_targetDir, SourceBlockPath.TrimStart('/').Replace("/", "_") + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".bin");
        }

        private string _targetDir;

        public string TargetMirrorFile
        {
            get
            {
                return _targetMirrorFile;
            }
        }
        private string _targetMirrorFile;

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
        /// 选择的分区大小
        /// </summary>
        public long SelectedSize
        {
            get
            {
                long totalSize = 0;
                foreach (var item in Items)
                {
                    if (item.IsChecked)
                    {
                        totalSize += item.Size;
                        break;
                    }
                }
                return totalSize;
            }
        }
    }

    /// <summary>
    /// 分区界面元素
    /// </summary>
    public class PartitionElement : NotifyPropertyBase
    {
        public PartitionElement(Partition partition)
        {
            if (null != partition.Block)
            {
                Path = partition.Block.ToString().Replace("\\", @"/");//此处把windows的反斜杠替换成linux的斜杠，否则，镜像时出现size全为0的回调数据
                                                                      //此处不能把Path转换成全部大写，否则出现镜像时返回的字节数组长度一直为0情况。
            }

            Name = partition.Text;
            Size = partition.Size;
        }

        private string _Name;

        /// <summary>
        /// 分区名字
        /// </summary>
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 分区的路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public new bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isChecked = false;

        const int G1 = 1024 * 1024 * 1024;
        const int M1 = 1024 * 1024;

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
                if (_size != 0)
                {
                    if (_size > G1)
                    {
                        SizeInfo = Math.Round((double)_size / G1, 2).ToString() + "G";
                    }
                    else
                    {
                        SizeInfo = Math.Round((double)_size / M1, 2).ToString() + "M";
                    }
                }
                else
                {
                    SizeInfo = string.Empty;
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
    }

}
