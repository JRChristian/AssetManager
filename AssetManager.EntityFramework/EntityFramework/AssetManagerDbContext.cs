using System.Data.Common;
using Abp.Zero.EntityFramework;
using AssetManager.Authorization.Roles;
using AssetManager.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.EntityFramework
{
    public class AssetManagerDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

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

        //This constructor is used in tests
        public AssetManagerDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
