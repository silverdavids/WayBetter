namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Included_liveMatchesTable5 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LiveMatches");
            AlterColumn("dbo.LiveMatches", "LiveMatchId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LiveMatches");
            AlterColumn("dbo.LiveMatches", "LiveMatchId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.LiveMatches", "LiveMatchId");
        }
    }
}
