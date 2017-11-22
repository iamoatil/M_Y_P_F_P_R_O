/* ==============================================================================
* Description：IEnable  
* Author     ：litao
* Create Date：2017/11/22 10:56:47
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// IEnable接口与界面上的Checkbox对应
    /// </summary>
    interface IEnable
    {
        /// <summary>
        /// 相当于CheckBox的IsEnable
        /// </summary>
        bool IsEnable { get; set; }

        /// <summary>
        /// 相当于CheckBox的Name
        /// </summary>
        string Name { get; set; }
        
    }
}
