using System.Windows;
using XLY.SF.Project.PreviewFilesView.UI;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
{
    class ExcelFileDecoder : IFileDecoder
    {
        public FrameworkElement Element { get { return _textBox; } }
        private readonly TextBoxUserControl _textBox = new TextBoxUserControl();

        public void Decode(string path)
        {
            _textBox.OpenFile(path);
        }
    }
}
