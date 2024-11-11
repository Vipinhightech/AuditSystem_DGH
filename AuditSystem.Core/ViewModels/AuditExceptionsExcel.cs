using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditSystem.Core.Models;

namespace AuditSystem.Core.ViewModels
{
    public class AuditExceptionsExcel
    {
       
        public int X_Id { get; set; }
       
        public string Year { get; set; }

        public string ToYear { get; set; }
       
        public string Name_Of_Auditor { get; set; }
        
        public int ExceptionNo { get; set; }
      
        public string ExceptionSubNo { get; set; }
   
        public string NatureOfException { get; set; }
     
        public string ExceptionTitle { get; set; }
   
        public string ZistOfException { get; set; }
     
        public string ExceptionType { get; set; } //financial , non-financial

        public string Currency { get; set; } //USD , INR
       
        public double Quantum { get; set; }
   
        public string OperatorsReply { get; set; }
     
        public string CFComments { get; set; }
  
        public string BlockCoordinatorsComments { get; set; }
   
        public string FinalRecommendations { get; set; }
    
        public string CurrentStatus { get; set; }
   
        public string Remark { get; set; }

        public DateTime? EXCEPTION_ISSUE_DATE { get; set; }

        public DateTime? OPERATOR_REPLY_DATE { get; set; }

        public virtual List<Audit_FurtherQuery_Details> FurtherQuery { get; set; }
    }
}
