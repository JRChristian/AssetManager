using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace AssetManager.EntityFramework.Repositories
{
    public abstract class AssetManagerRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<AssetManagerDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected AssetManagerRepositoryBase(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class AssetManagerRepositoryBase<TEntity> : AssetManagerRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected AssetManagerRepositoryBase(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
