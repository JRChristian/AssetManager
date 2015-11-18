using Abp.EntityFramework;
using AssetManager.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AssetManager.EntityFramework.Repositories
{
    /// <summary>
    /// Implements <see cref="IAssetRepository"/> for EntityFramework ORM.
    /// </summary>
    public class AssetRepository : AssetManagerRepositoryBase<Asset, long>, IAssetRepository
    {
        public AssetRepository(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public List<Asset> GetAllWithType(long? AssetTypeId)
        {
            //In repository methods, we do not deal with create/dispose DB connections, DbContexes and transactions. ABP handles it.

            var query = GetAll(); //GetAll() returns IQueryable<T>, so we can query over it.
            //var query = Context.Tasks.AsQueryable(); //Alternatively, we can directly use EF's DbContext object.
            //var query = Table.AsQueryable(); //Another alternative: We can directly use 'Table' property instead of 'Context.Tasks', they are identical.

            //Add some Where conditions...

            if (AssetTypeId.HasValue)
            {
                query = query.Where(x => x.AssetTypeId == AssetTypeId);
            }

            return query
                .OrderBy(x => x.Name)
                .Include(x => x.AssetType) //Include assigned asset type in a single query
                .ToList();
        }
    }
}