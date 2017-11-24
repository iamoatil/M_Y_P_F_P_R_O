/* ==============================================================================
* Description：ICategory  
* Author     ：litao
* Create Date：2017/11/24 15:38:57
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    interface IName
    {
        /// <summary>
        /// 类型的名字
        /// </summary>
        string Name { get; set; }
    }
    
    interface ICategory: IName
    {
        /// <summary>
        ///  此类型孩子的个数
        /// </summary>
        int ChildrenCount { get;  }

        /// <summary>
        /// 此类型是否被选中
        /// </summary>
        bool IsChecked { get; set; }
    }
}
