namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Included_liveMatchesTable7 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LiveMatches");
            AddColumn("dbo.LiveMatches", "LiveMatchNo", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchNo");
            DropColumn("dbo.LiveMatches", "LiveMatchId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LiveMatches", "LiveMatchId", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.LiveMatches");
            DropColumn("dbo.LiveMatches", "LiveMatchNo");
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchId");
        }
    }
}
