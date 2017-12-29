using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/* ==============================================================================
* Description：AppDir  
* Author     ：litao
* Create Date：2017/11/16 11:02:41
* ==============================================================================*/

namespace XLY.SF.Project.AndroidImg9008
{
    public static class AppDir
    {
        private static string _mainDir;
        private static string _backgroudAppDir;
        static AppDir()
        {
            bool isToolRun = Application.Current is App;
            if(!isToolRun)
            {
                _mainDir = @"XlyToolkits\Mirror\";                
            }
            else
            {
                _mainDir = @".\";
            }
            _mainDir = Path.GetFullPath(_mainDir);
            _backgroudAppDir = _mainDir + @"BackgroundMirror\";
        }

        /// <summary>
        /// 主程序的目录
        /// </summary>
        public static string MainDir { get { return _mainDir; } }

        /// <summary>
        /// 后台程序的目录
        /// </summary>
        public static string BackgroundAppDir { get { return _backgroudAppDir; } }
    }
}
