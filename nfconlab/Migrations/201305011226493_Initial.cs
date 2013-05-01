namespace nfconlab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionToPlayerItems",
                c => new
                    {
                        QuestionToPlayerItemId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.QuestionToPlayerItemId);
            
            AddColumn("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", c => c.Int());
            AddColumn("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", c => c.Int());
            AddForeignKey("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems", "QuestionToPlayerItemId");
            AddForeignKey("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems", "QuestionToPlayerItemId");
            CreateIndex("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            CreateIndex("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlayerItems", new[] { "QuestionToPlayerItem_QuestionToPlayerItemId" });
            DropIndex("dbo.QuestionItems", new[] { "QuestionToPlayerItem_QuestionToPlayerItemId" });
            DropForeignKey("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems");
            DropForeignKey("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems");
            DropColumn("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            DropColumn("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            DropTable("dbo.QuestionToPlayerItems");
        }
    }
}
