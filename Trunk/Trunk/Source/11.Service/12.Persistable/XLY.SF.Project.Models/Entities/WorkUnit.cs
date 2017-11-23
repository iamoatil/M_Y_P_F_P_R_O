using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Models.Entities
{
    [Table("WorkUnits")]
    public class WorkUnit : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 ID { get; set; }

        [Required]
        [MaxLength(200)]
        public String Unit { get; set; }

        [Required]
        [MaxLength(100)]
        public String Number { get; set; }
    }
}
