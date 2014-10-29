namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Included_liveMatchesTable1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.LiveMatches", name: "Match_BetServiceMatchNo", newName: "BetServiceMatchNo");
            RenameIndex(table: "dbo.LiveMatches", name: "IX_Match_BetServiceMatchNo", newName: "IX_BetServiceMatchNo");
            DropPrimaryKey("dbo.LiveMatches");
            AlterColumn("dbo.LiveMatches", "LiveMatchId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchId");
            DropColumn("dbo.LiveMatches", "ShortMatchCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LiveMatches", "ShortMatchCode", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.LiveMatches");
            AlterColumn("dbo.LiveMatches", "LiveMatchId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchId");
            RenameIndex(table: "dbo.LiveMatches", name: "IX_BetServiceMatchNo", newName: "IX_Match_BetServiceMatchNo");
            RenameColumn(table: "dbo.LiveMatches", name: "BetServiceMatchNo", newName: "Match_BetServiceMatchNo");
        }
    }
}
