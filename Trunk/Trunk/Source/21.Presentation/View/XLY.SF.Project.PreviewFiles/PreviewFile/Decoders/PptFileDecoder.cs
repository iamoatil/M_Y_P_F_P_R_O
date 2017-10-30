
using System;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.LargeFileTextBox;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    class PptFileDecoder : IFileDecoder
    {
        public FrameworkElement Element { get { return _textBox; } }
        private readonly TextBoxUserControl _textBox = new TextBoxUserControl();

        public void Decode(string path)
        {
            _textBox.OpenFile(path);
        }
    }
}
