using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using AssetManager.Authorization.Roles;
using AssetManager.MultiTenancy;
using AssetManager.Users;


namespace AssetManager.Features
{
    public class FeatureValueStore : AbpFeatureValueStore<Tenant, Role, User>
    {
        public FeatureValueStore(
            ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager)
            : base(cacheManager, tenantFeatureRepository, tenantRepository, editionFeatureRepository, featureManager)
        {
        }
    }
}