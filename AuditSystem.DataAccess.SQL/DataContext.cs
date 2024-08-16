using AuditSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSystem.DataAccess.SQL
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=OracleConnection")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<AuditSystem_Blocks>().HasKey(k => k.Block_Name);
            //modelBuilder.Entity<Audit_Exception_Details>().HasKey(k => k.ExceptionId);
            //modelBuilder.Entity<WS_Block_Master>().HasKey(k => k.Block_Id);
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("AUDIT");
           
        }
 
        public DbSet<AuditSystem_Blocks> AuditSystem_Blocks { get; set; }
        public DbSet<Audit_Exception_Details> Audit_Exception_Details { get; set; }
        public DbSet<Audit_FurtherQuery_Details> Audit_FurtherQuery_Details { get; set; }
        public DbSet<WS_Block_Master> WS_Block_Masters { get; set; }
        public DbSet<Audit_Attachments> Audit_Attachments { get; set; }
    }
}
