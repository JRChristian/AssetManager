using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Demo.Dtos
{
    public class GetDemoStatusOutput : IOutputDto
    {
        public int NumberTags { get; set; }
        public int NumberVariables { get; set; }
        public int NumberLimits { get; set; }
        public DateTime EarliestTagDataTimestamp { get; set; }
        public DateTime LatestTagDataTimestamp { get; set; }
        public double TagDataTimeRangeDays { get; set; }
        public double NumberHoursToMoveAhead { get; set; }
    }
}
