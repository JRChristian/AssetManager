using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLimitChartByDayOutput : IOutputDto
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public int NumberLimits { get; set; }
        public CanvasJSBar CanvasJS { get; set; }
    }
}
