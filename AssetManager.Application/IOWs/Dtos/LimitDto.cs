using Abp.Application.Services.Dto;
using Abp.Extensions;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class LimitDto : EntityDto<long>
    {
        public long IOWVariableId { get; set; }

        public bool IsActive { get; set; }

        public long IOWLevelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Direction Direction { get; set; }
        public double Value { get; set; }
        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public int SortOrder
        {
            get
            {
                return Direction != Direction.Low ? Criticality : 100-Criticality;
            }
        }
    }
}
