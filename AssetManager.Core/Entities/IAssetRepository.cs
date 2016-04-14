using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;

namespace AssetManager.Entities
{
    /// <summary>
    /// Defines a repository to perform database operations for <see cref="Task"/> Entities.
    /// 
    /// Extends <see cref="IRepository{TEntity, TPrimaryKey}"/> to inherit base repository functionality. 
    /// </summary>
    public interface IAssetRepository : IRepository<Asset, long>
    {
        /// <summary>
        /// Gets all tasks with <see cref="Asset.AssetType"/> is retrieved (Include for EntityFramework, Fetch for NHibernate)
        /// filtered by given conditions.
        /// </summary>
        /// <param name="AssetTypeId">Optional assigned person filter. If it's null, not filtered.</param>
        /// <returns>List of found tasks</returns>
        List<Asset> GetAllWithType(long? AssetTypeId);
    }
}
