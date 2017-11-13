using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl;
using MediaViewer = XLY.SF.Project.UserControls.PreviewFile.Decoders.FileViewer.MediaViewer;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    class AudioVLCFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get
            {
                return _audioUserControl;
            }
        }

        readonly AudioUserControlVLC _audioUserControl = new AudioUserControlVLC();
        readonly MediaViewer.MediaElement _mediaElement = new MediaViewer.MediaElement();

        public void Decode(string path)
        {
            path = Path.GetFullPath(path);
            _mediaElement.Open(path);
            _audioUserControl.SetMediaElement(_mediaElement);
        }
    }
}
