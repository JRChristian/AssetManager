using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class AssetVariableConfiguration : EntityTypeConfiguration<AssetVariable>
    {
        public AssetVariableConfiguration()
        {
            Property(x => x.AssetId).IsRequired();
            Property(x => x.IOWVariableId).IsRequired();
        }
    }
}
