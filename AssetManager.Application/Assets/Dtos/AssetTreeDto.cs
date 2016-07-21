using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class AssetTreeDto : EntityDto<long>
    {
        // This structure matches what is needed for Angular-ui-tree
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public long? ParentAssetTreeDtoId { get; set; }
        public virtual AssetTreeDto Parent { get; set; }
        public virtual ICollection<AssetTreeDto> Nodes { get; set; }
    }
}
