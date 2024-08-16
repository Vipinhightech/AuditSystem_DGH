using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("WS_Block_Master")]
    public class WS_Block_Master
    {
        [Key]
        public int Block_Id { get; set; }
        public string Block_Name { get; set; }
        public DateTime Psc_Start_Date { get; set; }
    }
}
