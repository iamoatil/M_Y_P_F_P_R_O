using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.Models
{
    /// <summary>
    /// 提供业务数据存取功能。
    /// </summary>
    public interface ILogicalDataContext : IRecordContextExtension, IRecordContext<OperationLog>, IRecordContext<UserInfo>, IRecordContext<RecentCase>
    {
    }
}
