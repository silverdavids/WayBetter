namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_the_shifts_and_terminals_model : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.People", newName: "Employees");
            DropIndex("dbo.Employees", new[] { "BranchId" });
            AlterColumn("dbo.Employees", "HireDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Employees", "BranchId", c => c.Int(nullable: false));
            CreateIndex("dbo.Employees", "BranchId");
            DropColumn("dbo.Employees", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropIndex("dbo.Employees", new[] { "BranchId" });
            AlterColumn("dbo.Employees", "BranchId", c => c.Int());
            AlterColumn("dbo.Employees", "HireDate", c => c.DateTime());
            CreateIndex("dbo.Employees", "BranchId");
            RenameTable(name: "dbo.Employees", newName: "People");
        }
    }
}
