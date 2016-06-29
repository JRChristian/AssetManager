namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetricsToLevelTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IOWLevel", "MetricType", c => c.Int(nullable: false));
            AddColumn("dbo.IOWLevel", "GoodDirection", c => c.Int(nullable: false));
            AddColumn("dbo.IOWLevel", "WarningLevel", c => c.Double(nullable: false));
            AddColumn("dbo.IOWLevel", "ErrorLevel", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IOWLevel", "ErrorLevel");
            DropColumn("dbo.IOWLevel", "WarningLevel");
            DropColumn("dbo.IOWLevel", "GoodDirection");
            DropColumn("dbo.IOWLevel", "MetricType");
        }
    }
}
