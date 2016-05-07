using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public class TagDataName
    {
        public long? TagId { get; set; }
        public string TagName { get; set; }
        public DateTime? Timestamp { get; set; }
        public double Value { get; set; }
        public TagDataQuality? Quality { get; set; }
    }
}
