using Abp.EntityFramework;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.Repositories
{
    public class TagDataRawUpdateRepository : AssetManagerRepositoryBase<TagDataRawUpdate, long>, ITagDataRawUpdateRepository
    {
        public TagDataRawUpdateRepository(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

    }
}
