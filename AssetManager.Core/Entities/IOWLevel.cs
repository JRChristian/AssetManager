using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class IOWLevel : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, 9, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double WarningLevel { get; set; }
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double ErrorLevel { get; set; }

        public virtual ICollection<IOWLimit> IOWLimits { get; set; }
        public virtual ICollection<HealthMetric> HealthMetrics { get; set; }
    }
}
