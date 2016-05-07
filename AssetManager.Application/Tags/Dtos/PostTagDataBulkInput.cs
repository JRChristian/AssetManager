using Abp.Application.Services.Dto;
using AssetManager.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class PostTagDataBulkInput : IInputDto
    {
        public List<TagDataName> TagDataName { get; set; }
    }
}
