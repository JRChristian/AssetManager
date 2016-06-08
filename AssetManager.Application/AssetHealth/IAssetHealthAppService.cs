using Abp.Application.Services;
using AssetManager.AssetHealth.Dtos;
using AssetManager.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth
{
    public interface IAssetHealthAppService : IApplicationService
    {
        GetAssetVariableListOutput GetAssetVariableList(GetAssetVariableListInput input);
        GetAssetHierarchyWithVariablesAsListOutput GetAssetHierarchyWithVariablesAsList(GetAssetHierarchyWithVariablesAsListInput input);
        UpdateAssetVariableListOutput UpdateAssetVariableList(UpdateAssetVariableListInput input);
        DeleteAssetVariableListOutput DeleteAssetVariableList(DeleteAssetVariableListInput input);
        GetAssetLevelTimeSummaryOutput GetAssetLevelTimeSummary(GetAssetLevelTimeSummaryInput input);
        GetAssetLimitStatsByDayOutput GetAssetLimitStatsByDay(GetAssetLimitStatsByDayInput input);
        GetAssetLimitChartByDayOutput GetAssetLimitChartByDay(GetAssetLimitChartByDayInput input);
        GetAssetLevelChartOutput GetAssetLevelChartCanvasJS(GetAssetLevelChartInput input);
    }
}
