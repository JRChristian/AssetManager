using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using AssetManager.Entities;

using AssetManager.Users.Dto;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetManager.Users
{
    /* THIS IS JUST A SAMPLE. */
    public class UserAppService : AssetManagerAppServiceBase, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly UserStore _userStore;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<User, long> _userRepository;

        public UserAppService(UserManager userManager, UserStore userStore, IRepository<User, long> userRepository, IPermissionManager permissionManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _userRepository = userRepository;
            _permissionManager = permissionManager;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await _userManager.ProhibitPermissionAsync(user, permission);
        }

        //Example for primitive method parameters.
        public async Task RemoveFromRole(long userId, string roleName)
        {
            CheckErrors(await _userManager.RemoveFromRoleAsync(userId, roleName));
        }

         public async Task<GetUserOutput> GetAllUsers(GetUserInput input)
        {
            List<User> _users = await _userRepository.GetAllListAsync();
            //List<UserDto> userDto = Mapper.Map<List<UserDto>>(_users);
            List<UserDto> userDto = new List<UserDto>();
            foreach(User oneUser in _users)
            {
                userDto.Add(new UserDto {
                    Name = oneUser.Name,
                    Surname = oneUser.Surname,
                    UserName = oneUser.UserName,
                    EmailAddress = oneUser.EmailAddress,
                    LastLoginTime = oneUser.LastLoginTime,
                    IsActive = oneUser.IsActive,
                    TenantId = oneUser.TenantId,
                    TenancyName = (oneUser.TenantId >0 ) ? oneUser.Tenant.TenancyName : "(Host)"
                });
            }

            return new GetUserOutput
            {
                Users = userDto
                //Users = Mapper.Map<List<UserDto>>( _users )
            };
        }
    }
}