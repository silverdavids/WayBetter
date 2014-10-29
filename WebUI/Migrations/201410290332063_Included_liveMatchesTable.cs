namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Included_liveMatchesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiveMatches",
                c => new
                    {
                        LiveMatchId = c.Long(nullable: false, identity: true),
                        ShortMatchCode = c.Int(nullable: false),
                        Match_BetServiceMatchNo = c.Int(),
                    })
                .PrimaryKey(t => t.LiveMatchId)
                .ForeignKey("dbo.Matches", t => t.Match_BetServiceMatchNo)
                .Index(t => t.Match_BetServiceMatchNo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiveMatches", "Match_BetServiceMatchNo", "dbo.Matches");
            DropIndex("dbo.LiveMatches", new[] { "Match_BetServiceMatchNo" });
            DropTable("dbo.LiveMatches");
        }
    }
}
