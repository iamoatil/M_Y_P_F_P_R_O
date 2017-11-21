using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 手机应用APP实体
    /// </summary>
    [Serializable]
    public class AppEntity : AbstractDataItem
    {
        /// <summary>
        /// 图标路径(相对路径)
        /// </summary>
        [Display(ColumnType = EnumColumnType.Image, Alignment = EnumAlignment.Center)]
        public string Icon { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 应用类型
        /// </summary>
        [Display]
        public string Type { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        [Display]
        public string AppId { get; set; }

        /// <summary>
        /// 版本信息
        /// </summary>
        public Version Version { get; set; }

        private string _VersionDesc;
        /// <summary>
        /// 版本描述
        /// </summary>
        [Display]
        public string VersionDesc
        {
            get { return _VersionDesc; }
            set
            {
                _VersionDesc = value;
                Version = value.ToSafeVersion();
            }
        }

        /// <summary>
        /// 应用描述
        /// </summary>
        [Display]
        public string Descritpion { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        [Display]
        public string InstallPath { get; set; }

        /// <summary>
        /// 数据存储路径
        /// </summary>
        [Display]
        public string DataPath { get; set; }

        /// <summary>
        /// 安装日期
        /// </summary>
        [Display]
        public DateTime? InstallDate { get; set; }

    }
}
