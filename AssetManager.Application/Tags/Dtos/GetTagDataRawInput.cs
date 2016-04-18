using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataRawInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
        public int? MaxValues { get; set; } // 0=no limit, +=from start, -=from end
    }
}
