using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_YEAR_REVENUE_EXPENDITURE")]
    public class Audit_Year_Revenue_Expenditure
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Column("YEAR")]
        public string Year { get; set; }

        [Required(ErrorMessage = "Revenue is required.")]
        [Column("REVENUE")]
        public decimal Revenue { get; set; }

        [Required(ErrorMessage = "Expenditure of attachment is required.")]
        [Column("EXPENDITURE")]
        public decimal Expenditure { get; set; }

        public virtual AuditSystem_Blocks Block { get; set; }
    }
}
