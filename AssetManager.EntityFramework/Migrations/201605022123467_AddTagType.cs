namespace AssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tag", "Type", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tag", "Type");
        }
    }
}
