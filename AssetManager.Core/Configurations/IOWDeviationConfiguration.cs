using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class IOWDeviationConfiguration : EntityTypeConfiguration<IOWDeviation>
    {
        public IOWDeviationConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.IOWLimitId).IsRequired();
            Property(x => x.StartTimestamp).IsRequired();
            Property(x => x.EndTimestamp).IsOptional();
            Property(x => x.LimitValue).IsRequired();
            Property(x => x.WorstValue).IsRequired();
            Property(x => x.Direction).IsRequired();
            Property(x => x.Status).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}
