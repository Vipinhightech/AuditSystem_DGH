using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("WS_BLOCK_MASTER")]
    public class WS_Block_Master
    {
        [Key]
        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }
        [Column("BLOCK_NAME")]
        public string Block_Name { get; set; }
        
        public DateTime? DATE_OF_SIGNING { get; set; }
        public DateTime? EFFECTIVE_DATE {  get; set; }
    }
}
