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
        }
    }
}
