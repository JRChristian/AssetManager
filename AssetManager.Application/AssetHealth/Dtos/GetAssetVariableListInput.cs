using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetVariableListInput : IInputDto
    {
        public long? AssetId { get; set; }
        public string AssetName { get; set; }
        public long? VariableId { get; set; }
        public string VariableName { get; set; }
    }
}
