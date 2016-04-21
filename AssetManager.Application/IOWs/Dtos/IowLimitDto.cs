using Abp.Application.Services.Dto;
using Abp.Extensions;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class IowLimitDto : EntityDto<long>
    {
        public long IOWVariableId { get; set; }

        public long IOWLevelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }

        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public double? LowLimit { get; set; }
        public double? HighLimit { get; set; }

        public DateTime? LastCheckDate { get; set; }
        public IOWStatus? LastStatus { get; set; }
        public DateTime? LastDeviationStartDate { get; set; }
        public DateTime? LastDeviationEndDate { get; set; }
    }
}
