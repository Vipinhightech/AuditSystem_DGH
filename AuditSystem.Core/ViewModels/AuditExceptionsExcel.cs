using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.ViewModels
{
    public class AuditExceptionsExcel
    {
       
        public int X_Id { get; set; }

        //public int Block_Id { get; set; }
       
        public string Year { get; set; }
       
        public string Name_Of_Auditor { get; set; }
        
        public int ExceptionNo { get; set; }
      
        public string ExceptionSubNo { get; set; }
   
        public string NatureOfException { get; set; }
     
        public string ExceptionTitle { get; set; }
   
        public string ZistOfException { get; set; }
     
        public string ExceptionType { get; set; } //financial , non-financial
       
        public double Quantum { get; set; }
   
        public string OperatorsReply { get; set; }
     
        public string CFComments { get; set; }
  
        public string BlockCoordinatorsComments { get; set; }
   
        public string FinalRecommendations { get; set; }
  
        //public string ActionTaken { get; set; }    //Settled, Unsettled
    
        public string CurrentStatus { get; set; }
   
        public string Remark { get; set; }

        //settlement of exception
    
        //public int S_Status { get; set; } //0-Unsettled, 1-Pending Approval, 2- Settled
   
        //public string S_InitiatedBy { get; set; }
   
        //public string S_ApprovedBy { get; set; }
   
        //public string S_RejectedBy { get; set; }
    
        //public string S_Remark { get; set; }
     
        //public string S_Doc_Address { get; set; }


     
        //public string Submit_Status { get; set; } //final, submit
     
        //public string Updated_Date { get; set; }
     
        //public string Updated_By { get; set; }
    }
}
