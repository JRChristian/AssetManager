namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddComputedColumnsToDeviationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IOWDeviation", "DurationHours", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IOWDeviation", "DurationHours");
        }
    }
}
