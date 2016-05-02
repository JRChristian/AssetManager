namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateNewIndexOnTagData : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TagDataRaw", new[] { "TenantId" });
            DropIndex("dbo.TagDataRaw", new[] { "TagId" });
            CreateIndex("dbo.TagDataRaw", "TenantId");
            CreateIndex("dbo.TagDataRaw", new[] { "TenantId", "TagId" });
            CreateIndex("dbo.TagDataRaw", new[] { "TenantId", "TagId", "Timestamp" });
            CreateIndex("dbo.TagDataRaw", "TagId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TagDataRaw", new[] { "TagId" });
            DropIndex("dbo.TagDataRaw", new[] { "TenantId", "TagId", "Timestamp" });
            DropIndex("dbo.TagDataRaw", new[] { "TenantId", "TagId" });
            DropIndex("dbo.TagDataRaw", new[] { "TenantId" });
            CreateIndex("dbo.TagDataRaw", "TagId");
            CreateIndex("dbo.TagDataRaw", "TenantId");
        }
    }
}
