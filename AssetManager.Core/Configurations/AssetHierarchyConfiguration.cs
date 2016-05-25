using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class AssetHierarchyConfiguration : EntityTypeConfiguration<AssetHierarchy>
    {
        public AssetHierarchyConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.AssetId).IsRequired();
            Property(x => x.ParentAssetHierarchyId).IsOptional();
        }
    }
}
