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
                        BetCategoryId = c.Int(nullable: false, identity: true),
                        BetCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.BetCategoryId);
            
            CreateTable(
                "dbo.BetOptions",
                c => new
                    {
                        BetOptionId = c.Int(nullable: false, identity: true),
                        BetCategoryId = c.Int(nullable: false),
                        Option = c.String(),
                        Line = c.String(),
                    })
                .PrimaryKey(t => t.BetOptionId)
                .ForeignKey("dbo.BetCategories", t => t.BetCategoryId, cascadeDelete: true)
                .Index(t => t.BetCategoryId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        BetServiceMatchNo = c.Int(nullable: false),
                        SetNo = c.Int(nullable: false),
                        ShortMatchCode = c.Int(nullable: false),
                        League = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        GameStatus = c.String(),
                        AwayTeamName = c.String(),
                        HomeTeamName = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        HomeScore = c.Int(nullable: false),
                        AwayScore = c.Int(nullable: false),
                        HalfTimeHomeScore = c.Int(nullable: false),
                        HalfTimeAwayScore = c.Int(nullable: false),
                        ResultStatus = c.Int(),
                    })
                .PrimaryKey(t => t.BetServiceMatchNo);
            
            CreateTable(
                "dbo.MatchOdds",
                c => new
                    {
                        MatchOddId = c.Int(nullable: false, identity: true),
                        BetServiceMatchNo = c.Int(nullable: false),
                        SetNo = c.Int(nullable: false),
                        ShortMatchCode = c.Int(nullable: false),
                        BetOptionId = c.Int(nullable: false),
                        Odd = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HandicapGoals = c.Int(),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.MatchOddId)
                .ForeignKey("dbo.BetOptions", t => t.BetOptionId, cascadeDelete: true)
                .ForeignKey("dbo.Matches", t => t.BetServiceMatchNo, cascadeDelete: true)
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
