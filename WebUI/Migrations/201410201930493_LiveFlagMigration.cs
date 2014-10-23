namespace WebUI.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LiveFlagMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShortMatchCodes", "MatchNo", "dbo.Matches");
            DropPrimaryKey("dbo.ShortMatchCodes");
            AddColumn("dbo.Matches", "LiveBetFlag", c => c.Boolean(nullable: false));
            AddPrimaryKey("dbo.ShortMatchCodes", new[] { "MatchNo", "SetNo", "ShortCode" });
            AddForeignKey("dbo.ShortMatchCodes", "MatchNo", "dbo.Matches", "BetServiceMatchNo", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShortMatchCodes", "MatchNo", "dbo.Matches");
            DropPrimaryKey("dbo.ShortMatchCodes");
            DropColumn("dbo.Matches", "LiveBetFlag");
            AddPrimaryKey("dbo.ShortMatchCodes", "MatchNo");
            AddForeignKey("dbo.ShortMatchCodes", "MatchNo", "dbo.Matches", "BetServiceMatchNo");
        }
    }
}
