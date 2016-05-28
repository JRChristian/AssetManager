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
    public class IOWVariable : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public long TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public string UOM { get; set; }

        public virtual ICollection<IOWLimit> IOWLimits { get; set; }
        public virtual ICollection<AssetVariable> Assets { get; set; }
    }
}
