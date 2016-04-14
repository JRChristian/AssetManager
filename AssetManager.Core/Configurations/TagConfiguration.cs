using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
   public class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(40);
            Property(x => x.Description).IsRequired().HasMaxLength(255);
            Property(x => x.UOM).IsOptional().HasMaxLength(40);
        }
    }
}
