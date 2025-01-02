using AuditSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.ViewModels
{
    public class AuditBlocksViewModel
    {
        public AuditSystem_Blocks Block { get; set; }
        public IEnumerable<V_Block_Master> v_Blocks { get; set; }
    }
}
