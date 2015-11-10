using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using AssetManager.EntityFramework;

namespace AssetManager
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(AssetManagerCoreModule))]
    public class AssetManagerDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
