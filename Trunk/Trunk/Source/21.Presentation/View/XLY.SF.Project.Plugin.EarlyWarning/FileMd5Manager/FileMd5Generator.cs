using System.Collections.Generic;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class FileMd5Generator
    {
        /// <summary>
        /// 文件Md5记录的list
        /// </summary>
        public List<FileMd5DataItem> Md5List { get { return _md5List; } }
        private List<FileMd5DataItem> _md5List = new List<FileMd5DataItem>();

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        public void GenerateMd5(string dir, string baseDir)
        {
            Md5List.Clear();
            string direcotry = baseDir + dir;
            if (!Directory.Exists(direcotry))
            {
                return;
            }
            string[] files = Directory.GetFiles(direcotry, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string md5String = CryptographyHelper.MD5FromFileUpper(file);
                Md5List.Add(new FileMd5DataItem(baseDir, file, md5String));
            }
        }
    }
}
