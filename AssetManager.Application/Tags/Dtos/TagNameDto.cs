using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class TagNameDto : EntityDto<long>
    {
        public string Name { get; set; }
    }
}
