using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class GetAssetRelativesOutput : IOutputDto
    {
        public AssetDto Asset { get; set; }
        public AssetDto Parent { get; set; }
        public List<AssetDto> Children { get; set; }
    }
}
