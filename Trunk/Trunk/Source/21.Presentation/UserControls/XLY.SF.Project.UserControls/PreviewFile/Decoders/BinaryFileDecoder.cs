using System.Windows;
using System.Windows.Media;
using XLY.SF.Project.UserControls.PreviewFile.UserControls.LargeFileTextBox;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    class BinaryFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get { return _textBox; }
        }

        private readonly TextBoxUserControl _textBox = new TextBoxUserControl()
        {
            FontFamily = new FontFamily("Courier New"),
            EncodingName = "Hex"
        };

        /// <summary>
        /// 解码内容到属性Element上。
        /// </summary>
        public void Decode(string path)
        {
            _textBox.OpenFile(path);
        }
    }
}
