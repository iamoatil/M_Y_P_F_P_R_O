using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Models.Entities
{
    [Table("Inspections")]
    public class Inspection : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 ID { get; set; }

        [Required]
        [MaxLength(50)]
        public String NameCn { get; set; }

        [Required]
        [MaxLength(50)]
        public String CategoryCn { get; set; }

        [Required]
        [MaxLength(50)]
        public String NameEn { get; set; }

        [Required]
        [MaxLength(50)]
        public String CategoryEn { get; set; }

        [Required]
        public String ConfigFile { get; set; }

        public Int32 SelectedToken { get; set; }

        [NotMapped]
        public Boolean IsSelected
        {
            get => SelectedToken > 0;
            set => SelectedToken = value ? 1 : 0;
        }
    }
}
