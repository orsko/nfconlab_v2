namespace nfconlab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems");
            DropForeignKey("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems");
            DropIndex("dbo.QuestionItems", new[] { "QuestionToPlayerItem_QuestionToPlayerItemId" });
            DropIndex("dbo.PlayerItems", new[] { "QuestionToPlayerItem_QuestionToPlayerItemId" });
            CreateTable(
                "dbo.PlayerItemQuestionItems",
                c => new
                    {
                        PlayerItem_PlayerItemId = c.Int(nullable: false),
                        QuestionItem_QuestionItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PlayerItem_PlayerItemId, t.QuestionItem_QuestionItemId })
                .ForeignKey("dbo.PlayerItems", t => t.PlayerItem_PlayerItemId, cascadeDelete: true)
                .ForeignKey("dbo.QuestionItems", t => t.QuestionItem_QuestionItemId, cascadeDelete: true)
                .Index(t => t.PlayerItem_PlayerItemId)
                .Index(t => t.QuestionItem_QuestionItemId);
            
            DropColumn("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            DropColumn("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            DropTable("dbo.QuestionToPlayerItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QuestionToPlayerItems",
                c => new
                    {
                        QuestionToPlayerItemId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.QuestionToPlayerItemId);
            
            AddColumn("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", c => c.Int());
            AddColumn("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", c => c.Int());
            DropIndex("dbo.PlayerItemQuestionItems", new[] { "QuestionItem_QuestionItemId" });
            DropIndex("dbo.PlayerItemQuestionItems", new[] { "PlayerItem_PlayerItemId" });
            DropForeignKey("dbo.PlayerItemQuestionItems", "QuestionItem_QuestionItemId", "dbo.QuestionItems");
            DropForeignKey("dbo.PlayerItemQuestionItems", "PlayerItem_PlayerItemId", "dbo.PlayerItems");
            DropTable("dbo.PlayerItemQuestionItems");
            CreateIndex("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            CreateIndex("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId");
            AddForeignKey("dbo.PlayerItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems", "QuestionToPlayerItemId");
            AddForeignKey("dbo.QuestionItems", "QuestionToPlayerItem_QuestionToPlayerItemId", "dbo.QuestionToPlayerItems", "QuestionToPlayerItemId");
        }
    }
}
