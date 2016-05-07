using Abp.Domain.Services;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public interface IIowManager : IDomainService
    {
        IOWLevel FirstOrDefaultLevel(long id);
        IOWLevel FirstOrDefaultLevel(string name);
        IOWLevel FirstOrDefaultLevel(Expression<Func<IOWLevel, bool>> predicate);
        List<IOWLevel> GetAllLevels();
        IOWLevel InsertOrUpdateLevel(IOWLevel input);
        bool DeleteLevel(long id);
        bool DeleteLevel(string name);
        bool DeleteLevel(Expression<Func<IOWLevel, bool>> predicate);

        List<IOWDeviation> DetectDeviations(TagDataRaw tagdata);
    }
}
