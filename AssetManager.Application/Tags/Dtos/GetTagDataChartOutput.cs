using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataChartOutput : IOutputDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int? Precision { get; set; }
        public double? MinTimestampJS { get; set; }
        public double? MaxTimestampJS { get; set; }
        public List<TagDataChartDto> TagDataChart { get; set; }
    }
}
