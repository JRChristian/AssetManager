using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Utilities
{
    public class AssetTypeMetricValue
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public bool ApplyToEachAsset { get; set; }
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        public int Period { get; set; }
        public double Warning { get; set; }
        public double Error { get; set; }
        public double Value { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public double DurationHours { get; set; }
        public int NumberLimits { get; set; }
        public int Order { get; set; }
    }
}
