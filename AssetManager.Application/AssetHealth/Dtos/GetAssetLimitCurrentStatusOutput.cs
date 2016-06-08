using Abp.Application.Services.Dto;
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
        public List<VariableLimitStatusDto> variablelimits { get; set; }
    }
}
