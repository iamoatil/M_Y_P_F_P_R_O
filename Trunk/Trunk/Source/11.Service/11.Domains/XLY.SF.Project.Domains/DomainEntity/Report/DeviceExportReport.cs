using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.Domains
{
   public class DeviceExportReport : NotifyPropertyBase
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }

        public string Path { get; set; }
        public bool IsFirstStyle { get; set; }

        public DeviceExportReport Parent { get; set; }

        /// <summary>
        /// 子节点列表
        /// </summary>
        public ObservableCollection<DeviceExportReport> TreeNodes { get; set; }
    }
}
