namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssetHierarchy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetHierarchy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                        ParentAssetHierarchyId = c.Long(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        Parent_Id = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AssetHierarchy_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetHierarchy_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Asset", t => t.AssetId)
                .ForeignKey("dbo.AssetHierarchy", t => t.Parent_Id)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.AssetId)
                .Index(t => t.Parent_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetHierarchy", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.AssetHierarchy", "Parent_Id", "dbo.AssetHierarchy");
            DropForeignKey("dbo.AssetHierarchy", "AssetId", "dbo.Asset");
            DropIndex("dbo.AssetHierarchy", new[] { "Parent_Id" });
            DropIndex("dbo.AssetHierarchy", new[] { "AssetId" });
            DropIndex("dbo.AssetHierarchy", new[] { "TenantId" });
            DropTable("dbo.AssetHierarchy",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AssetHierarchy_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetHierarchy_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
