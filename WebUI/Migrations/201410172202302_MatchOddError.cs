namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MatchOddError : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MatchOdds", "Line");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MatchOdds", "Line", c => c.String());
        }
    }
}
