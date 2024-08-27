using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_EXCEPTION_DETAILS")]
    public class Audit_Exception_Details
    {
        [Key]
        [Column("EXCEPTIONID")]
        public int ExceptionId { get; set; }

        [Required]
        [Column("BLOCK_ID")]
        public int Block_Id { get; set; }
        [Column("YEAR")]
        public string Year { get; set; }
        [Column("NAME_OF_AUDITOR")]
        public string Name_Of_Auditor { get; set; }
        [Column("EXCEPTIONNO")]
        public int ExceptionNo { get; set; }
        [Column("EXCEPTIONSUBNO")]
        public string ExceptionSubNo { get; set; }
        [Column("NATUREOFEXCEPTION")]
        public string NatureOfException { get; set; }
        [Column("EXCEPTIONTITLE")]
        public string ExceptionTitle { get; set; }
        [Column("ZISTOFEXCEPTION")]
        public string ZistOfException { get; set; }
        [Column("EXCEPTIONTYPE")]
        public string ExceptionType { get; set; } //financial , non-financial
        [Column("QUANTUM")]
        public double Quantum { get; set; }
        [Column("OPERATORSREPLY")]
        public string OperatorsReply { get; set; }
        [Column("CFCOMMENTS")]
        public string CFComments { get; set; }
        [Column("BLOCKCOORDINATORSCOMMENTS")]
        public string BlockCoordinatorsComments { get; set; }
        [Column("FINALRECOMMENDATIONS")]
        public string FinalRecommendations { get; set; }
        [Column("ACTIONTAKEN")]
        public string ActionTaken { get; set; }    //Settled, Unsettled
        [Column("CURRENTSTATUS")]
        public string CurrentStatus { get; set; }
        [Column("REMARK")]
        public string Remark { get; set; }

        //settlement of exception
        [Column("S_STATUS")]
        public int S_Status { get; set; } //0-Unsettled, 1-Pending Approval, 2- Settled
        [Column("S_INITIATEDBY")]
        public string S_InitiatedBy { get; set; }
        [Column("S_APPROVEDBY")]
        public string S_ApprovedBy { get; set; }
        [Column("S_REJECTEDBY")]
        public string S_RejectedBy {  get; set; }
        [Column("S_REMARK")]
        public string S_Remark { get; set; }
        [Column("S_DOC_ADDRESS")]
        public string S_Doc_Address { get; set; }


        [Column("SUBMIT_STATUS")]
        public string Submit_Status { get; set; } //final, submit
        [Column("UPDATED_DATE")]
        public string Updated_Date { get; set; }
        [Column("UPDATED_BY")]
        public string Updated_By { get; set; }

        public virtual List<Audit_FurtherQuery_Details> FurtherQuery { get; set; }
        public virtual AuditSystem_Blocks Block { get; set; }

    }
}
