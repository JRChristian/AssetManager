namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTagTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tag", "LastTimestamp", c => c.DateTime());
            AddColumn("dbo.Tag", "LastValue", c => c.Double());
            AddColumn("dbo.Tag", "LastQuality", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tag", "LastQuality");
            DropColumn("dbo.Tag", "LastValue");
            DropColumn("dbo.Tag", "LastTimestamp");
        }
    }
}
