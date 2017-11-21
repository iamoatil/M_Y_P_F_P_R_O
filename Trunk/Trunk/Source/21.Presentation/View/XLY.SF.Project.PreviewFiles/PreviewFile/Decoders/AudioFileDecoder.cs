using System.IO;
using System.Windows;
using XLY.SF.Project.PreviewFilesView.UI;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
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

        public void Decode(string path)
        {
            path = Path.GetFullPath(path);
            _audioUserControl.Open(path);
        }
    }
}
