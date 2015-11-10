using Abp.MultiTenancy;
using AssetManager.Authorization.Roles;
using AssetManager.Editions;
using AssetManager.Users;

namespace AssetManager.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(EditionManager editionManager)
            : base(editionManager)
        {

        }
    }
}