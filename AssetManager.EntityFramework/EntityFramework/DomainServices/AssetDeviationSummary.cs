using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public class AssetDeviationSummary
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public List<DeviationDetails> DeviationDetails { get; set; }
    }

    public class DeviationDetails
    {
        public DateTime Timestamp { get; set; }
        public int DeviationCount { get; set; }
        public double DurationHours { get; set; }
    }

    public class AssetDeviationSummaryOutput
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public int NumberPeriods { get; set; }
        public int HoursInPeriod { get; set; }
        public List<AssetDeviationSummary> AssetDeviations { get; set; }
    }
}
