using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class AssetTypeMetricsDto
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public List<MetricStats> Overall { get; set; }
        public List<AssetStats> Assets { get; set; }
    }

    public class AssetStats
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<MetricStats> Metrics { get; set; }
    }

    public class MetricStats
    {
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        public double WarningLevel { get; set; }
        public double ErrorLevel { get; set; }
        public double MetricValue { get; set; }
    }
}
