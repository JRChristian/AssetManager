namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagDataRaw : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagDataRaw",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        TagId = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                        Quality = c.Int(nullable: false),
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
                    { "DynamicFilter_TagDataRaw_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_TagDataRaw_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagDataRaw", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.TagDataRaw", "TagId", "dbo.Tag");
            DropIndex("dbo.TagDataRaw", new[] { "TagId" });
            DropIndex("dbo.TagDataRaw", new[] { "TenantId" });
            DropTable("dbo.TagDataRaw",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TagDataRaw_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_TagDataRaw_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
