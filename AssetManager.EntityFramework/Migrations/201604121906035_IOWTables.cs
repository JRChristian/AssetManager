namespace AssetManager.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class IOWTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.AbpUserLogins", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.AbpPermissions", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.AbpUserRoles", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.AbpFeatures", "EditionId", "dbo.AbpEditions");
            DropForeignKey("dbo.AbpPermissions", "RoleId", "dbo.AbpRoles");
            CreateTable(
                "dbo.IOWDeviation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        IOWLimitId = c.Long(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        LowLimit = c.Double(),
                        HighLimit = c.Double(),
                        WorstValue = c.Double(),
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
                    { "DynamicFilter_IOWDeviation_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWDeviation_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IOWLimit", t => t.IOWLimitId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.IOWLimitId);
            
            CreateTable(
                "dbo.IOWLimit",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        IOWVariableId = c.Long(nullable: false),
                        IOWLevelId = c.Long(nullable: false),
                        Cause = c.String(maxLength: 255),
                        Consequences = c.String(maxLength: 255),
                        Action = c.String(maxLength: 255),
                        LowLimit = c.Double(),
                        HighLimit = c.Double(),
                        LastCheckDate = c.DateTime(),
                        LastStatus = c.Int(),
                        LastDeviationStartDate = c.DateTime(),
                        LastDeviationEndDate = c.DateTime(),
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
                    { "DynamicFilter_IOWLimit_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWLimit_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IOWLevel", t => t.IOWLevelId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .ForeignKey("dbo.IOWVariable", t => t.IOWVariableId)
                .Index(t => t.TenantId)
                .Index(t => t.IOWVariableId)
                .Index(t => t.IOWLevelId);
            
            CreateTable(
                "dbo.IOWStatsByDay",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        IOWLimitId = c.Long(nullable: false),
                        Day = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        DeviationCount = c.Long(nullable: false),
                        DeviationDuration = c.Double(nullable: false),
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
                    { "DynamicFilter_IOWStatsByDay_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWStatsByDay_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IOWLimit", t => t.IOWLimitId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.IOWLimitId);
            
            CreateTable(
                "dbo.IOWLevel",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        Criticality = c.Int(nullable: false),
                        ResponseGoal = c.String(nullable: false, maxLength: 255),
                        MetricGoal = c.String(nullable: false, maxLength: 255),
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
                    { "DynamicFilter_IOWLevel_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWLevel_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.IOWVariable",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        TagId = c.Long(nullable: false),
                        UOM = c.String(maxLength: 40),
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
                    { "DynamicFilter_IOWVariable_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWVariable_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        Description = c.String(nullable: false, maxLength: 255),
                        UOM = c.String(maxLength: 40),
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
                    { "DynamicFilter_Tag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TagData",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        TagId = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
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
                    { "DynamicFilter_TagData_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_TagData_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.AbpTenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.TagId);
            
            AddForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants", "Id");
            AddForeignKey("dbo.AbpUserLogins", "UserId", "dbo.AbpUsers", "Id");
            AddForeignKey("dbo.AbpPermissions", "UserId", "dbo.AbpUsers", "Id");
            AddForeignKey("dbo.AbpUserRoles", "UserId", "dbo.AbpUsers", "Id");
            AddForeignKey("dbo.AbpFeatures", "EditionId", "dbo.AbpEditions", "Id");
            AddForeignKey("dbo.AbpPermissions", "RoleId", "dbo.AbpRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AbpPermissions", "RoleId", "dbo.AbpRoles");
            DropForeignKey("dbo.AbpFeatures", "EditionId", "dbo.AbpEditions");
            DropForeignKey("dbo.AbpUserRoles", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.AbpPermissions", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.AbpUserLogins", "UserId", "dbo.AbpUsers");
            DropForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.IOWDeviation", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.IOWVariable", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.Tag", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.TagData", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.TagData", "TagId", "dbo.Tag");
            DropForeignKey("dbo.IOWVariable", "TagId", "dbo.Tag");
            DropForeignKey("dbo.IOWLimit", "IOWVariableId", "dbo.IOWVariable");
            DropForeignKey("dbo.IOWLimit", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.IOWLevel", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.IOWLimit", "IOWLevelId", "dbo.IOWLevel");
            DropForeignKey("dbo.IOWStatsByDay", "TenantId", "dbo.AbpTenants");
            DropForeignKey("dbo.IOWStatsByDay", "IOWLimitId", "dbo.IOWLimit");
            DropForeignKey("dbo.IOWDeviation", "IOWLimitId", "dbo.IOWLimit");
            DropIndex("dbo.TagData", new[] { "TagId" });
            DropIndex("dbo.TagData", new[] { "TenantId" });
            DropIndex("dbo.Tag", new[] { "TenantId" });
            DropIndex("dbo.IOWVariable", new[] { "TagId" });
            DropIndex("dbo.IOWVariable", new[] { "TenantId" });
            DropIndex("dbo.IOWLevel", new[] { "TenantId" });
            DropIndex("dbo.IOWStatsByDay", new[] { "IOWLimitId" });
            DropIndex("dbo.IOWStatsByDay", new[] { "TenantId" });
            DropIndex("dbo.IOWLimit", new[] { "IOWLevelId" });
            DropIndex("dbo.IOWLimit", new[] { "IOWVariableId" });
            DropIndex("dbo.IOWLimit", new[] { "TenantId" });
            DropIndex("dbo.IOWDeviation", new[] { "IOWLimitId" });
            DropIndex("dbo.IOWDeviation", new[] { "TenantId" });
            DropTable("dbo.TagData",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TagData_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_TagData_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Tag",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.IOWVariable",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_IOWVariable_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWVariable_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.IOWLevel",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_IOWLevel_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWLevel_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.IOWStatsByDay",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_IOWStatsByDay_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWStatsByDay_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.IOWLimit",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_IOWLimit_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWLimit_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.IOWDeviation",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_IOWDeviation_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_IOWDeviation_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            AddForeignKey("dbo.AbpPermissions", "RoleId", "dbo.AbpRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AbpFeatures", "EditionId", "dbo.AbpEditions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AbpUserRoles", "UserId", "dbo.AbpUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AbpPermissions", "UserId", "dbo.AbpUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AbpUserLogins", "UserId", "dbo.AbpUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Asset", "TenantId", "dbo.AbpTenants", "Id", cascadeDelete: true);
        }
    }
}
