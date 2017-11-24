/* ==============================================================================
* Description：ExtactionItem  
* Author     ：litao
* Create Date：2017/11/24 15:36:27
* ==============================================================================*/

using System;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class ExtactionItem : IName
    {
        public string Name { get; set; }

        public AbstractDataItem DataItem { get; private set; }

        internal void SetActualData(AbstractDataItem dataItem)
        {
            DataItem = dataItem;
        }
    }
}
