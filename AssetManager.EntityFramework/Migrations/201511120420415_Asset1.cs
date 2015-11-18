namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Asset1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Asset",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        CreationTime = c.DateTime(nullable: false),
                        AssetTypeId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetType", t => t.AssetTypeId)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.AssetType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Asset", "AssetTypeId", "dbo.AssetType");
            DropIndex("dbo.Asset", new[] { "AssetTypeId" });
            DropTable("dbo.AssetType");
            DropTable("dbo.Asset");
        }
    }
}
