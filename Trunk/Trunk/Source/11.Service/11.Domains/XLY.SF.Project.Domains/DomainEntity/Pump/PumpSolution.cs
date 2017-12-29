using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提取方案。
    /// </summary>
    [Flags]
    public enum PumpSolution
    {
        None = 0x0000,
        /// <summary>
        /// 临时ROOT。
        /// </summary>
        TempRoot = 0x0101,
        /// <summary>
        /// APP植入。
        /// </summary>
        AppInjection= 0x0102,
        /// <summary>
        /// 备份提取。
        /// </summary>
        BackExtraction = 0x0104,
        /// <summary>
        /// 降级提取。使用此标志将忽略其它标志。
        /// </summary>
        Downgrading = 0x0108,
    }
}
