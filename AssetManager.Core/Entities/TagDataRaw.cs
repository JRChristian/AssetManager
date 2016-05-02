using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class TagDataRaw : FullAuditedEntity<long>, IMustHaveTenant
    {
        [Index("IX_TenantId")]
        [Index("IX_TenantId_TagId", 1)]
        [Index("IX_TenantId_TagId_Timestamp",1)]
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        [Index("IX_TagId")]
        [Index("IX_TenantId_TagId", 2)]
        [Index("IX_TenantId_TagId_Timestamp", 2)]
        public long TagId { get; set; }
        public virtual Tag Tag { get; set; }

        [Index("IX_TenantId_TagId_Timestamp", 3)]
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public TagDataQuality Quality { get; set; }
    }
}
