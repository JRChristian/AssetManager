using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using AssetManager.Api;

namespace AssetManager.Web
{
    [DependsOn(typeof(AssetManagerDataModule), typeof(AssetManagerApplicationModule), typeof(AssetManagerWebApiModule))]
    public class AssetManagerWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Add/remove languages for your application
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English (US)", "famfamfam-flag-us", true));
            Configuration.Localization.Languages.Add(new LanguageInfo("ca", "English (Canadian)", "famfamfam-flag-ca"));
            Configuration.Localization.Languages.Add(new LanguageInfo("uk", "English (UK)", "famfamfam-flag-gb"));
            Configuration.Localization.Languages.Add(new LanguageInfo("gd", "Scots Gàidhlig", "famfamfam-flag-scotland"));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flag-tr"));
            Configuration.Localization.Languages.Add(new LanguageInfo("zh-CN", "简体中文", "famfamfam-flag-cn"));

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<AssetManagerNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
