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

            //We must declare mappings to be able to use AutoMapper
            DtoMappings.Map();
        }
    }
}
