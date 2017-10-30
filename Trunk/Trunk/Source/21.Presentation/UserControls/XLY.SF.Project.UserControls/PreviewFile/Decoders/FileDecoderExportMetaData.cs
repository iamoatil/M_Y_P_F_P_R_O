using System;
using System.ComponentModel.Composition;

/* ==============================================================================
* Description：ExportMetaData  
* Author     ：litao
* Create Date：2017/10/25 19:50:41
* ==============================================================================*/

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
{
    /// <summary>
    /// 导出文件解码器的特性
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportFileDecoderAttribute : ExportAttribute
    {
        public ExportFileDecoderAttribute()
            : base(typeof(ISuffixFileDecoder))
        {           
        }

        public FileDecoderTypes FileDecoderType { get; set; }
    }

    /// <summary>
    /// IFileDecoder的容器
    /// </summary>
    public interface IFileDecoderCapabilities
    {
        FileDecoderTypes FileDecoderType { get; }
    }

    /// <summary>
    /// FileDecoder的类型
    /// </summary>
    public enum FileDecoderTypes
    {
        Bin,
        Audio,
        AudioVLC,        
        Html,
        Picture,
        Text,
        Video,
        VideoVLC,
        //预留占坑的类型
        Decoder0,
        Decoder1,
        Decoder2,
        Decoder3,
        Decoder4,
        Decoder5,
        Decoder6,
        Decoder7,
    }
}
