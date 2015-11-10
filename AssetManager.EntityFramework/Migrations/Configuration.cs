using System.Data.Entity.Migrations;
using AssetManager.Migrations.SeedData;
using EntityFramework.DynamicFilters;

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
        }
    }
}
