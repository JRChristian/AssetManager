using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class HealthMetricTypesDto
    {
        public MetricType Code { get; set; }
        public string Description { get; set; }
    }
}
