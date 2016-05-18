using Abp.Application.Services.Dto;
using AssetManager.Entities;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class DeviationsDto : EntityDto<long>
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
        public double LimitValue { get; set; }
        public double WorstValue { get; set; }
        public Direction Direction { get; set; }
        public IOWStatus Status { get; set; }
        public double DurationHours { get; set; }
    }
}
