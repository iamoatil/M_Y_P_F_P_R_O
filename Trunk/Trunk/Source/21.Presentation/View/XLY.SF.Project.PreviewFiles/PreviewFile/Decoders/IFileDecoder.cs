using System.Windows;
using XLY.SF.Project.PreviewFilesView.PreviewFile.MatchDecoder;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
{
    /// <summary>
    /// 文件解码器接口
    /// </summary>
    public interface IFileDecoder
    {
        /// <summary>
        /// 解码生成的界面元素
        /// </summary>
        FrameworkElement Element { get; }

        /// <summary>
        /// 解码一个文件
        /// </summary>
        /// <param name="path">文件路径</param>
        void Decode(string path);
    }

    /// <summary>
    /// 后缀文件解码器。根据后缀自动判断使用的哪种解码器
    /// </summary>
    public interface ISuffixFileDecoder 
    {
        IFileDecoder FileDecoder { get; }

        IMatched SuffixMatcher { get; }
    }
}
