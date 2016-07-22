using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AssetManager.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public class Asset : Entity<long>, IHasCreationTime, IMustHaveTenant
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }

        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }
        public string Materials { get; set; }

        public int TenantId { get; set; }
        public virtual Tenant Tenants { get; set; }

        public virtual ICollection<AssetImage> Images { get; set; }
        public virtual ICollection<AssetHierarchy> Hierarchy { get; set; }
        public virtual ICollection<AssetVariable> Variables { get; set; }

        public Asset()
        {
            CreationTime = DateTime.Now;
        }
    }
}
