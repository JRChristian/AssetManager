﻿using Abp.Application.Services;
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

        GetHealthMetricTypesOutput GetHealthMetricTypes();
        GetHealthMetricOutput GetHealthMetric(GetHealthMetricInput input);
        GetHealthMetricListOutput GetHealthMetricList(GetHealthMetricListInput input);
        UpdateHealthMetricOutput UpdateHealthMetric(UpdateHealthMetricInput input);
        DeleteHealthMetricOutput DeleteHealthMetric(DeleteHealthMetricInput input);

        GetAssetLevelTimeSummaryOutput GetAssetLevelTimeSummary(GetAssetLevelTimeSummaryInput input);
        GetAssetLimitStatsByDayOutput GetAssetLimitStatsByDay(GetAssetLimitStatsByDayInput input);
        GetAssetLimitChartByDayOutput GetAssetLimitChartByDay(GetAssetLimitChartByDayInput input);
        GetAssetLevelChartOutput GetAssetLevelChartCanvasJS(GetAssetLevelChartInput input);
        GetAssetLevelStatsOutput GetAssetLevelStats(GetAssetLevelStatsInput input);
        GetAssetLevelStatsforTypeOutput GetAssetLevelStatsForType(GetAssetLevelStatsForTypeInput input);
        GetAssetLimitStatsOutput GetAssetLimitStats(GetAssetLimitStatsInput input);
        GetCompoundAssetLevelStatsOutput GetCompoundAssetLevelStats(GetCompoundAssetLevelStatsInput input);
        GetAssetLimitCurrentStatusOutput GetAssetLimitCurrentStatus(GetAssetLimitCurrentStatusInput input);
        GetAssetHealthMetricValuesOutput GetAssetHealthMetricValues();
    }
}
