namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDeviationHandling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IOWDeviation", "StartTimestamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.IOWDeviation", "EndTimestamp", c => c.DateTime());
            AddColumn("dbo.IOWDeviation", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.IOWLimit", "LastDeviationStartTimestamp", c => c.DateTime());
            AddColumn("dbo.IOWLimit", "LastDeviationEndTimestamp", c => c.DateTime());
            DropColumn("dbo.IOWDeviation", "StartDate");
            DropColumn("dbo.IOWDeviation", "EndDate");
            DropColumn("dbo.IOWLimit", "LastDeviationStartDate");
            DropColumn("dbo.IOWLimit", "LastDeviationEndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IOWLimit", "LastDeviationEndDate", c => c.DateTime());
            AddColumn("dbo.IOWLimit", "LastDeviationStartDate", c => c.DateTime());
            AddColumn("dbo.IOWDeviation", "EndDate", c => c.DateTime());
            AddColumn("dbo.IOWDeviation", "StartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.IOWLimit", "LastDeviationEndTimestamp");
            DropColumn("dbo.IOWLimit", "LastDeviationStartTimestamp");
            DropColumn("dbo.IOWDeviation", "Status");
            DropColumn("dbo.IOWDeviation", "EndTimestamp");
            DropColumn("dbo.IOWDeviation", "StartTimestamp");
        }
    }
}
