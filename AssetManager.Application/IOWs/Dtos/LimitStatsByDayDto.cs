using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class LimitStatsByDayDto : EntityDto<long>
    {
        public long LimitId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public Direction Direction { get; set; }
        public List<LimitStatDaysDto> Days { get; set; }
    }

    public class LimitStatDaysDto
    {
        public DateTime Day { get; set; }
        public long NumberDeviations { get; set; }
        public double DurationHours { get; set; }
    }
}
