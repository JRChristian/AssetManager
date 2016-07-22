namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaterialsToAssetTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Asset", "Materials", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Asset", "Materials");
        }
    }
}
