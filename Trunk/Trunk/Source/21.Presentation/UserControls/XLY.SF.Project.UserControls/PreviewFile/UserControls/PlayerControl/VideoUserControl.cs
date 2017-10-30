using System.Windows;

/* ==============================================================================
* Description：VideoUserControl  
* Author     ：litao
* Create Date：2017/10/23 13:52:50
* ==============================================================================*/

namespace XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl
{
    class VideoUserControl : PlayerUserControl
    {
        public VideoUserControl()
        {
            this.VerticalAlignment = VerticalAlignment.Stretch;            
        }
    }

    class VideoUserControlVLC : PlayerUserControlVLC
    {
        public VideoUserControlVLC() 
        {
        }
    }
}
