using System;

namespace XLY.SF.Project.Domains
{

    [Serializable]
    public class FileMd5DataItem : AbstractDataItem
    {
        /// <summary>
        /// 固定的文件路径
        /// </summary>
        [Display]
        public string FilePath { get; private set; }

        /// <summary>
        /// 文件的Md5
        /// </summary>
        [Display]
        public string FileMd5 { get; private set; }

        /// <summary>
        /// BaseDir
        /// </summary>
        public string BaseDir { get; private set; }

        public FileMd5DataItem(string baseDir,string filePath, string fileMd5)
        {
            BaseDir = baseDir;
            this.FilePath = filePath;
            this.FileMd5 = fileMd5;
        }

        public override string ToString()
        {
            return FilePath + " \t" + FileMd5;
        }
    }
}
