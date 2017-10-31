using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.UserControls.PreviewFile.Decoders;
using XLY.SF.Project.UserControls.PreviewFile.FileDecode;

namespace XLY.SF.Project.PreviewFiles.UI
{
    /// <summary>
    /// PreViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreViewControl : UserControl
    {
        public PreViewControl()
        {
            InitializeComponent();
            _decoderCollection = new FileDecoderCollection();
            _binaryDecoder = _decoderCollection.GetFileDecoder(FileDecoderTypes.Bin);
        }

        private FileDecoderCollection _decoderCollection;      

        IFileDecoder _binaryDecoder;

        public void ReplaceContent(string filePath)
        {
            try
            {
                FrameworkElement element = _decoderCollection.Decode(filePath);
                Preview.Content = element;
                _binaryDecoder.Decode(filePath);
                element = _binaryDecoder.Element;
                HexPreview.Content = element;
            }
            catch (System.Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex.Message+"/n"+ex.StackTrace);
            }            
        }
    }    
}
