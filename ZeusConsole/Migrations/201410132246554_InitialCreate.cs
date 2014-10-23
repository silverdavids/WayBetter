namespace ZeusConsole.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BetCategories",
                c => new
                    {
                        BetCategoryId = c.Int(false, true),
                        BetCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.BetCategoryId);
            
            CreateTable(
                "dbo.BetOptions",
                c => new
                    {
                        BetOptionId = c.Int(false, true),
                        BetCategoryId = c.Int(false),
                        Option = c.String(),
                        Line = c.String(),
                    })
                .PrimaryKey(t => t.BetOptionId)
                .ForeignKey("dbo.BetCategories", t => t.BetCategoryId, true)
                .Index(t => t.BetCategoryId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        BetServiceMatchNo = c.Int(false),
                        League = c.String(),
                        StartTime = c.DateTime(false),
                        GameStatus = c.String(),
                        AwayTeamName = c.String(),
                        HomeTeamName = c.String(),
                        RegistrationDate = c.DateTime(false),
                        HomeScore = c.Int(false),
                        AwayScore = c.Int(false),
                        HalfTimeHomeScore = c.Int(false),
                        HalfTimeAwayScore = c.Int(false),
                        ResultStatus = c.Int(),
                    })
                .PrimaryKey(t => t.BetServiceMatchNo);
            
            CreateTable(
                "dbo.MatchOdds",
                c => new
                    {
                        MatchOddId = c.Int(false, true),
                        SetNo = c.Int(false),
                        ShortMatchCode = c.Int(false),
                        BetServiceMatchNo = c.Int(false),
                        BetOptionId = c.Int(false),
                        Odd = c.Decimal(false, 18, 2),
                        HandicapGoals = c.Int(),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.MatchOddId, t.SetNo, t.ShortMatchCode })
                .ForeignKey("dbo.BetOptions", t => t.BetOptionId, true)
                .ForeignKey("dbo.Matches", t => t.BetServiceMatchNo, true)
                .Index(t => t.BetServiceMatchNo)
                .Index(t => t.BetOptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MatchOdds", "BetServiceMatchNo", "dbo.Matches");
            DropForeignKey("dbo.MatchOdds", "BetOptionId", "dbo.BetOptions");
            DropForeignKey("dbo.BetOptions", "BetCategoryId", "dbo.BetCategories");
            DropIndex("dbo.MatchOdds", new[] { "BetOptionId" });
            DropIndex("dbo.MatchOdds", new[] { "BetServiceMatchNo" });
            DropIndex("dbo.BetOptions", new[] { "BetCategoryId" });
            DropTable("dbo.MatchOdds");
            DropTable("dbo.Matches");
            DropTable("dbo.BetOptions");
            DropTable("dbo.BetCategories");
        }
    }
}
