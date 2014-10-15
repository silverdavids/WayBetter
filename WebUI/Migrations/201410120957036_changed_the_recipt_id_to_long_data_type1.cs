namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_the_recipt_id_to_long_data_type1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "RecieptId", "dbo.Receipts");
            DropForeignKey("dbo.CancelReciepts", "RecieptId", "dbo.Receipts");
            DropIndex("dbo.Bets", new[] { "RecieptId" });
            DropIndex("dbo.CancelReciepts", new[] { "RecieptId" });
            DropPrimaryKey("dbo.Receipts");
            DropPrimaryKey("dbo.CancelReciepts");
            AlterColumn("dbo.Receipts", "ReceiptId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Bets", "RecieptId", c => c.Int(nullable: false));
            AlterColumn("dbo.CancelReciepts", "RecieptId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Receipts", "ReceiptId");
            AddPrimaryKey("dbo.CancelReciepts", "RecieptId");
            CreateIndex("dbo.Bets", "RecieptId");
            CreateIndex("dbo.CancelReciepts", "RecieptId");
            AddForeignKey("dbo.Bets", "RecieptId", "dbo.Receipts", "ReceiptId", cascadeDelete: true);
            AddForeignKey("dbo.CancelReciepts", "RecieptId", "dbo.Receipts", "ReceiptId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CancelReciepts", "RecieptId", "dbo.Receipts");
            DropForeignKey("dbo.Bets", "RecieptId", "dbo.Receipts");
            DropIndex("dbo.CancelReciepts", new[] { "RecieptId" });
            DropIndex("dbo.Bets", new[] { "RecieptId" });
            DropPrimaryKey("dbo.CancelReciepts");
            DropPrimaryKey("dbo.Receipts");
            AlterColumn("dbo.CancelReciepts", "RecieptId", c => c.Long(nullable: false));
            AlterColumn("dbo.Bets", "RecieptId", c => c.Long(nullable: false));
            AlterColumn("dbo.Receipts", "ReceiptId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.CancelReciepts", "RecieptId");
            AddPrimaryKey("dbo.Receipts", "ReceiptId");
            CreateIndex("dbo.CancelReciepts", "RecieptId");
            CreateIndex("dbo.Bets", "RecieptId");
            AddForeignKey("dbo.CancelReciepts", "RecieptId", "dbo.Receipts", "ReceiptId");
            AddForeignKey("dbo.Bets", "RecieptId", "dbo.Receipts", "ReceiptId", cascadeDelete: true);
        }
    }
}
