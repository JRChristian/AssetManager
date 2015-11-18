namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Asset2 : DbMigration
    {
        public override void Up()
        {
            AlterTableAnnotations(
                "dbo.Asset",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        CreationTime = c.DateTime(nullable: false),
                        AssetTypeId = c.Long(),
                        TenantId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Asset_MustHaveTenant",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AddColumn("dbo.Asset", "TenantId", c => c.Int(nullable: false));
            CreateIndex("dbo.Asset", "TenantId");
            AddForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants");
            DropIndex("dbo.Asset", new[] { "TenantId" });
            DropColumn("dbo.Asset", "TenantId");
            AlterTableAnnotations(
                "dbo.Asset",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        CreationTime = c.DateTime(nullable: false),
                        AssetTypeId = c.Long(),
                        TenantId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Asset_MustHaveTenant",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
        }
    }
}
