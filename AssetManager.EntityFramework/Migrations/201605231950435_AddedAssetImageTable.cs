namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssetImageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetImage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                        ImageId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AssetImage_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetImage_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Asset", t => t.AssetId)
                .ForeignKey("dbo.Image", t => t.ImageId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.AssetId)
                .Index(t => t.ImageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetImage", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.AssetImage", "ImageId", "dbo.Image");
            DropForeignKey("dbo.AssetImage", "AssetId", "dbo.Asset");
            DropIndex("dbo.AssetImage", new[] { "ImageId" });
            DropIndex("dbo.AssetImage", new[] { "AssetId" });
            DropIndex("dbo.AssetImage", new[] { "TenantId" });
            DropTable("dbo.AssetImage",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AssetImage_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetImage_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
