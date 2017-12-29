using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步通知状态
    /// </summary>
    public enum TaskState : UInt16
    {
        /// <summary>
        /// 空闲，未开始
        /// </summary>
        Idle = 0x0000,
        /// <summary>
        /// 等待中
        /// </summary>
        Waiting = 0x0001,
        /// <summary>
        /// 正在准备开始中
        /// </summary>
        Starting = 0x0101,
        /// <summary>
        /// 正在运行
        /// </summary>
        Running = 0x0102,
        /// <summary>
        /// 正在停止
        /// </summary>
        IsCancellationRequest = 0x0104,
        /// <summary>
        /// 执行完成
        /// </summary>
        Completed = 0x0202,
        /// <summary>
        /// 已取消。
        /// </summary>
        Cancelled = 0x0204,
        /// <summary>
        /// 失败。
        /// </summary>
        Failed = 0x0205,
    }
}
