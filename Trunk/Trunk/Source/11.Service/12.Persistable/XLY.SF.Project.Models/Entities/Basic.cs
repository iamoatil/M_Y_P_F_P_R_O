using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Models.Entities
{
    [Table("Basics")]
    public class Basic: IEntity
    {
        [Key]
        [MaxLength(100)]
        public String Key { get; set; }

        public String Value { get; set; }
    }
}
