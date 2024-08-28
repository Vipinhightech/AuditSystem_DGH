using AuditSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("AUDIT");
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<AuditSystem_Blocks>().HasKey(k => k.Block_Name);
            //modelBuilder.Entity<Audit_Exception_Details>().HasKey(k => k.ExceptionId);
            //modelBuilder.Entity<WS_Block_Master>().HasKey(k => k.Block_Id);

            modelBuilder.Properties<int>()
            .Where(p => p.Name.EndsWith("Id"))  // Applies to all properties ending with "Id"
            .Configure(p => p.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None));


           // modelBuilder.Entity<WS_Block_Master>()
           //.Property(b => b.Block_Id)
           //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
 
        public DbSet<AuditSystem_Blocks> AuditSystem_Blocks { get; set; }
        public DbSet<Audit_Exception_Details> Audit_Exception_Details { get; set; }
        public DbSet<Audit_FurtherQuery_Details> Audit_FurtherQuery_Details { get; set; }
        public DbSet<WS_Block_Master> WS_BLOCK_MASTER { get; set; }
        public DbSet<Audit_Attachments> Audit_Attachments { get; set; }
    }
}
