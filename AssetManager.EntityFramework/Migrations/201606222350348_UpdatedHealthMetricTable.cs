namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedHealthMetricTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HealthMetric", "ApplyToEachAsset", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HealthMetric", "ApplyToEachAsset");
        }
    }
}
