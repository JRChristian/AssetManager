namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedIOWStatsByDay : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.IOWStatsByDay", new[] { "TenantId" });
            RenameIndex(table: "dbo.IOWStatsByDay", name: "IX_IOWLimitId", newName: "IOWLimitId");
            AddColumn("dbo.IOWStatsByDay", "NumberDeviations", c => c.Long(nullable: false));
            AddColumn("dbo.IOWStatsByDay", "DurationHours", c => c.Double(nullable: false));
            CreateIndex("dbo.IOWStatsByDay", "TenantId");
            CreateIndex("dbo.IOWStatsByDay", new[] { "TenantId", "IOWLimitId" });
            CreateIndex("dbo.IOWStatsByDay", new[] { "TenantId", "IOWLimitId", "Day" });
            DropColumn("dbo.IOWStatsByDay", "Status");
            DropColumn("dbo.IOWStatsByDay", "DeviationCount");
            DropColumn("dbo.IOWStatsByDay", "DeviationDuration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IOWStatsByDay", "DeviationDuration", c => c.Double(nullable: false));
            AddColumn("dbo.IOWStatsByDay", "DeviationCount", c => c.Long(nullable: false));
            AddColumn("dbo.IOWStatsByDay", "Status", c => c.Int(nullable: false));
            DropIndex("dbo.IOWStatsByDay", new[] { "TenantId", "IOWLimitId", "Day" });
            DropIndex("dbo.IOWStatsByDay", new[] { "TenantId", "IOWLimitId" });
            DropIndex("dbo.IOWStatsByDay", new[] { "TenantId" });
            DropColumn("dbo.IOWStatsByDay", "DurationHours");
            DropColumn("dbo.IOWStatsByDay", "NumberDeviations");
            RenameIndex(table: "dbo.IOWStatsByDay", name: "IOWLimitId", newName: "IX_IOWLimitId");
            CreateIndex("dbo.IOWStatsByDay", "TenantId");
        }
    }
}
