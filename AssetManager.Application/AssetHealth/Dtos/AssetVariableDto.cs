using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class AssetVariableDto : EntityDto<long>
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public string AssetTypeName { get; set; }
        public string AssetMaterials { get; set; }
        public long VariableId { get; set; }
        public string VariableName { get; set; }
    }
}
