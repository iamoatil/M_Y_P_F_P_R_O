using System.ComponentModel.Composition;
/* ==============================================================================
* Description：SuffixFileDecoder  
* Author     ：litao
* Create Date：2017/10/26 11:34:03
* ==============================================================================*/
using XLY.SF.Project.UserControls.PreviewFile.Decoders;
using XLY.SF.Project.UserControls.PreviewFile.Selector;

namespace XLY.SF.Project.UserControls.PreviewFile.FileDecode
{
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    //[ExportFileDecoder(FileDecoderType = FileDecoderTypes.Audio)]
    class AudioSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly AudioFileDecoder _fileDecoder=new AudioFileDecoder();

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".mp3|.wma|.ape|.flac|.aac|.ac3|.mmf|.amr|.m4a|.m4r|.ogg|.wav|.mp2");
    }

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportFileDecoder(FileDecoderType = FileDecoderTypes.AudioVLC)]
    class AudioVLCSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly AudioVLCFileDecoder _fileDecoder = new AudioVLCFileDecoder();

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".mp3|.wma|.ape|.flac|.aac|.ac3|.mmf|.amr|.m4a|.m4r|.ogg|.wav|.mp2");
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

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".bin");
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

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".html|.Xml");
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

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".jpg|.png|.ico|.bmp|.tif|.tga|.gif");
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

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".txt|.ini");
    }

    //[PartCreationPolicy(CreationPolicy.NonShared)]
    //[ExportFileDecoder(FileDecoderType = FileDecoderTypes.Video)]
    class VideoSuffixFileDecoder : ISuffixFileDecoder
    {
        public IFileDecoder FileDecoder
        {
            get { return _fileDecoder; }
        }
        private readonly VideoFileDecoder _fileDecoder = new VideoFileDecoder();

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".avi|.rmvb|.rm|.mp4|.mkv|.webM|.3gp|.WMV|.MPG|.vob|.mov|.flv|.swf");
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

        public ISuffixSelector SuffixSelector
        {
            get { return _selector; }
        }

        private readonly SuffixSelector _selector = new SuffixSelector(".avi|.rmvb|.rm|.mp4|.mkv|.webM|.3gp|.WMV|.MPG|.vob|.mov|.flv|.swf");
    }
}
