using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class IOWStatsByDay : FullAuditedEntity<long>, IMustHaveTenant
    {
        [Index("IX_TenantId")]
        [Index("IX_TenantId_IOWLimitId", 1)]
        [Index("IX_TenantId_IOWLimitId_Day", 1)]
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        [Index("IOWLimitId")]
        [Index("IX_TenantId_IOWLimitId", 2)]
        [Index("IX_TenantId_IOWLimitId_Day", 2)]
        public long IOWLimitId { get; set; }
        public virtual IOWLimit IOWLimits { get; set; }

        [Index("IX_TenantId_IOWLimitId_Day", 3)]
        public DateTime Day { get; set; }
        public long NumberDeviations { get; set; }
        public double DurationHours { get; set; }
    }
}
