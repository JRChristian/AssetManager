using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class AssetHierarchyDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssetTypeName { get; set; }
        public string ParentAssetName { get; set; }
        public string Materials { get; set; }
        public int Level { get; set; }
    }
}
