using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class AssetType : Entity<long>, IHasCreationTime
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<HealthMetric> HealthMetrics { get; set; }

        public AssetType()
        {
            CreationTime = DateTime.Now;
            Assets = new List<Asset>();
            HealthMetrics = new List<HealthMetric>();
        }
    }
}
