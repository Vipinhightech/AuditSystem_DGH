using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_UPDATE_LOG")]
    public class AUDIT_UPDATE_LOG
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IP")]
        public string IP { get; set; }

        [Column("USERID")]
        public string USERID { get; set; }

        [Column("TIME")]
        public DateTime TIME { get; set; }

        [Column("ACTIVITY")]
        public string ACTIVITY { get; set; }

        [Column("TABLE_NAME")]
        public string TABLE_NAME { get; set; }

        [Column("ACTIVITY_VALUES")]
        public string ACTIVITY_VALUES { get; set; }

        [Column("REMARKS")]
        public string REMARKS { get; set; }
    }
}
