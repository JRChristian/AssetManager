using Abp.Domain.Services;
using AssetManager.DomainServices;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public interface IAssetHealthManager : IDomainService
    {
        // Variable-Asset assignments
        List<AssetVariable> GetAssetVariableList();
        List<AssetVariable> GetAssetVariableList(long? assetId, string assetName, long? variableId, string variableName);
        List<AssetVariable> UpdateAssetVariableList(List<AssetVariableCombinations> input);
        bool DeleteAssetVariable(long? assetId, string assetName, long? variableId, string variableName);
        AssetDeviationSummaryOutput GetAssetLevelTimeSummary(DateTime? startTimestamp, int? hoursInPeriod);
        List<AssetLimitStatsByDay> GetAssetLimitStatsByDay(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, bool includeAsset, bool includeChildren);
        //List<AssetLimitStats> GetAssetLimitStats(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality, bool includeAsset, bool includeChildren);
        List<AssetLevelStats> GetAssetLevelStats(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality, bool includeAsset, bool includeChildren);
    }
}
