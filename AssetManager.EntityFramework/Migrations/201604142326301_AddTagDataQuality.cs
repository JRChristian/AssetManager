namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagDataQuality : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TagData", newName: "TagDataRaw");
            AlterTableAnnotations(
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
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_TagData_MustHaveTenant",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                    { 
                        "DynamicFilter_TagData_SoftDelete",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                    { 
                        "DynamicFilter_TagDataRaw_MustHaveTenant",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                    { 
                        "DynamicFilter_TagDataRaw_SoftDelete",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AddColumn("dbo.TagDataRaw", "Timestamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.TagDataRaw", "Quality", c => c.Int(nullable: false));
            DropColumn("dbo.TagDataRaw", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TagDataRaw", "Date", c => c.DateTime(nullable: false));
            DropColumn("dbo.TagDataRaw", "Quality");
            DropColumn("dbo.TagDataRaw", "Timestamp");
            AlterTableAnnotations(
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
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_TagData_MustHaveTenant",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                    { 
                        "DynamicFilter_TagData_SoftDelete",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                    { 
                        "DynamicFilter_TagDataRaw_MustHaveTenant",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                    { 
                        "DynamicFilter_TagDataRaw_SoftDelete",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
            RenameTable(name: "dbo.TagDataRaw", newName: "TagData");
        }
    }
}
