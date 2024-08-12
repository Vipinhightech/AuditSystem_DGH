using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    public class AuditSystem_Blocks
    {
        [Key]
        [Required]
        public int Block_Id { get; set; }

        [Display(Name = "Block Name")]
        public string Block_Name { get; set; }

        [Display(Name = "PSC start Date")]
        public DateTime Psc_Start_Date { get; set; }

        [Display(Name = "Block Current Status")]
        public string Block_Current_Status { get; set; }

        [Display(Name = "Block Category")]
        public int Block_Category { get; set; }

        public decimal Revenue_Till_Date { get; set; }

        public decimal Expenditure_Till_Date { get; set; }

        public string Updated_Status_Year { get; set; }

        public string UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }
        
        public string Submit_Status { get; set; } //{draft, submit} can use if do not want to allow users to edit block details

        public virtual ICollection<Audit_Exception_Details> Exceptions { get; set; }
        public virtual ICollection<Audit_Attachments> Attachments { get; set; }
    }
}
