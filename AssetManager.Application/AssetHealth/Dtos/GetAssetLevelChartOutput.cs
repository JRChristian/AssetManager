using Abp.Application.Services.Dto;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLevelChartOutput : IOutputDto
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public int NumberPeriods { get; set; }
        public int HoursInPeriod { get; set; }
        public CanvasJSHorizontalBar CanvasJS { get; set; }
    }
}
