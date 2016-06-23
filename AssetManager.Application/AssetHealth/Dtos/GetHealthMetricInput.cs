using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetHealthMetricInput : IInputDto
    {
        public long? HealthMetricId { get; set; }
        public string HealthMetricName { get; set; }
    }
}
