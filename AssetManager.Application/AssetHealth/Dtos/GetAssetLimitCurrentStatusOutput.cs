using Abp.Application.Services.Dto;
using AssetManager.Assets.Dtos;
using AssetManager.IOWs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLimitCurrentStatusOutput : IOutputDto
    {
        public AssetDto Asset{ get; set; }
        public List<LevelDto> LevelsUsed { get; set; }
        public List<VariableLimitStatusDto> VariableLimits { get; set; }

    }
}
