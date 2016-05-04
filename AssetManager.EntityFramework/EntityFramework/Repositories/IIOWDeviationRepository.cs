using Abp.Domain.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.Repositories
{
    public interface IIOWDeviationRepository : IRepository<IOWDeviation, long>
    {
    }
}
