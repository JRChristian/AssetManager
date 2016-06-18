using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetLimitStats
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<LimitStats> Limits { get; set; }
    }

    public class LimitStats
    {
        public long LimitId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public long NumberDeviations { get; set; }
        public double DurationHours { get; set; }
    }
}
