using AssetManager.Entities;
using AssetManager.EntityFramework;
using AssetManager.Migrations.SeedData;
using EntityFramework.DynamicFilters;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace AssetManager.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AssetManager.EntityFramework.AssetManagerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AssetManager";
        }

        protected override void Seed(AssetManager.EntityFramework.AssetManagerDbContext context)
        {
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

            /*
            var asset_types = new List<AssetType>
            {
                new AssetType {Name="Facility"},
                new AssetType {Name="Fixed"},
                new AssetType {Name="Logical"},
                new AssetType {Name="Other"},
                new AssetType {Name="Structural"},
                new AssetType {Name="Temporary"},
                new AssetType {Name="Unit Ops"}
            };
            asset_types.ForEach(x => context.AssetTypes.AddOrUpdate(y => y.Name, x));
            context.SaveChanges();

            // Assets
            long asset_type_id;

            asset_type_id = context.AssetTypes.Where(x => x.Name == "Facility").Single().Id;
            var assets = new List<Asset>
            {
                new Asset {Name="Westlake Refinery", Description="Westlake Refinery", AssetTypeId=asset_type_id, TenantId=1}
            };
            assets.ForEach(x => context.Assets.AddOrUpdate(y => y.Name, x));

            asset_type_id = context.AssetTypes.Where(x => x.Name == "Unit Ops").Single().Id;
            assets = new List<Asset>
            {
                new Asset {Name="CDU", Description="Crude Distillation Unit", AssetTypeId=asset_type_id, TenantId=1},
                new Asset {Name="Reformer", Description="Catalytic Reforming Unit", AssetTypeId=asset_type_id, TenantId=1}
            };
            assets.ForEach(x => context.Assets.AddOrUpdate(y => y.Name, x));
            context.SaveChanges();

            */
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
