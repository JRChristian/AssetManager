﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Users.Dto
{
    public class GetUserInput : IInputDto
    {
        public long? UserId { get; set; }
    }
}
