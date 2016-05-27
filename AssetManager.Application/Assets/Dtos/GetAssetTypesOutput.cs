﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class GetAssetTypesOutput : IOutputDto
    {
        public List<AssetTypeDto> AssetTypes { get; set; }
    }
}
