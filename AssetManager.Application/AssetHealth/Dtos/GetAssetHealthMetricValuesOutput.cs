using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetHealthMetricValuesOutput : IOutputDto
    {
        public List<AssetTypeMetricValue> Metrics { get; set; }
    }
}
