using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl;
using MediaViewer = XLY.SF.Project.UserControls.PreviewFile.Decoders.FileViewer.MediaViewer;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    class VideoFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get
            {
                return _audioUserControl;
            }
        }

        readonly VideoUserControl _audioUserControl = new VideoUserControl();
        readonly MediaElement _mediaElement = new MediaElement();

        public void Decode(string path)
        {
            path = Path.GetFullPath(path);
            _mediaElement.Source = new Uri(path);
            _audioUserControl.SetMediaElement(_mediaElement);
        }
    }
    
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
        readonly MediaViewer.MediaElement _mediaElement = new MediaViewer.MediaElement();

        public void Decode(string path)
        {
            path = Path.GetFullPath(path);
            _mediaElement.Open(path);
            _videoUserControl.SetMediaElement(_mediaElement);
        }
    }
}
