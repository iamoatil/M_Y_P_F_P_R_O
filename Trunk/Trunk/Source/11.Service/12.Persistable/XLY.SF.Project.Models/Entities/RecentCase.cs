using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLY.SF.Project.Models.Entities
{
    /// <summary>
    /// 最近案例
    /// </summary>
    [Table("RecentCases")]
    public class RecentCase : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 ID { get; set; }

        /// <summary>
        /// 案例ID 系统生成 保证唯一性
        /// </summary>
        [Required]
        [StringLength(36)]
        public String CaseID { get; set; }

        /// <summary>
        /// 案例名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public String Name { get; set; }

        /// <summary>
        /// 案例编号
        /// </summary>
        [Required]
        [MaxLength(100)]
        public String Number { get; set; }

        [Required]
        [MaxLength(10)]
        public String Author { get; set; }

        [Required]
        [MaxLength(30)]
        public String Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 最新打开时间
        /// </summary>
        [Required]
        public DateTime LastOpenTime { get; set; }

        /// <summary>
        /// 案例CP文件路径
        /// </summary>
        [Required]
        public String CaseProjectFile { get; set; }
    }
}
