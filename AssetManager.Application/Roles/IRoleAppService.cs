using System.Threading.Tasks;
using Abp.Application.Services;
using AssetManager.Roles.Dto;

namespace AssetManager.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
    }
}
