using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class TagDataRawUpdateConfiguration : EntityTypeConfiguration<TagDataRawUpdate>
    {
        public TagDataRawUpdateConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.TagId).IsRequired();
            Property(x => x.StartTimestamp).IsRequired();
            Property(x => x.EndTimestamp).IsRequired();
        }
    }
}
