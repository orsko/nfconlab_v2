namespace nfconlab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial5 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Players");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerId = c.Int(nullable: false, identity: true),
                        User_ID = c.String(),
                        Answered = c.String(),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerId);
            
        }
    }
}
