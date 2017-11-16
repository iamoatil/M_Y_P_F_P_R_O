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
        Start = 0,
        /// <summary>
        /// 停止。
        /// </summary>
        Stop = 1,
        /// <summary>
        /// 进度改变。
        /// </summary>
        ProgressChanged = 2,
        /// <summary>
        /// 任务结束。
        /// </summary>
        Terminate = 3,
    }
}
