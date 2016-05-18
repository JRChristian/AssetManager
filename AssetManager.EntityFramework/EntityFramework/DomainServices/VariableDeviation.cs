using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public class VariableDeviation
    {
        public long VariableId { get; set; }
        public string VariableName { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public long LimitId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public Direction Direction { get; set; }
        public int DeviationCount { get; set; }
        public double DurationHours { get; set; }
    }
}
