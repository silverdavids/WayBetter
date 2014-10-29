namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_SetDate_Field_ToLiveMatch_Entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LiveMatches", "SetDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LiveMatches", "SetDate");
        }
    }
}
