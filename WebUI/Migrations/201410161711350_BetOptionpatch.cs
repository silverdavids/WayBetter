namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BetOptionpatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatchOdds", "Line", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MatchOdds", "Line");
        }
    }
}
