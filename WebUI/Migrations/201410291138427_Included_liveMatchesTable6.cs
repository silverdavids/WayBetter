namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Included_liveMatchesTable6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LiveMatches", "BetServiceMatchNo", "dbo.Matches");
            DropIndex("dbo.LiveMatches", new[] { "BetServiceMatchNo" });
            AlterColumn("dbo.LiveMatches", "BetServiceMatchNo", c => c.Int(nullable: false));
            CreateIndex("dbo.LiveMatches", "BetServiceMatchNo");
            AddForeignKey("dbo.LiveMatches", "BetServiceMatchNo", "dbo.Matches", "BetServiceMatchNo", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiveMatches", "BetServiceMatchNo", "dbo.Matches");
            DropIndex("dbo.LiveMatches", new[] { "BetServiceMatchNo" });
            AlterColumn("dbo.LiveMatches", "BetServiceMatchNo", c => c.Int());
            CreateIndex("dbo.LiveMatches", "BetServiceMatchNo");
            AddForeignKey("dbo.LiveMatches", "BetServiceMatchNo", "dbo.Matches", "BetServiceMatchNo");
        }
    }
}
