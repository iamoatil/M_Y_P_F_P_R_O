using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：ISelector  
* Author     ：litao
* Create Date：2017/10/26 10:39:30
* ==============================================================================*/

namespace XLY.SF.Project.UserControls.PreviewFile.Selector
{
    /// <summary>
    /// ISelector
    /// </summary>
    public interface ISuffixSelector
    {
        bool IsMatch(string suffix);
    }
}
