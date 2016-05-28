using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetVariableListOutput : IOutputDto
    {
        public List<AssetVariableDto> AssetVariables { get; set; }
    }
}
