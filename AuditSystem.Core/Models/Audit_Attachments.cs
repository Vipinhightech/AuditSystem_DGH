﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.Core.Models
{
    public class Audit_Attachments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Block_Id { get; set; }
        [Required]
        public string Year { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "DateOfLetter")]
        public DateTime DateofAttachment { get; set; }

        public string Doc_Address { get; set; }

        public string Updated_Date { get; set; }

        public string Updated_By { get; set; }

        public virtual AuditSystem_Blocks Block { get; set; }

    }
}
