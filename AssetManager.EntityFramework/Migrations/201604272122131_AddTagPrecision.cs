namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagPrecision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tag", "Precision", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tag", "Precision");
        }
    }
}
