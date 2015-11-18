using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    public class GetAllAssetTypeOutput : IOutputDto
    {
        public List<AssetTypeDto> AssetTypes { get; set; }
    }
}
