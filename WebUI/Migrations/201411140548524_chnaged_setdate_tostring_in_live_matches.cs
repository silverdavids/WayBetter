namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chnaged_setdate_tostring_in_live_matches : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LiveMatches", "SetDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LiveMatches", "SetDate", c => c.DateTime(nullable: false));
        }
    }
}
