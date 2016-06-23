using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class HealthMetricConfiguration : EntityTypeConfiguration<HealthMetric>
    {
        public HealthMetricConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(40);
            Property(x => x.AssetTypeId).IsRequired();
            Property(x => x.IOWLevelId).IsRequired();
            Property(x => x.Period).IsRequired();
            Property(x => x.MetricType).IsRequired();
            Property(x => x.GoodDirection).IsRequired();
            Property(x => x.WarningLevel).IsRequired();
            Property(x => x.ErrorLevel).IsRequired();
            Property(x => x.ApplyToEachAsset).IsRequired();
            Property(x => x.Order).IsRequired();
        }
    }
}
