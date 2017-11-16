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
    public struct DataExtractionParams
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 DataExtractionParams。
        /// </summary>
        /// <param name="pump">数据泵信息。</param>
        /// <param name="items">提取项列表。</param>
        public DataExtractionParams(Pump pump,params ExtractItem[] items)
        {
            Pump = pump;
            Items = items;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 提取对象。
        /// </summary>
        public Pump Pump { get; private set; }

        /// <summary>
        /// 提取项。
        /// </summary>
        public ExtractItem[] Items { get; private set; }

        #endregion
    }
}
