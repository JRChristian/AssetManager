using Abp.Application.Services.Dto;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class TagDataChartDto : EntityDto<long>
    {
        public double x { get; set; } // Timestamp in JavaScript format, which is milliseconds since January 1, 1970
        public double y { get; set; }
    }
}
