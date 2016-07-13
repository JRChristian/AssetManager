using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataInput : IInputDto
    {
        public List<TagNameDto> Tags { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
