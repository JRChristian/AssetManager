using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class IOWLimitConfiguration : EntityTypeConfiguration<IOWLimit>
    {
        public IOWLimitConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.IOWVariableId).IsRequired();
            Property(x => x.IOWLevelId).IsRequired();
            Property(x => x.Cause).IsOptional().HasMaxLength(255);
            Property(x => x.Consequences).IsOptional().HasMaxLength(255);
            Property(x => x.Action).IsOptional().HasMaxLength(255);
            Property(x => x.LowLimit).IsOptional();
            Property(x => x.HighLimit).IsOptional();
            Property(x => x.LastCheckDate).IsOptional();
            Property(x => x.LastStatus).IsOptional();
            Property(x => x.LastDeviationStartDate).IsOptional();
            Property(x => x.LastDeviationEndDate).IsOptional();

            
        }
    }
}
