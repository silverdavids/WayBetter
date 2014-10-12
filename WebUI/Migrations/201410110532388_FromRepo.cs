namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FromRepo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Branches", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Employees", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Shifts", "PersonId", "dbo.Employees");
            DropForeignKey("dbo.AspNetUsers", "PersonId", "dbo.Employees");
            DropForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals");
            DropIndex("dbo.Branches", new[] { "BranchTypeId" });
            DropIndex("dbo.Branches", new[] { "CompanyId" });
            DropIndex("dbo.Employees", new[] { "BranchId" });
            DropIndex("dbo.Shifts", new[] { "PersonId" });
            DropIndex("dbo.Shifts", new[] { "TerminalId" });
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.AspNetUsers", new[] { "PersonId" });
            AddColumn("dbo.Shifts", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shifts", "OpeningCash", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shifts", "ClosingCash", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shifts", "UserId", c => c.String());
            AlterColumn("dbo.Branches", "ManagerId", c => c.Int());
            AlterColumn("dbo.Branches", "BranchTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Shifts", "AssignedBy", c => c.String());
            AlterColumn("dbo.Shifts", "TerminalId", c => c.Int(nullable: false));
            AlterColumn("dbo.Terminals", "BranchId", c => c.Int(nullable: false));
            AlterColumn("dbo.Countries", "CountryName", c => c.String(nullable: false));
            CreateIndex("dbo.Branches", "BranchTypeId");
            CreateIndex("dbo.Terminals", "BranchId");
            CreateIndex("dbo.Shifts", "TerminalId");
            AddForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes", "BranchTypeId", cascadeDelete: true);
            AddForeignKey("dbo.Terminals", "BranchId", "dbo.Branches", "BranchId", cascadeDelete: true);
            AddForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals", "TerminalId", cascadeDelete: true);
            DropColumn("dbo.Branches", "BranchCode");
            DropColumn("dbo.Branches", "DateCreated");
            DropColumn("dbo.Branches", "MaxCash");
            DropColumn("dbo.Branches", "MinStake");
            DropColumn("dbo.Branches", "CompanyId");
            DropColumn("dbo.Shifts", "StartTime");
            DropColumn("dbo.Shifts", "EndTime");
            DropColumn("dbo.Shifts", "IsClosed");
            DropColumn("dbo.Shifts", "StartCash");
            DropColumn("dbo.Shifts", "CashIn");
            DropColumn("dbo.Shifts", "CashOut");
            DropColumn("dbo.Shifts", "PersonId");
            DropColumn("dbo.Terminals", "IpAddress");
            DropColumn("dbo.Terminals", "DateCreated");
            DropColumn("dbo.Terminals", "isActive");
            DropColumn("dbo.AspNetUsers", "PersonId");
            DropTable("dbo.Employees");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.PersonId);
            
            AddColumn("dbo.AspNetUsers", "PersonId", c => c.Int());
            AddColumn("dbo.Terminals", "isActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Terminals", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Terminals", "IpAddress", c => c.String());
            AddColumn("dbo.Shifts", "PersonId", c => c.Int(nullable: false));
            AddColumn("dbo.Shifts", "CashOut", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "CashIn", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "StartCash", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Shifts", "IsClosed", c => c.Boolean());
            AddColumn("dbo.Shifts", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shifts", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Branches", "CompanyId", c => c.Int());
            AddColumn("dbo.Branches", "MinStake", c => c.Decimal(storeType: "money"));
            AddColumn("dbo.Branches", "MaxCash", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Branches", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Branches", "BranchCode", c => c.String());
            DropForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes");
            DropIndex("dbo.Shifts", new[] { "TerminalId" });
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.Branches", new[] { "BranchTypeId" });
            AlterColumn("dbo.Countries", "CountryName", c => c.String());
            AlterColumn("dbo.Terminals", "BranchId", c => c.Int());
            AlterColumn("dbo.Shifts", "TerminalId", c => c.Int());
            AlterColumn("dbo.Shifts", "AssignedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.Branches", "BranchTypeId", c => c.Int());
            AlterColumn("dbo.Branches", "ManagerId", c => c.String());
            DropColumn("dbo.Shifts", "UserId");
            DropColumn("dbo.Shifts", "ClosingCash");
            DropColumn("dbo.Shifts", "OpeningCash");
            DropColumn("dbo.Shifts", "StartDate");
            CreateIndex("dbo.AspNetUsers", "PersonId");
            CreateIndex("dbo.Terminals", "BranchId");
            CreateIndex("dbo.Shifts", "TerminalId");
            CreateIndex("dbo.Shifts", "PersonId");
            CreateIndex("dbo.Employees", "BranchId");
            CreateIndex("dbo.Branches", "CompanyId");
            CreateIndex("dbo.Branches", "BranchTypeId");
            AddForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals", "TerminalId");
            AddForeignKey("dbo.Terminals", "BranchId", "dbo.Branches", "BranchId");
            AddForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes", "BranchTypeId");
            AddForeignKey("dbo.AspNetUsers", "PersonId", "dbo.Employees", "PersonId");
            AddForeignKey("dbo.Shifts", "PersonId", "dbo.Employees", "PersonId", cascadeDelete: true);
            AddForeignKey("dbo.Employees", "BranchId", "dbo.Branches", "BranchId", cascadeDelete: true);
            AddForeignKey("dbo.Branches", "CompanyId", "dbo.Companies", "CompanyId");
        }
    }
}
