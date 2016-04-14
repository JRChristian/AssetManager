using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class IOWStatsByDay : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public long IOWLimitId { get; set; }
        public virtual IOWLimit IOWLimits { get; set; }

        public DateTime Day { get; set; }
        public IOWStatus Status { get; set; }
        public long DeviationCount { get; set; }
        public double DeviationDuration { get; set; }
    }
}
