using System.Windows;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    class VideoVLCFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get
            {
                return _videoUserControl;
            }
        }

        VideoUserControlVLC _videoUserControl = new VideoUserControlVLC();

        public void Decode(string path)
        {
            _videoUserControl.LoadFile(path);
        }
    }
}
