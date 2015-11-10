using Abp.Authorization;
using AssetManager.Authorization.Roles;
using AssetManager.MultiTenancy;
using AssetManager.Users;

namespace AssetManager.Authorization
{
    public class PermissionChecker : PermissionChecker<Tenant, Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
