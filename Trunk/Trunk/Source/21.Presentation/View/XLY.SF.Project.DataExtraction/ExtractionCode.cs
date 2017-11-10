using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// 数据提取操作码。
    /// </summary>
    public enum ExtractionCode
    {
        /// <summary>
        /// 开始。
        /// </summary>
        Start,
        /// <summary>
        /// 停止。
        /// </summary>
        Stop,
        /// <summary>
        /// 错误。
        /// </summary>
        Error = 2,
        /// <summary>
        /// 进度改变。
        /// </summary>
        ProgressChanged,
    }
}
