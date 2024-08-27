using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    [Table("AUDIT_FURTHERQUERY_DETAILS")]
    public class Audit_FurtherQuery_Details
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("EXCEPTIONID")]
        public int ExceptionId { get; set; }
        [Column("FURTHERQUERY")]
        public string FurtherQuery { get; set; }
        [Column("FURTHEROPERATORSREPLY")]
        public string FurtherOperatorsReply { get; set; }
        [Column("FURTHERCFCOMMENTS")]
        public string FurtherCFComments { get; set; }
        [Column("FURTHERBLOCKCOORDINATORSCOMMENTS")]
        public string FurtherBlockCoordinatorsComments { get; set; }
        [Column("FURTHERFINALRECOMMENDATIONS")]
        public string FurtherFinalRecommendations { get; set; }

        public virtual Audit_Exception_Details Exceptions { get; set; }
    }
}
