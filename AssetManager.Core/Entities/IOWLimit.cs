using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class IOWLimit : FullAuditedEntity<long>, IMustHaveTenant
    {
        [Index("IX_TenantId")]
        [Index("IX_TenantId_IOWVariableId_IOWLevelId", 1)]
        [Index("IX_TenantId_IOWVariableId", 1)]
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        [Index("IX_IOWVariableId")]
        [Index("IX_TenantId_IOWVariableId", 2)]
        [Index("IX_TenantId_IOWVariableId_IOWLevelId", 2)]
        public long IOWVariableId { get; set; }
        public virtual IOWVariable Variable { get; set;  }

        [Index("IX_TenantId_IOWVariableId_IOWLevelId", 3)]
        public long IOWLevelId { get; set; }
        public virtual IOWLevel Level { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Direction Direction { get; set; }
        public double Value { get; set; }
        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public DateTime? LastCheckDate { get; set; }
        public IOWStatus LastStatus { get; set; }
        public DateTime? LastDeviationStartTimestamp { get; set; }
        public DateTime? LastDeviationEndTimestamp { get; set; }

        public virtual ICollection<IOWDeviation> IOWDeviations { get; set; }
        public virtual ICollection<IOWStatsByDay> IOWStatsByDay { get; set; }
    }
}
