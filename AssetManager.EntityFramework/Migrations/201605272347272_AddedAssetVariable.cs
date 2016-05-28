namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssetVariable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetVariable",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                        IOWVariableId = c.Long(nullable: false),
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
                    { "DynamicFilter_AssetVariable_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetVariable_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Asset", t => t.AssetId)
                .ForeignKey("dbo.IOWVariable", t => t.IOWVariableId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.AssetId)
                .Index(t => t.IOWVariableId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetVariable", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.AssetVariable", "IOWVariableId", "dbo.IOWVariable");
            DropForeignKey("dbo.AssetVariable", "AssetId", "dbo.Asset");
            DropIndex("dbo.AssetVariable", new[] { "IOWVariableId" });
            DropIndex("dbo.AssetVariable", new[] { "AssetId" });
            DropIndex("dbo.AssetVariable", new[] { "TenantId" });
            DropTable("dbo.AssetVariable",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AssetVariable_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_AssetVariable_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
