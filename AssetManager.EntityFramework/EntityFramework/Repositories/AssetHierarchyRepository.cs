using Abp.EntityFramework;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.Repositories
{
    public class AssetHierarchyRepository : AssetManagerRepositoryBase<AssetHierarchy, long>, IAssetHierarchyRepository
    {
        public AssetHierarchyRepository(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
