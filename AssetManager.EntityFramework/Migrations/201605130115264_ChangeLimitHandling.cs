namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLimitHandling : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.IOWLimit", new[] { "TenantId" });
            DropIndex("dbo.IOWLimit", new[] { "IOWVariableId" });
            DropIndex("dbo.IOWLimit", new[] { "IOWLevelId" });
            AddColumn("dbo.IOWDeviation", "Direction", c => c.Int(nullable: false));
            AddColumn("dbo.IOWDeviation", "LimitValue", c => c.Double(nullable: false));
            AddColumn("dbo.IOWLimit", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.IOWLimit", "EndDate", c => c.DateTime());
            AddColumn("dbo.IOWLimit", "Direction", c => c.Int(nullable: false));
            AddColumn("dbo.IOWLimit", "Value", c => c.Double(nullable: false));
            AlterColumn("dbo.IOWDeviation", "WorstValue", c => c.Double(nullable: false));
            CreateIndex("dbo.IOWLimit", "TenantId");
            CreateIndex("dbo.IOWLimit", new[] { "TenantId", "IOWVariableId" });
            CreateIndex("dbo.IOWLimit", new[] { "TenantId", "IOWVariableId", "IOWLevelId" });
            CreateIndex("dbo.IOWLimit", "IOWVariableId");
            DropColumn("dbo.IOWDeviation", "LowLimit");
            DropColumn("dbo.IOWDeviation", "HighLimit");
            DropColumn("dbo.IOWLimit", "LowLimit");
            DropColumn("dbo.IOWLimit", "HighLimit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IOWLimit", "HighLimit", c => c.Double());
            AddColumn("dbo.IOWLimit", "LowLimit", c => c.Double());
            AddColumn("dbo.IOWDeviation", "HighLimit", c => c.Double());
            AddColumn("dbo.IOWDeviation", "LowLimit", c => c.Double());
            DropIndex("dbo.IOWLimit", new[] { "IOWVariableId" });
            DropIndex("dbo.IOWLimit", new[] { "TenantId", "IOWVariableId", "IOWLevelId" });
            DropIndex("dbo.IOWLimit", new[] { "TenantId", "IOWVariableId" });
            DropIndex("dbo.IOWLimit", new[] { "TenantId" });
            AlterColumn("dbo.IOWDeviation", "WorstValue", c => c.Double());
            DropColumn("dbo.IOWLimit", "Value");
            DropColumn("dbo.IOWLimit", "Direction");
            DropColumn("dbo.IOWLimit", "EndDate");
            DropColumn("dbo.IOWLimit", "StartDate");
            DropColumn("dbo.IOWDeviation", "LimitValue");
            DropColumn("dbo.IOWDeviation", "Direction");
            CreateIndex("dbo.IOWLimit", "IOWLevelId");
            CreateIndex("dbo.IOWLimit", "IOWVariableId");
            CreateIndex("dbo.IOWLimit", "TenantId");
        }
    }
}
