using Abp.Authorization.Roles;
using AssetManager.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.Authorization.Roles
{
    public class Role : AbpRole<Tenant, User>
    {

    }
}