using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataOutput : IOutputDto
    {
        public List<TagDataDto> TagData { get; set; }
    }
}
