using AssetManager.Entities;
using AssetManager.EntityFramework;
using System.Data.Entity.Migrations;
using System.Linq;

namespace AssetManager.Migrations.SeedData
{
    public class DefaultAssetBuilder
    {
        private readonly AssetManagerDbContext _context;

        public DefaultAssetBuilder(AssetManagerDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            CreateAssets();
        }

        private void CreateAssets()
        {
            // Asset types
            bool r;
            r = AddAssetType(_context, "Facility");
            r = AddAssetType(_context, "Fixed");
            r = AddAssetType(_context, "Logical");
            r = AddAssetType(_context, "Other");
            r = AddAssetType(_context, "Structural");
            r = AddAssetType(_context, "Temporary");
            r = AddAssetType(_context, "Unit Ops");

            r = AddAsset(_context, "Westlake Refinery", "Westlake Refinery", "Facility");
            r = AddAsset(_context, "CDU", "Crude Distillation Unit", "Unit Ops");
            r = AddAsset(_context, "Reformer", "Catalytic Reforming Unit", "Unit Ops");
            r = AddAsset(_context, "FCCU", "Fluid Catalytic Cracking Unit", "Unit Ops");
        }

        // Helper function to add or update an Asset record. All records are assigned to the default tenant
        bool AddAsset(AssetManager.EntityFramework.AssetManagerDbContext context, string AssetName, string AssetDesc, string AssetTypeName)
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
