using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class IOWDeviation : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public long IOWLimitId { get; set; }
        public virtual IOWLimit IOWLimits { get; set; }

        public DateTime StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
        public double LimitValue { get; set; }
        public double WorstValue { get; set; }
        public Direction Direction { get; set; }

        public IOWStatus Status
        {
            get { return (EndTimestamp == null ? IOWStatus.OpenDeviation : IOWStatus.Deviation); }
            private set { }
        }
    }
}
