using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 联系人
    /// </summary>
    [Serializable]
    public class Contact : AbstractDataItem
    {
        /// <summary>
        /// 电话号码
        /// </summary>
        [Display]
        public string Number { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        [Display]
        public string GroupName { get; set; }

        /// <summary>
        /// 号码归属地
        /// </summary>
        [Display]
        public string LocationInfo { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Display(Key = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 组织(公司+职位）
        /// </summary>
        [Display(Key = "组织")]
        public string Organization { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Key = "地址")]
        public string PostalAddress { get; set; }

        /// <summary>
        /// 最后联系时间
        /// </summary>
        [Display]
        public DateTime? LastContactDate { get; set; }

        /// <summary>
        /// 其他备注
        /// </summary>
        [Display]
        public string Remark { get; set; }

        /// <summary>
        /// 联系人创建时间。
        /// </summary>
        [Display]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 即时通信号
        /// </summary>
        [Display]
        public string ImNumber { get; set; }

        /// <summary>
        /// 临时属性 用于安卓联系人解析
        /// </summary>
        public int Id { get; set; }

    }
}
