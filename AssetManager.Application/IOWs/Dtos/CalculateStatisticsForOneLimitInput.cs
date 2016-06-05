using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class CalculateStatisticsForOneLimitInput : IInputDto
    {
        public int? LimitId { get; set; }
        public string VariableName { get; set; }
        public string LevelName { get; set; }
        public Direction? Direction { get; set; }
        public DateTime? startTimestamp { get; set; }
        public DateTime? endTimestamp { get; set; }
    }
}
