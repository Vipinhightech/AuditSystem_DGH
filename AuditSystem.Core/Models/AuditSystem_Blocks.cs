using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDITSYSTEM_BLOCKS")]
    public class AuditSystem_Blocks
    {
        [Key]
        [Required]
        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }

        [Column("BLOCK_NAME")]
        [Display(Name = "Block Name")]
        public string Block_Name { get; set; }
        [Column("PSC_START_DATE")]
        [Display(Name = "PSC start Date")]
        public DateTime Psc_Start_Date { get; set; }
        [Column("BLOCK_CURRENT_STATUS")]
        [Display(Name = "Block Current Status")]
        public string Block_Current_Status { get; set; }
        [Column("BLOCK_CATEGORY")]
        [Display(Name = "Block Category")]
        public int Block_Category { get; set; }
        [Column("REVENUE_TILL_DATE")]
        public decimal Revenue_Till_Date { get; set; }
        [Column("EXPENDITURE_TILL_DATE")]
        public decimal Expenditure_Till_Date { get; set; }
        [Column("UPDATED_STATUS_YEAR")]
        public string Updated_Status_Year { get; set; }
        [Column("UPDATEDDATE")]
        public DateTime? UpdatedDate { get; set; }
        [Column("UPDATEDBY")]
        public string UpdatedBy { get; set; }
        [Column("SUBMIT_STATUS")]
        public string Submit_Status { get; set; } //{draft, submit} can use if do not want to allow users to edit block details

        public virtual ICollection<Audit_Exception_Details> Exceptions { get; set; }
        public virtual ICollection<Audit_Attachments> Attachments { get; set; }
        public virtual List<Audit_Year_Revenue_Expenditure> Revenue_Expenditures { get; set; }
    }
}
