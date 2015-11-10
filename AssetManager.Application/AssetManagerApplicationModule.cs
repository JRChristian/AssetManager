using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace AssetManager
{
    [DependsOn(typeof(AssetManagerCoreModule), typeof(AbpAutoMapperModule))]
    public class AssetManagerApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
