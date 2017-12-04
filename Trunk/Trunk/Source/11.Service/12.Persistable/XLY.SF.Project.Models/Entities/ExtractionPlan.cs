using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Models.Entities
{
    [Table("ExtractionPlans")]
    public class ExtractionPlan : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 ID { get; set; }

        /// <summary>
        /// 方案名称。
        /// </summary>
        [Required]
        [MaxLength(50)]
        public String Name { get; set; }

        /// <summary>
        /// 以逗号分隔的提取项Token列表。
        /// </summary>
        public String ExtractItems { get; set; }
    }
}
