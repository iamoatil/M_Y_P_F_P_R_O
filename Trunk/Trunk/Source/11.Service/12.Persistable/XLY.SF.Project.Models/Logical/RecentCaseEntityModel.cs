using System;
using System.ComponentModel.DataAnnotations;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models.Logical
{
    /// <summary>
    /// 最近案例
    /// </summary>
    public class RecentCaseEntityModel : LogicalModelBase<RecentCase>
    {
        #region Constructors

        public RecentCaseEntityModel(RecentCase entity)
            : base(entity)
        {
        }

        public RecentCaseEntityModel()
        {
        }

        #endregion

        #region Properties

        [Required]
        public Int32 ID => Entity.ID;

        /// <summary>
        /// 案例ID 系统生成 保证唯一性
        /// </summary>
        [Required]
        [StringLength(36)]
        public String CaseID
        {
            get => Entity.CaseID;
            set => Entity.CaseID = value;
        }

        /// <summary>
        /// 案例名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public String Name
        {
            get => Entity.Name;
            set => Entity.Name = value;
        }

        /// <summary>
        /// 案例编号
        /// </summary>
        [Required]
        [StringLength(100)]
        public String Number
        {
            get => Entity.Number;
            set => Entity.Number = value;
        }

        [Required]
        [StringLength(10)]
        public String Author
        {
            get => Entity.Author;
            set => Entity.Author = value;
        }

        [Required]
        [StringLength(30)]
        public String Type
        {
            get => Entity.Type;
            set => Entity.Type = value;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime Timestamp
        {
            get => Entity.Timestamp;
            set => Entity.Timestamp = value;
        }

        /// <summary>
        /// 最新打开时间
        /// </summary>
        [Required]
        public DateTime LastOpenTime
        {
            get => Entity.LastOpenTime;
            set => Entity.LastOpenTime = value;
        }

        /// <summary>
        /// 案例CP文件路径
        /// </summary>
        [Required]
        public String CaseProjectFile
        {
            get => Entity.CaseProjectFile;
            set => Entity.CaseProjectFile = value;
        }

        #endregion
    }
}
