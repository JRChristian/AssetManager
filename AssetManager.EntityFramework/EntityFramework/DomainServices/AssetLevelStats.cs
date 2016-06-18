using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetLevelStats
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<LevelStats> Levels { get; set; }
    }

    public class LevelStats
    {
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public long NumberDeviations { get; set; }
        public double DurationHours { get; set; }
        public double NumberLimits { get; set; }
        public long NumberDeviatingLimits { get; set; }
    }
}
