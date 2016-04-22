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
    public class IOWLimit : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public long IOWVariableId { get; set; }
        public virtual IOWVariable Variable { get; set;  }

        public long IOWLevelId { get; set; }
        public virtual IOWLevel Level { get; set; }

        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public double? LowLimit { get; set; }
        public double? HighLimit { get; set; }

        public DateTime? LastCheckDate { get; set; }
        public IOWStatus LastStatus { get; set; }
        public DateTime? LastDeviationStartDate { get; set; }
        public DateTime? LastDeviationEndDate { get; set; }

        public virtual ICollection<IOWDeviation> IOWDeviations { get; set; }
        public virtual ICollection<IOWStatsByDay> IOWStatsByDay { get; set; }
    }
}
