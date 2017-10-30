using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using XLY.SF.Project.UserControls.PreviewFile.Decoders;
using XLY.SF.Project.UserControls.PreviewFile.Selector;

namespace XLY.SF.Project.UserControls.PreviewFile.FileDecode
{
    /// <summary>
    /// 文件解码器集合
    /// 功能：其中包含多种可以解析文件的解码器的
    /// </summary>
    public class FileDecoderCollection
    {
        public FileDecoderCollection()
        {
            Compose();
        }

        private void Compose()
        {
            AssemblyCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        [ImportMany]
        public Lazy<ISuffixFileDecoder, IFileDecoderCapabilities>[] SuffixFileDecoders { get; set; }

        /// <summary>
        /// 根据类型获取文件解码器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFileDecoder GetFileDecoder(FileDecoderTypes type)
        {
            if (SuffixFileDecoders == null)
            {
                return null;
            }

            foreach (var item in SuffixFileDecoders)
            {
                if (item.Metadata.FileDecoderType == type)
                {
                    return item.Value.FileDecoder;
                }
            }

            return null;
        }

        /// <summary>
        /// 解析一个Path为控件
        /// </summary>
        /// <returns></returns>
        public FrameworkElement Decode(string filePath)
        {
            if (SuffixFileDecoders == null)
            {
                return null;
            }
            string suffix = Path.GetExtension(filePath);

            foreach (var item in SuffixFileDecoders)
            {
                IFileDecoder fileDecoder = item.Value.FileDecoder;
                ISuffixSelector suffixSelector=item.Value.SuffixSelector;
                if (suffixSelector.IsMatch(suffix))
                {
                    fileDecoder.Decode(filePath);
                    return fileDecoder.Element;
                }
            }

            return null;
        }
    }    
}
