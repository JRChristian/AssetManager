namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedHealthMetricTableAgain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HealthMetric", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HealthMetric", "Order");
        }
    }
}
