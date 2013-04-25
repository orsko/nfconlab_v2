namespace nfconlab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerItems",
                c => new
                    {
                        PlayerItemId = c.Int(nullable: false, identity: true),
                        User_ID = c.String(),
                        Answered = c.String(),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerItemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlayerItems");
        }
    }
}
