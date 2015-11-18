using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class AssetTypeConfiguration : EntityTypeConfiguration<AssetType>
    {
        public AssetTypeConfiguration()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(40);
        }
    }
}
