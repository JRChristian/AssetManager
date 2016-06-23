using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class HealthMetricDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public bool ApplyToEachAsset { get; set; }
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public int Period { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        public double WarningLevel { get; set; }
        public double ErrorLevel { get; set; }
        public int Order { get; set; }
    }
}
