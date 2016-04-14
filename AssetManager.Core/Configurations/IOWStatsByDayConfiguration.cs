using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class IOWStatsByDayConfiguration : EntityTypeConfiguration<IOWStatsByDay>
    {
        public IOWStatsByDayConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.IOWLimitId).IsRequired();
            Property(x => x.Day).IsRequired();
            Property(x => x.Status).IsRequired();
            Property(x => x.DeviationCount).IsRequired();
            Property(x => x.DeviationDuration).IsRequired();
        }
    }
}
