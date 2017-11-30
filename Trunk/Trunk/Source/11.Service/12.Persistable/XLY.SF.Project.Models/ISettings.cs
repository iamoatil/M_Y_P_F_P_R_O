using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models
{
    /// <summary>
    /// 提供配置信息存取功能。
    /// </summary>
    public interface ISettings : IRecordContext<Basic>, IRecordContext<CaseType>, IRecordContext<WorkUnit>, IRecordContext<Inspection>
    {
        /// <summary>
        /// 获取 Basic 配置中的指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>值。</returns>
        String GetValue(String key);

        /// <summary>
        /// 设置 Basic 配置中的指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void SetValue(String key, String value);
    }
}
