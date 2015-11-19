using AssetManager.Entities;
using AssetManager.EntityFramework;
using AssetManager.Migrations.SeedData;
using EntityFramework.DynamicFilters;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

//See http://stackoverflow.com/questions/13510341/entity-framework-5-migrations-setting-up-an-initial-migration-and-single-seed-o
//for thoughts about when the Seed() method runs and how to control it.

namespace AssetManager.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AssetManager.EntityFramework.AssetManagerDbContext>
    {
        private readonly bool _pendingMigrations;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "AssetManager";
            var migrator = new DbMigrator(this);
            _pendingMigrations = migrator.GetPendingMigrations().Any();
        }

        protected override void Seed(AssetManager.EntityFramework.AssetManagerDbContext context)
        {
            //Uncomment this line to exit if there aren't any pending migrations
            //if (!_pendingMigrations) return;

            context.DisableAllFilters();
            new InitialDataBuilder(context).Build();

            AddAssetConfiguation(context);
        }

        // Configuration for assets and asset types
        bool AddAssetConfiguation(AssetManager.EntityFramework.AssetManagerDbContext context)
        {
            // Asset types
            bool r;
            r = AddAssetType(context, "Facility");
            r = AddAssetType(context, "Fixed");
            r = AddAssetType(context, "Logical");
            r = AddAssetType(context, "Other");
            r = AddAssetType(context, "Structural");
            r = AddAssetType(context, "Temporary");
            r = AddAssetType(context, "Unit Ops");

            r = AddAsset(context, "Westlake Refinery", "Westlake Refinery", "Facility");
            r = AddAsset(context, "CDU", "Crude Distillation Unit", "Unit Ops");
            r = AddAsset(context, "Reformer", "Catalytic Reforming Unit", "Unit Ops");
            r = AddAsset(context, "FCCU", "Fluid Catalytic Cracking Unit", "Unit Ops");
            return true;
        }

        // Helper function to add or update an Asset record. All records are assigned to the default tenant
        bool AddAsset(AssetManager.EntityFramework.AssetManagerDbContext context, string AssetName, string AssetDesc, string AssetTypeName )
        {
            long AssetTypeId = context.AssetTypes.Where(x => x.Name == AssetTypeName).SingleOrDefault().Id;
            if (AssetTypeId != 0)
            {
                context.Assets.AddOrUpdate(x => x.Name, new Asset { Name = AssetName, Description = AssetDesc, AssetTypeId = AssetTypeId, TenantId = 1 });
                context.SaveChanges();
                return true;
            }
             return false;
        }

        // Helper function to add or update an AssetType record
        bool AddAssetType(AssetManager.EntityFramework.AssetManagerDbContext context, string AssetTypeName)
        {
            context.AssetTypes.AddOrUpdate(x => x.Name, new AssetType { Name = AssetTypeName });
            context.SaveChanges();
            return true;
        }
    }
}
