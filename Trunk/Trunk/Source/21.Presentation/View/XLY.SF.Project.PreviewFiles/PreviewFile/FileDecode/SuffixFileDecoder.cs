/* ==============================================================================
* Description：SuffixFileDecoder  
* Author     ：litao
* Create Date：2017/10/26 11:34:03
* ==============================================================================*/
using System.ComponentModel.Composition;
using XLY.SF.Project.PreviewFilesView.PreviewFile.MatchDecoder;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile.FileDecode
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.AudioVLC)]
    class AudioVLCSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly AudioVLCFileDecoder _fileDecoder = new AudioVLCFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".mp3|.wma|.ape|.flac|.aac|.ac3|.mmf|.amr|.m4a|.m4r|.ogg|.wav|.mp2");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Bin)]
    class BinarySuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly BinaryFileDecoder _fileDecoder = new BinaryFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".bin");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Html)]
    class HtmlSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly HtmlFileDecoder _fileDecoder = new HtmlFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".html|.Xml");
    }
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Picture)]
    class PictureSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly PictureFileDecoder _fileDecoder = new PictureFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".jpg|.png|.ico|.bmp|.tif|.tga|.gif");
    }
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Text)]
    class TextSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly TextFileDecoder _fileDecoder = new TextFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".txt|.ini");
    }    

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.VideoVLC)]
    class VideoVLCSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly VideoVLCFileDecoder _fileDecoder = new VideoVLCFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".avi|.rmvb|.rm|.mp4|.mkv|.webM|.3gp|.WMV|.MPG|.vob|.mov|.flv|.swf");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Word)]
    class WordSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly WordFileDecoder _fileDecoder = new WordFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".doc|.docx");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Excel)]
    class ExcelSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly ExcelFileDecoder _fileDecoder = new ExcelFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".xls|.xlsx");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Ppt)]
    class PptSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly PptFileDecoder _fileDecoder = new PptFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".ppt|.pptx");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.Pdf)]
    class PdfSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly PdfFileDecoder _fileDecoder = new PdfFileDecoder();

        public IMatched SuffixMatcher
        {
            get { return _selector; }
        }

        private readonly SuffixMatcher _selector = new SuffixMatcher(".pdf");
    }
}
