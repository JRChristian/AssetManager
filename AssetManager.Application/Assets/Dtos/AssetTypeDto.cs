using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    [AutoMapFrom(typeof(AssetType))]
    public class AssetTypeDto : EntityDto
    {
        public string Name { get; set; }
    }
}
