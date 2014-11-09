namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_BetMinute_To_Bets_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "BetMinute", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bets", "BetMinute");
        }
    }
}
