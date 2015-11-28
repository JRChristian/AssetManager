using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Users.Dto
{
    public class GetUserOutput : IOutputDto
    {
        public List<UserDto> Users { get; set; }
    }
}
