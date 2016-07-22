using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class AssetConfiguration : EntityTypeConfiguration<Asset>
    {
        public AssetConfiguration()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(40);
            Property(x => x.Description).IsRequired().HasMaxLength(255);
            Property(x => x.AssetTypeId).IsOptional();
            Property(x => x.Materials).IsOptional().HasMaxLength(255);
            Property(x => x.TenantId).IsRequired();
        }
    }
}
