using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_ATTACHMENTS")]
    public class Audit_Attachments
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }
        [Required]
        [Column("YEAR")]
        public string Year { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [Column("TITLE")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Date of attachment is required.")]
        [Display(Name = "DateOfLetter")]
        [Column("DATEOFATTACHMENT")]
        public DateTime DateofAttachment { get; set; }

        [Column("DOC_ADDRESS")]
        public string Doc_Address { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? Updated_Date { get; set; }

        [Column("UPDATED_BY")]
        public string Updated_By { get; set; }

        public virtual AuditSystem_Blocks Block { get; set; }

    }
}
