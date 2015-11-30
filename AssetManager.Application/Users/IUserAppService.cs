using System.Threading.Tasks;
using Abp.Application.Services;
using AssetManager.Users.Dto;

namespace AssetManager.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(ProhibitPermissionInput input);
        Task RemoveFromRole(long userId, string roleName);
        Task<GetUserOutput> GetAllUsers();
        Task<GetOneUserOutput> GetOneUser(GetUserInput input);
        Task UpdateUser(UpdateUserInput input);
    }
}