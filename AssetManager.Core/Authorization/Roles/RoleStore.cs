using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using AssetManager.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.Authorization.Roles
{
    public class RoleStore : AbpRoleStore<Tenant, Role, User>
    {
        public RoleStore(
            IRepository<Role> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                roleRepository,
                userRoleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}