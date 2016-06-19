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
    public class HealthMetric : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public string Name { get; set; }
        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }
        public long IOWLevelId { get; set; }
        public virtual IOWLevel Level { get; set; }
        public int Period { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        public double WarningLevel { get; set; }
        public double ErrorLevel { get; set; }
    }
}
