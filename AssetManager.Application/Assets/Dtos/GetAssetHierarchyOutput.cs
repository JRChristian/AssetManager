﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class GetAssetHierarchyOutput : IOutputDto
    {
        public List<AssetHierarchyDto> AssetHierarchy { get; set; }
    }
}
