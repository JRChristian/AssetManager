namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedComputedColumnsFromDeviationTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.IOWDeviation", "Status");
            DropColumn("dbo.IOWDeviation", "DurationHours");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IOWDeviation", "DurationHours", c => c.Double(nullable: false));
            AddColumn("dbo.IOWDeviation", "Status", c => c.Int(nullable: false));
        }
    }
}
