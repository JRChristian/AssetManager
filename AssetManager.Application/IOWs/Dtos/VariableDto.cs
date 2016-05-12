using Abp.Application.Services.Dto;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class VariableDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
    }
}
