namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New_Migration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.Branches", new[] { "BranchTypeId" });
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.Shifts", new[] { "TerminalId" });
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        HireDate = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        MiddleName = c.String(),
                        Surname = c.String(nullable: false, maxLength: 50),
                        Gender = c.Int(nullable: false),
                        Email = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PhoneNo = c.String(),
                        Mobile = c.String(),
                        Photo = c.String(),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            AddColumn("dbo.Branches", "BranchCode", c => c.String());
            AddColumn("dbo.Branches", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Branches", "MaxCash", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Branches", "MinStake", c => c.Decimal(storeType: "money"));
            AddColumn("dbo.Branches", "CompanyId", c => c.Int());
            AddColumn("dbo.Terminals", "IpAddress", c => c.String());
            AddColumn("dbo.Terminals", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Terminals", "isActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Shifts", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shifts", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shifts", "IsClosed", c => c.Boolean());
            AddColumn("dbo.Shifts", "StartCash", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "CashIn", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "CashOut", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "PersonId", c => c.Int(nullable: false));
            AlterColumn("dbo.Branches", "ManagerId", c => c.String());
            AlterColumn("dbo.Branches", "BranchTypeId", c => c.Int());
            AlterColumn("dbo.Terminals", "BranchId", c => c.Int());
            AlterColumn("dbo.Shifts", "AssignedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.Shifts", "TerminalId", c => c.Int());
            CreateIndex("dbo.Branches", "BranchTypeId");
            CreateIndex("dbo.Branches", "CompanyId");
            CreateIndex("dbo.Shifts", "PersonId");
            CreateIndex("dbo.Shifts", "TerminalId");
            CreateIndex("dbo.Terminals", "BranchId");
            AddForeignKey("dbo.Branches", "CompanyId", "dbo.Companies", "CompanyId");
            AddForeignKey("dbo.Shifts", "PersonId", "dbo.Employees", "PersonId", cascadeDelete: true);
            AddForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes", "BranchTypeId");
            AddForeignKey("dbo.Terminals", "BranchId", "dbo.Branches", "BranchId");
            AddForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals", "TerminalId");
            DropColumn("dbo.Shifts", "StartDate");
            DropColumn("dbo.Shifts", "OpeningCash");
            DropColumn("dbo.Shifts", "ClosingCash");
            DropColumn("dbo.Shifts", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shifts", "UserId", c => c.String());
            AddColumn("dbo.Shifts", "ClosingCash", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shifts", "OpeningCash", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shifts", "StartDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes");
            DropForeignKey("dbo.Shifts", "PersonId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.Shifts", new[] { "TerminalId" });
            DropIndex("dbo.Shifts", new[] { "PersonId" });
            DropIndex("dbo.Employees", new[] { "BranchId" });
            DropIndex("dbo.Branches", new[] { "CompanyId" });
            DropIndex("dbo.Branches", new[] { "BranchTypeId" });
            AlterColumn("dbo.Shifts", "TerminalId", c => c.Int(nullable: false));
            AlterColumn("dbo.Shifts", "AssignedBy", c => c.String());
            AlterColumn("dbo.Terminals", "BranchId", c => c.Int(nullable: false));
            AlterColumn("dbo.Branches", "BranchTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Branches", "ManagerId", c => c.Int());
            DropColumn("dbo.Shifts", "PersonId");
            DropColumn("dbo.Shifts", "CashOut");
            DropColumn("dbo.Shifts", "CashIn");
            DropColumn("dbo.Shifts", "StartCash");
            DropColumn("dbo.Shifts", "IsClosed");
            DropColumn("dbo.Shifts", "EndTime");
            DropColumn("dbo.Shifts", "StartTime");
            DropColumn("dbo.Terminals", "isActive");
            DropColumn("dbo.Terminals", "DateCreated");
            DropColumn("dbo.Terminals", "IpAddress");
            DropColumn("dbo.Branches", "CompanyId");
            DropColumn("dbo.Branches", "MinStake");
            DropColumn("dbo.Branches", "MaxCash");
            DropColumn("dbo.Branches", "DateCreated");
            DropColumn("dbo.Branches", "BranchCode");
            DropTable("dbo.Employees");
            CreateIndex("dbo.Shifts", "TerminalId");
            CreateIndex("dbo.Terminals", "BranchId");
            CreateIndex("dbo.Branches", "BranchTypeId");
            AddForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals", "TerminalId", cascadeDelete: true);
            AddForeignKey("dbo.Terminals", "BranchId", "dbo.Branches", "BranchId", cascadeDelete: true);
            AddForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes", "BranchTypeId", cascadeDelete: true);
        }
    }
}
