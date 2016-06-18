using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetLimitStatsChartByDayOutput : IOutputDto
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public CanvasJSBar CanvasJS { get; set; }
    }
}
