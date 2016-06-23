using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class UpdateHealthMetricOutput : IOutputDto
    {
        public HealthMetricDto Metric { get; set; }
    }
}
