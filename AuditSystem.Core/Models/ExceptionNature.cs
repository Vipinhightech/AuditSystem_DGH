using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_NATURE_EXCEPTION")]
    public class ExceptionNature
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column("NATURE_OF_EXCEPTION")]
        public string NatureOfException { get; set; }

    }
}
