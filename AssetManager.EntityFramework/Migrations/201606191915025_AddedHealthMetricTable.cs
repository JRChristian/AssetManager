namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHealthMetricTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HealthMetric",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        AssetTypeId = c.Long(nullable: false),
                        IOWLevelId = c.Long(nullable: false),
                        Period = c.Int(nullable: false),
                        MetricType = c.Int(nullable: false),
                        GoodDirection = c.Int(nullable: false),
                        WarningLevel = c.Double(nullable: false),
                        ErrorLevel = c.Double(nullable: false),
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
                    { "DynamicFilter_HealthMetric_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_HealthMetric_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetType", t => t.AssetTypeId)
                .ForeignKey("dbo.IOWLevel", t => t.IOWLevelId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.IOWLevelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HealthMetric", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.HealthMetric", "IOWLevelId", "dbo.IOWLevel");
            DropForeignKey("dbo.HealthMetric", "AssetTypeId", "dbo.AssetType");
            DropIndex("dbo.HealthMetric", new[] { "IOWLevelId" });
            DropIndex("dbo.HealthMetric", new[] { "AssetTypeId" });
            DropIndex("dbo.HealthMetric", new[] { "TenantId" });
            DropTable("dbo.HealthMetric",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_HealthMetric_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_HealthMetric_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
