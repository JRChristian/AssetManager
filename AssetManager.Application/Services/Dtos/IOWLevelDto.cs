using Abp.Application.Services.Dto;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    /// <summary>
    /// A DTO class that can be used in various application service methods when needed to send/receive Level objects.
    /// </summary>
    public class IOWLevelDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }
    }
}
