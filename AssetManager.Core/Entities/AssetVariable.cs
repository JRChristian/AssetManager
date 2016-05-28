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
    public class AssetVariable : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public long IOWVariableId { get; set; }
        public virtual IOWVariable IOWVariable { get; set; }
    }
}
