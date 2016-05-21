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
    public class DeviationDto : EntityDto<long>
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
        public double LimitValue { get; set; }
        public double WorstValue { get; set; }
        public double DurationHours
        {
            // Calculate duration from the start and end time. If end is missing, use "now".
            get { return ((EndTimestamp.HasValue ? EndTimestamp.Value : DateTime.Now) - StartTimestamp).TotalHours; }
        }
    }
}
