using System.IO;
using System.Windows;
using XLY.SF.Project.PreviewFilesView.UI;

/* ==============================================================================
* Description：AppDir  
* Author     ：litao
* Create Date：2017/11/16 11:02:41
* ==============================================================================*/

namespace XLY.SF.Project.PreviewFilesView
{
    public class AppDir
    {
        private string _mainDir;
        public AppDir()
        {
            bool isToolRun = Application.Current is App;
            if (!isToolRun)
            {
                _mainDir = @"XlyToolkits\PreviewFiles\";
            }
            else
            {
                _mainDir = @".\";
            }
            _mainDir = Path.GetFullPath(_mainDir);
        }

        /// <summary>
        /// 主程序的目录
        /// </summary>
        public string MainDir { get { return _mainDir; } }
    }
}
