using Abp.EntityFramework;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.Repositories
{
    public class IOWLevelRepository : AssetManagerRepositoryBase<IOWLevel, long>, IIOWLevelRepository
    {
        public IOWLevelRepository(IDbContextProvider<AssetManagerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
