using System.Windows;

/* ==============================================================================
* Description：AudioUserControl  
* Author     ：litao
* Create Date：2017/10/23 15:05:34
* ==============================================================================*/

namespace XLY.SF.Project.PreviewFilesView.UI
{
    class AudioUserControlVLC : PlayerUserControlVLC
    {
        public AudioUserControlVLC() : base()
        {
            this.VerticalAlignment = VerticalAlignment.Center;
            MediaElementContainer.Height = 0;
        }
    }
}
