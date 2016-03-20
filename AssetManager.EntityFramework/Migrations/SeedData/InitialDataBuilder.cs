using AssetManager.EntityFramework;

namespace AssetManager.Migrations.SeedData
{
    public class InitialDataBuilder
    {
        private readonly AssetManagerDbContext _context;

        public InitialDataBuilder(AssetManagerDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            new DefaultTenantRoleAndUserBuilder(_context).Build();
            new DefaultAssetBuilder(_context).Build();
        }
    }
}
