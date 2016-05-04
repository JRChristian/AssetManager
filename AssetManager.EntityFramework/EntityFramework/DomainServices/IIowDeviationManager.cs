using Abp.Domain.Services;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public interface IIowDeviationManager : IDomainService
    {
        List<IOWDeviation> DetectDeviations(TagDataRaw tagdata);
    }
}
