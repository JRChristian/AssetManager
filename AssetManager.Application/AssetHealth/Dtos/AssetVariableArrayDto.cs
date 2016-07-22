using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class AssetVariableArrayDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssetTypeName { get; set; }
        public string Materials { get; set; }
        public string ParentAssetName { get; set; }
        public int Level { get; set; }
        public List<VariableArrayDto> Variables { get; set; }
    }

    public class VariableArrayDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public IsVariableAssignedToAsset IsAssigned { get; set; }
    }
}
