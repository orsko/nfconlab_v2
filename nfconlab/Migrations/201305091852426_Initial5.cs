namespace nfconlab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlayerItems", "User_ID", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlayerItems", "User_ID", c => c.String());
        }
    }
}
