using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataExtract
{
    /// <summary>
    /// 数据提取参数。
    /// </summary>
    public class DataExtractionParams
    {
        /// <summary>
        /// 提取对象。
        /// </summary>
        public Pump Pump { get; set; }

        /// <summary>
        /// 提取项。
        /// </summary>
        public List<ExtractItem> Items { get; set; }
    }
}
