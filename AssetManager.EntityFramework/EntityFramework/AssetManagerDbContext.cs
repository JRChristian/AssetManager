using Abp.Zero.EntityFramework;
using AssetManager.Authorization.Roles;
using AssetManager.Configurations;
using AssetManager.Entities;
using AssetManager.MultiTenancy;
using AssetManager.Users;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AssetManager.EntityFramework
{
    public class AssetManagerDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        #region Entity Sets
        public IDbSet<Asset> Assets { get; set; }
        public IDbSet<AssetType> AssetTypes { get; set; }
        #endregion

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public AssetManagerDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in AssetManagerDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of AssetManagerDbContext since ABP automatically handles it.
         */
        public AssetManagerDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new AssetConfiguration());
            modelBuilder.Configurations.Add(new AssetHierarchyConfiguration());
            modelBuilder.Configurations.Add(new AssetImageConfiguration());
            modelBuilder.Configurations.Add(new AssetTypeConfiguration());
            modelBuilder.Configurations.Add(new ImageConfiguration());
            modelBuilder.Configurations.Add(new IOWDeviationConfiguration());
            modelBuilder.Configurations.Add(new IOWLevelConfiguration());
            modelBuilder.Configurations.Add(new IOWLimitConfiguration());
            modelBuilder.Configurations.Add(new IOWStatsByDayConfiguration());
            modelBuilder.Configurations.Add(new IOWVariableConfiguration());
            modelBuilder.Configurations.Add(new TagConfiguration());
            modelBuilder.Configurations.Add(new TagDataRawConfiguration());
            modelBuilder.Configurations.Add(new TagDataRawUpdateConfiguration());
        }

        //This constructor is used in tests
        public AssetManagerDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
