using System.IO;
using System.Windows;
using XLY.SF.Project.PreviewFilesView.UI;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
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

        readonly VideoUserControlVLC _videoUserControl = new VideoUserControlVLC();
        readonly MediaElement _mediaElement = new MediaElement();

        public void Decode(string path)
        {
            path = Path.GetFullPath(path);
            _mediaElement.Open(path);
            _videoUserControl.SetMediaElement(_mediaElement);
        }
    }
}
