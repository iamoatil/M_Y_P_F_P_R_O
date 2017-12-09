using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Extension.CommandEx
{
    /// <summary>
    /// 日志模型【Command】
    /// </summary>
    public class CmdLogModel
    {
        /// <summary>
        /// 操作模块
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string OperationResult { get; set; }
    }
}
