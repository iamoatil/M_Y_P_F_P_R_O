using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Persistable
{
    /// <summary>
    /// 提供配置信息存取功能。
    /// </summary>
    public interface ISettings : IRecordContext<Basic>, IRecordContext<CaseType>, IRecordContext<WorkUnit>, IRecordContext<Inspection>
    {
    }
}
