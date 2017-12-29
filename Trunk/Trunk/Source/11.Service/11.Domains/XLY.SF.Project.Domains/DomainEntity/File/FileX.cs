using System;

namespace XLY.SF.Project.Domains
{
    [Serializable]
    public class FileX : AbstractDataItem
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Display]
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 文件所在文件夹
        /// </summary>
        [Display]
        public string ParentDirectory { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 最后访问日期
        /// </summary>

        [Display]
        public DateTime LastAccessDate { get; set; }

        /// <summary>
        /// 最后编辑日期
        /// </summary>
        [Display]
        public DateTime LastWriteData { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [Display]
        public long Size { get; set; }

    }
}

