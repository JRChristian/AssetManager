using Abp.Application.Features;
using AssetManager.Authorization.Roles;
using AssetManager.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.Features
{
    public class FeatureValueStore : AbpFeatureValueStore<Tenant, Role, User>
    {
        public FeatureValueStore(TenantManager tenantManager)
            : base(tenantManager)
        {
        }
    }
}