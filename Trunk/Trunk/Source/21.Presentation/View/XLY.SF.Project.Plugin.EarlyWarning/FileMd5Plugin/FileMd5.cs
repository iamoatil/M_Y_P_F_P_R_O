using System;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{

    [Serializable]
    public class FileMd5 : AbstractDataItem
    {
        /// <summary>
        /// 固定的文件路径
        /// </summary>
        [Display]
        public string FixedPath { get; private set; }

        /// <summary>
        /// 文件的Md5
        /// </summary>
        [Display]
        public string Md5String { get; private set; }

        /// <summary>
        /// BaseDir
        /// </summary>
        public string BaseDir { get; private set; }

        public FileMd5(string baseDir,string fixedFile, string md5String)
        {
            BaseDir = baseDir;
            this.FixedPath = fixedFile;
            this.Md5String = md5String;
        }

        public override string ToString()
        {
            return FixedPath + " \t" + Md5String;
        }
    }
}
