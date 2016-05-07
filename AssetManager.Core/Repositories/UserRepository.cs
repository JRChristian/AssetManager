using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.Runtime.Caching;
using AssetManager.Authorization.Roles;
using AssetManager.Entities;
using AssetManager.MultiTenancy;
using AssetManager.Users;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.Repositories
{
    public class UserRepository : AssetManagerRepositoryBase<User,long>
    {
        public UserRepository(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public List<User> GetAllWithTenant(long? TenantId)
        {
            //In repository methods, we do not deal with create/dispose DB connections, DbContexes and transactions. ABP handles it.

            var query = GetAll(); //GetAll() returns IQueryable<T>, so we can query over it.
            //var query = Context.Tasks.AsQueryable(); //Alternatively, we can directly use EF's DbContext object.
            //var query = Table.AsQueryable(); //Another alternative: We can directly use 'Table' property instead of 'Context.Tasks', they are identical.

            //Add some Where conditions...

            if (TenantId.HasValue)
            {
                query = query.Where(x => x.TenantId == TenantId);
            }

            return query
                .OrderBy(x => x.Name)
                .Include(x => x.Tenant) //Include assigned asset type in a single query
                .ToList();
        }
    }
}
