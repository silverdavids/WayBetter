namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_LiveMatchResultsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiveResults",
                c => new
                    {
                        LiveResultId = c.Int(nullable: false, identity: true),
                        ResultMinute = c.String(),
                        HomeScore = c.String(),
                        AwayScore = c.String(),
                        TimeUpdated = c.DateTime(nullable: false),
                        BetServiceMatchNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LiveResultId)
                .ForeignKey("dbo.Matches", t => t.BetServiceMatchNo, cascadeDelete: true)
                .Index(t => t.BetServiceMatchNo);
            
            AddColumn("dbo.LiveMatches", "DailyShortCode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiveResults", "BetServiceMatchNo", "dbo.Matches");
            DropIndex("dbo.LiveResults", new[] { "BetServiceMatchNo" });
            DropColumn("dbo.LiveMatches", "DailyShortCode");
            DropTable("dbo.LiveResults");
        }
    }
}
