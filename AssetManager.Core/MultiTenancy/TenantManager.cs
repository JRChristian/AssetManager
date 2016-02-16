using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using AssetManager.Authorization.Roles;
using AssetManager.Editions;
using AssetManager.Users;

namespace AssetManager.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(
        IRepository<Tenant> tenantRepository,
        IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
        EditionManager editionManager) :
        base(
            tenantRepository,
            tenantFeatureRepository,
            editionManager
        )
        {
        }
    }
}