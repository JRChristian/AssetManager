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

        public async Task<GetUserOutput> GetAllUsers()
        {
            List<User> _users = await _userRepository.GetAllListAsync();
            //List<UserDto> userDto = Mapper.Map<List<UserDto>>(_users);
            List<UserDto> userDto = new List<UserDto>();
            foreach(User oneUser in _users)
            {
                userDto.Add(new UserDto {
                    Id = oneUser.Id,
                    Name = oneUser.Name,
                    Surname = oneUser.Surname,
                    UserName = oneUser.UserName,
                    EmailAddress = oneUser.EmailAddress,
                    LastLoginTime = oneUser.LastLoginTime,
                    IsActive = oneUser.IsActive,
                    TenantId = (oneUser.TenantId.HasValue) ? oneUser.TenantId.Value : 0,
                    TenancyName = (oneUser.TenantId.HasValue ) ? oneUser.Tenant.TenancyName : "(Host)"
                });
            }

            return new GetUserOutput
            {
                Users = userDto
                //Users = Mapper.Map<List<UserDto>>( _users )
            };
        }

        public async Task<GetOneUserOutput> GetOneUser(GetUserInput input)
        {
            User oneUser = await _userRepository.FirstOrDefaultAsync(x => x.Id == input.UserId);
            UserDto _userDto = new UserDto
            {
                Id = oneUser.Id,
                UserName = oneUser.UserName,
                Name = oneUser.Name,
                Surname = oneUser.Surname,
                EmailAddress = oneUser.EmailAddress,
                LastLoginTime = oneUser.LastLoginTime,
                IsActive = oneUser.IsActive,
                TenantId = (oneUser.TenantId.HasValue) ? oneUser.TenantId.Value : 0,
                TenancyName = (oneUser.TenantId.HasValue) ? oneUser.Tenant.TenancyName : "(Host)"
            };
            return new GetOneUserOutput
            {
                User = _userDto
            };
        }

        public async Task UpdateUser(UpdateUserInput input)
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Updating a user for input: " + input);

            //Get the original user
            User theUser = await _userRepository.FirstOrDefaultAsync(x => x.Id == input.Id);

            //Update changed properties of the retriever User entity
            string s;
            s = input.Name.Trim();
            if (s != "")
            {
                if (s == s.ToLower() || s == s.ToUpper())
                    s = s[0].ToString().ToUpper() + s.Substring(1).ToLower();
                theUser.Name = s;
            }

            s = input.Surname.Trim();
            if (s != "")
            {
                if (s == s.ToLower() || s == s.ToUpper())
                    s = s[0].ToString().ToUpper() + s.Substring(1).ToLower();
                theUser.Surname = s;
            }

            s = input.EmailAddress.Trim();
            if (s != "")
                theUser.EmailAddress = s;

            //No need to call the Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default and
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }
    }
}