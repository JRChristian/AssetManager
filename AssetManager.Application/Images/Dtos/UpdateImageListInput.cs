﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Images.Dtos
{
    public class UpdateImageListInput : IInputDto
    {
        public List<ImageDto> Images { get; set; }
    }
}