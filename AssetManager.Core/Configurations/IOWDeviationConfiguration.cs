using AssetManager.Entities;
using System;
using System.Collections.Generic;
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
            Property(x => x.StartDate).IsRequired();
            Property(x => x.EndDate).IsOptional();
            Property(x => x.LowLimit).IsOptional();
            Property(x => x.HighLimit).IsOptional();
            Property(x => x.WorstValue).IsOptional();
        }
    }
}
