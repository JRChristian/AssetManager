using Abp.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.MultiTenancy
{
    public class Tenant : AbpTenant<Tenant, User>
    {

    }
}