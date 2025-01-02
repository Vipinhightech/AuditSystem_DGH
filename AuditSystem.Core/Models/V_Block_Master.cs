using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("V_BLOCK_MASTER")]
    public class V_Block_Master
    {
        [Key]
        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }
        [Column("BLOCK_NAME")]
        public string Block_Name { get; set; }

        [Column("BLOCK_TYPE")]
        public string Block_Type { get; set; }

        public DateTime? DATE_OF_SIGNING { get; set; }
        public DateTime? EFFECTIVE_DATE { get; set; }
    }
}
