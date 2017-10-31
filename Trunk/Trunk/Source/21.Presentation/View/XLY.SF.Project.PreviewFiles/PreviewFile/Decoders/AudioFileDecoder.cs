using System.Windows;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl;

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

        public void Decode(string path)
        {
            _audioUserControl.LoadFile(path);
        }
    }
}
