namespace AuditSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audit_Attachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Block_Id = c.Int(nullable: false),
                        Year = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        DateofAttachment = c.DateTime(nullable: false),
                        Doc_Address = c.String(),
                        Updated_Date = c.String(),
                        Updated_By = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditSystem_Blocks", t => t.Block_Id, cascadeDelete: true)
                .Index(t => t.Block_Id);
            
            CreateTable(
                "dbo.AuditSystem_Blocks",
                c => new
                    {
                        Block_Id = c.Int(nullable: false, identity: true),
                        Block_Name = c.String(),
                        Psc_Start_Date = c.DateTime(nullable: false),
                        Block_Current_Status = c.String(),
                        Block_Category = c.Int(nullable: false),
                        Revenue_Till_Date = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expenditure_Till_Date = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Updated_Status_Year = c.String(),
                        UpdatedDate = c.String(),
                        UpdatedBy = c.String(),
                        Submit_Status = c.String(),
                    })
                .PrimaryKey(t => t.Block_Id);
            
            CreateTable(
                "dbo.Audit_Exception_Details",
                c => new
                    {
                        ExceptionId = c.Int(nullable: false, identity: true),
                        Block_Id = c.Int(nullable: false),
                        Year = c.String(),
                        Name_Of_Auditor = c.String(),
                        ExceptionNo = c.Int(nullable: false),
                        ExceptionSubNo = c.String(),
                        NatureOfException = c.String(),
                        ExceptionTitle = c.String(),
                        ZistOfException = c.String(),
                        ExceptionType = c.String(),
                        Quantum = c.Double(nullable: false),
                        OperatorsReply = c.String(),
                        CFComments = c.String(),
                        BlockCoordinatorsComments = c.String(),
                        FinalRecommendations = c.String(),
                        ActionTaken = c.String(),
                        CurrentStatus = c.String(),
                        Remark = c.String(),
                        S_Status = c.Int(nullable: false),
                        S_InitiatedBy = c.String(),
                        S_ApprovedBy = c.String(),
                        S_RejectedBy = c.String(),
                        S_Remark = c.String(),
                        S_Doc_Address = c.String(),
                        Submit_Status = c.String(),
                        Updated_Date = c.String(),
                        Updated_By = c.String(),
                    })
                .PrimaryKey(t => t.ExceptionId)
                .ForeignKey("dbo.AuditSystem_Blocks", t => t.Block_Id, cascadeDelete: true)
                .Index(t => t.Block_Id);
            
            CreateTable(
                "dbo.Audit_FurtherQuery_Details",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExceptionId = c.Int(nullable: false),
                        FurtherQuery = c.String(),
                        FurtherOperatorsReply = c.String(),
                        FurtherCFComments = c.String(),
                        FurtherBlockCoordinatorsComments = c.String(),
                        FurtherFinalRecommendations = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Audit_Exception_Details", t => t.ExceptionId, cascadeDelete: true)
                .Index(t => t.ExceptionId);
            
            CreateTable(
                "dbo.WS_Block_Master",
                c => new
                    {
                        Block_Id = c.Int(nullable: false, identity: true),
                        Block_Name = c.String(),
                        Psc_Start_Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Block_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Audit_FurtherQuery_Details", "ExceptionId", "dbo.Audit_Exception_Details");
            DropForeignKey("dbo.Audit_Exception_Details", "Block_Id", "dbo.AuditSystem_Blocks");
            DropForeignKey("dbo.Audit_Attachments", "Block_Id", "dbo.AuditSystem_Blocks");
            DropIndex("dbo.Audit_FurtherQuery_Details", new[] { "ExceptionId" });
            DropIndex("dbo.Audit_Exception_Details", new[] { "Block_Id" });
            DropIndex("dbo.Audit_Attachments", new[] { "Block_Id" });
            DropTable("dbo.WS_Block_Master");
            DropTable("dbo.Audit_FurtherQuery_Details");
            DropTable("dbo.Audit_Exception_Details");
            DropTable("dbo.AuditSystem_Blocks");
            DropTable("dbo.Audit_Attachments");
        }
    }
}
