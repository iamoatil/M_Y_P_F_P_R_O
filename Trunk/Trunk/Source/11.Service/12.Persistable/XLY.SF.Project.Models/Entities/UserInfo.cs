using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLY.SF.Project.Models.Entities
{
    [Table("UserInfos")]
    public class UserInfo : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 UserID { get; set; }

        [Required]
        [MaxLength(10)]
        public String UserName { get; set; } = String.Empty;

        public String WorkUnit { get; set; } = String.Empty;

        public String IdNumber { get; set; } = String.Empty;

        public String PhoneNumber { get; set; } = String.Empty;

        [Required]
        [MaxLength(10)]
        public String LoginUserName { get; set; } = String.Empty;

        [Required]
        public String LoginPassword { get; set; } = String.Empty;

        public DateTime LoginTime { get; set; }
    }
}
