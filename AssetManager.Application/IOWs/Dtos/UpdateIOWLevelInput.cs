using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class UpdateIOWLevelInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1, 5)]
        public int? Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }
    }
}
