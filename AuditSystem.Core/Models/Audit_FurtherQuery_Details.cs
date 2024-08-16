using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("Audit_FurtherQuery_Details")]
    public class Audit_FurtherQuery_Details
    {
        public int Id { get; set; }
        public int ExceptionId { get; set; }
        public string FurtherQuery { get; set; }
        public string FurtherOperatorsReply { get; set; }
        public string FurtherCFComments { get; set; }
        public string FurtherBlockCoordinatorsComments { get; set; }
        public string FurtherFinalRecommendations { get; set; }
        public virtual Audit_Exception_Details Exceptions { get; set; }
    }
}
