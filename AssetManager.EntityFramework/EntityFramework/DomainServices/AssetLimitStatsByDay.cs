using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetLimitStatsByDay
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<LimitStatsByDay> Limits { get; set; }
    }

    public class LimitStatsByDay
    {
        public long LimitId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public Direction Direction { get; set; }
        public List<LimitStatDays> Days { get; set; }
    }

    public class LimitStatDays
    {
        public DateTime Day { get; set; }
        public long NumberDeviations { get; set; }
        public double DurationHours { get; set; }
    }
}
