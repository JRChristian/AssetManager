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
    public class TagDataRawUpdate : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }
        public long TagId { get; set; }
        public virtual Tag Tag { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
    }
}
