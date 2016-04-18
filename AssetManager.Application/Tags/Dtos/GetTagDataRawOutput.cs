using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataRawOutput : IOutputDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public List<TagDataRawDto> TagDataRaw { get; set; }
    }
}
