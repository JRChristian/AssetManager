using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Localization;
using AssetManager.AssetHealth.Dtos;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
using AssetManager.IOWs.Dtos;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth
{
    public class AssetHealthAppService : AssetManagerAppServiceBase, IAssetHealthAppService
    {
        private readonly IAssetManager _assetManager;
        private readonly IAssetHealthManager _assetHealthManager;
        private readonly IIowManager _iowManager;
        private readonly ILocalizationManager _localizationManager;

        public AssetHealthAppService( 
            IAssetManager assetManager,
            IAssetHealthManager assetHealthManager,
            IIowManager iowManager,
            ILocalizationManager localizationManager)
        {
            _assetManager = assetManager;
            _assetHealthManager = assetHealthManager;
            _iowManager = iowManager;
            _localizationManager = localizationManager;
        }

        // Asset-Variable combinations
        public GetAssetVariableListOutput GetAssetVariableList(GetAssetVariableListInput input)
        {
            GetAssetVariableListOutput output = new GetAssetVariableListOutput { AssetVariables = null };

            List<AssetVariable> assetVariables = _assetHealthManager.GetAssetVariableList(input.AssetId, input.AssetName, input.VariableId, input.VariableName);
            if (assetVariables != null)
            {
                output.AssetVariables = new List<AssetVariableDto>();
                foreach (AssetVariable av in assetVariables)
                {
                    output.AssetVariables.Add(new AssetVariableDto { Id = av.Id, AssetId = av.AssetId, AssetName = av.Asset.Name, VariableId = av.IOWVariableId, VariableName = av.IOWVariable.Name });
                }
            }
            return output;
        }

        // This routine is similar to GetAssetHierarchyAsList() in the Asset Manager, with the addition of variables associated with each asset.
        public GetAssetHierarchyWithVariablesAsListOutput GetAssetHierarchyWithVariablesAsList(GetAssetHierarchyWithVariablesAsListInput input)
        {
            // Output array
            List<AssetVariableArrayDto> flatHierarchy = new List<AssetVariableArrayDto>();

            // Get all nodes
            List<AssetHierarchy> rawHierarchy = _assetManager.GetAssetHierarchy();
            HierarchyBuilder(rawHierarchy, ref flatHierarchy, 0, null, "");

            return new GetAssetHierarchyWithVariablesAsListOutput
            {
                AssetHierarchy = flatHierarchy
            };
        }

        private void HierarchyBuilder(List<AssetHierarchy> rawHierarchy, ref List<AssetVariableArrayDto> flatHierarchy, int level, long? parentAssetHierarchyId, string parentAssetName)
        {
            var oneLevel = from assets in rawHierarchy
                           where assets.ParentAssetHierarchyId == parentAssetHierarchyId
                           orderby assets.Asset.Name ascending
                           select assets;

            if (oneLevel != null && oneLevel.Count() > 0)
            {
                foreach (var oneItem in oneLevel)
                {
                    AssetVariableArrayDto hierarchy = new AssetVariableArrayDto
                    {
                        Id = oneItem.AssetId,
                        Name = oneItem.Asset.Name,
                        Description = oneItem.Asset.Description,
                        AssetTypeName = oneItem.Asset.AssetType.Name,
                        ParentAssetName = parentAssetName,
                        Level = level,
                        Variables = new List<VariableArrayDto>()
                    };
                    // Now add the child variables, if any
                    if( oneItem.Asset != null && oneItem.Asset.Variables != null )
                    {
                        foreach(var v in oneItem.Asset.Variables)
                        {
                            VariableArrayDto variable = new VariableArrayDto
                            {
                                Id = v.IOWVariableId,
                                Name = v.IOWVariable.Name,
                                Description = v.IOWVariable.Description,
                                TagName = v.IOWVariable.Tag.Name,
                                UOM = v.IOWVariable.UOM,
                                IsAssigned = IsVariableAssignedToAsset.Yes
                            };
                            hierarchy.Variables.Add(variable);
                        }
                        hierarchy.Variables = hierarchy.Variables.OrderBy(p => p.Name).ToList();
                    }

                    flatHierarchy.Add(hierarchy);

                    // And add any children
                    HierarchyBuilder(rawHierarchy, ref flatHierarchy, level + 1, oneItem.Id, oneItem.Asset.Name);
                }
            }
        }

        public UpdateAssetVariableListOutput UpdateAssetVariableList(UpdateAssetVariableListInput input)
        {
            UpdateAssetVariableListOutput output = new UpdateAssetVariableListOutput { NumberUpdates = 0 };
            if( input.AssetVariables != null )
            {
                List<AssetVariableCombinations> thingsToUpdate = new List<AssetVariableCombinations>();
                foreach(AssetVariableDto oneInputItem in input.AssetVariables)
                {
                    AssetVariableCombinations oneOutputItem = new AssetVariableCombinations
                    {
                        AssetName = oneInputItem.AssetName,
                        VariableName = oneInputItem.VariableName,
                        IsAssigned = IsVariableAssignedToAsset.NoAndAdd
                    };
                    thingsToUpdate.Add(oneOutputItem);
                }

                if( thingsToUpdate.Count > 0 )
                {
                    List<AssetVariable> result = _assetHealthManager.UpdateAssetVariableList(thingsToUpdate);
                    if (result != null)
                        output.NumberUpdates = result.Count;
                }
            }
            return output;
        }

        public DeleteAssetVariableListOutput DeleteAssetVariableList(DeleteAssetVariableListInput input)
        {
            DeleteAssetVariableListOutput output = new DeleteAssetVariableListOutput { NumberDeletes = 0 };
            if (input.AssetVariables != null)
            {
                foreach (AssetVariableDto oneInputItem in input.AssetVariables)
                {
                    if( _assetHealthManager.DeleteAssetVariable(null, oneInputItem.AssetName, null, oneInputItem.VariableName) )
                        output.NumberDeletes++;
                }
            }
            return output;
        }

        public GetHealthMetricTypesOutput GetHealthMetricTypes()
        {
            var localize = _localizationManager.GetSource("AssetManager");

            List<HealthMetricTypesDto> metricTypes = new List<HealthMetricTypesDto>();
            metricTypes.Add(new HealthMetricTypesDto { Code = MetricType.None, Description = localize.GetString("AssetHealthLblMetricTypeNone") });
            metricTypes.Add(new HealthMetricTypesDto { Code = MetricType.PercentTimeInDeviation, Description = localize.GetString("AssetHealthLblMetricTypePctTime") });
            metricTypes.Add(new HealthMetricTypesDto { Code = MetricType.PercentLimitsInDeviation, Description = localize.GetString("AssetHealthLblMetricTypePctLimits") });

            return new GetHealthMetricTypesOutput { MetricTypes = metricTypes };
        }

        public GetHealthMetricOutput GetHealthMetric(GetHealthMetricInput input)
        {
            HealthMetric healthMetric = _assetHealthManager.GetHealthMetric(input.HealthMetricId, input.HealthMetricName);
            GetHealthMetricOutput output = new GetHealthMetricOutput
            {
                Metric = healthMetric.MapTo<HealthMetricDto>()
            };
            if (output.Metric != null && healthMetric.Level != null)
                output.Metric.Criticality = healthMetric.Level.Criticality;
            return output;
        }

        public GetHealthMetricListOutput GetHealthMetricList(GetHealthMetricListInput input)
        {
            List<HealthMetric> healthMetrics = _assetHealthManager.GetHealthMetricList();
            GetHealthMetricListOutput output = new GetHealthMetricListOutput
            {
                Metrics = new List<HealthMetricDto>()
            };
            if( healthMetrics != null && healthMetrics.Count > 0 )
            {
                foreach( HealthMetric metric in healthMetrics )
                {
                    HealthMetricDto metricDto = metric.MapTo<HealthMetricDto>();
                    metricDto.Criticality = metric.Level.Criticality;
                    output.Metrics.Add(metricDto);
                }
            }
            return output;
        }

        public UpdateHealthMetricOutput UpdateHealthMetric(UpdateHealthMetricInput input)
        {
            UpdateHealthMetricOutput output = new UpdateHealthMetricOutput { Metric = null };
            AssetType assetType = null;
            IOWLevel level = null;

            if (input != null)
            {

                if (input.Metric.AssetTypeId > 0)
                    assetType = _assetManager.GetAssetType(input.Metric.AssetTypeId);
                else
                    assetType = _assetManager.GetAssetType(input.Metric.AssetTypeName);

                if (input.Metric.LevelId > 0)
                    level = _iowManager.FirstOrDefaultLevel(input.Metric.LevelId);
                else
                    level = _iowManager.FirstOrDefaultLevel(input.Metric.LevelName);

                if( assetType != null && level != null )
                {
                    HealthMetric metric = _assetHealthManager.GetHealthMetric(input.Metric.Id, input.Metric.Name);

                    if (metric == null)
                        metric = new HealthMetric { };

                    metric.TenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;
                    metric.Name = input.Metric.Name;
                    metric.AssetTypeId = assetType.Id;
                    metric.ApplyToEachAsset = input.Metric.ApplyToEachAsset;
                    metric.IOWLevelId = level.Id;
                    metric.Period = input.Metric.Period;
                    metric.MetricType = input.Metric.MetricType;
                    metric.GoodDirection = input.Metric.GoodDirection;
                    metric.WarningLevel = input.Metric.WarningLevel;
                    metric.ErrorLevel = input.Metric.ErrorLevel;
                    metric.Order = input.Metric.Order;

                    HealthMetric outputMetric =  _assetHealthManager.UpdateHealthMetric( metric );
                    if (outputMetric != null)
                        output.Metric = outputMetric.MapTo<HealthMetricDto>();
                }
            }
            return output;
        }

        public DeleteHealthMetricOutput DeleteHealthMetric(DeleteHealthMetricInput input)
        {
            return new DeleteHealthMetricOutput
            {
                Succeeded = _assetHealthManager.DeleteHealthMetric(input.Id, input.Name)
            };
        }

        public GetAssetLevelTimeSummaryOutput GetAssetLevelTimeSummary(GetAssetLevelTimeSummaryInput input)
        {
            AssetDeviationSummaryOutput assetSummary = _assetHealthManager.GetAssetLevelTimeSummary(input.StartTimestamp, input.HoursInPeriod);

            return new GetAssetLevelTimeSummaryOutput
            {
                StartTimestamp = assetSummary.StartTimestamp,
                EndTimestamp = assetSummary.EndTimestamp,
                NumberPeriods = assetSummary.NumberPeriods,
                HoursInPeriod = assetSummary.HoursInPeriod,
                AssetDeviations = assetSummary.AssetDeviations.MapTo<List<AssetLevelTimeDto>>()
            };
        }

        public GetAssetLimitStatsByDayOutput GetAssetLimitStatsByDay(GetAssetLimitStatsByDayInput input)
        {
            GetAssetLimitStatsByDayOutput output = new GetAssetLimitStatsByDayOutput { };
            output.StartTimestamp = _iowManager.NormalizeStartDay(input.StartTimestamp);
            output.EndTimestamp = _iowManager.NormalizeEndDay(output.StartTimestamp, input.EndTimestamp);

            List<AssetLimitStatsByDay> assetLimits = _assetHealthManager.GetAssetLimitStatsByDay(input.AssetId, input.AssetName, input.StartTimestamp, input.EndTimestamp, true, true);

            if (assetLimits != null)
                output.AssetLimits = assetLimits.MapTo<List<AssetLimitStatsByDayDto>>();

            return output;
        }

        public GetAssetLimitChartByDayOutput GetAssetLimitChartByDay(GetAssetLimitChartByDayInput input)
        {
            Asset asset = _assetManager.GetAsset(input.AssetId, input.AssetName);
            GetAssetLimitChartByDayOutput output = new GetAssetLimitChartByDayOutput
            {
                AssetId = (asset != null) ? asset.Id : 0,
                AssetName = (asset != null) ? asset.Name : "",
                AssetDescription = (asset != null) ? asset.Description : "",
                StartTimestamp = _iowManager.NormalizeStartDay(input.StartTimestamp),
                NumberLimits = 0,
                CanvasJS = new CanvasJSBar
                {
                    exportEnabled = true,
                    title = new CanvasJSBarTitle { text = "" /*(asset != null) ? (asset.Name + ": " + asset.Description) : ""*/ },
                    axisX = new CanvasJSBarAxisX { },
                    axisY = new CanvasJSBarAxisY { title = "Deviation hours" },
                    data = new List<CanvasJSBarData>()
                }
            };
            output.EndTimestamp = _iowManager.NormalizeEndDay(output.StartTimestamp, input.EndTimestamp);

            List<AssetLimitStatsByDay> assetLimits = _assetHealthManager.GetAssetLimitStatsByDay(input.AssetId, input.AssetName, input.StartTimestamp, input.EndTimestamp, true, false);

            if (assetLimits != null && assetLimits.Count > 0 && assetLimits[0].Limits != null && assetLimits[0].Limits.Count > 0 )
            {
                // There should be exactly one asset
                // Each limit will be its own series
                output.NumberLimits = assetLimits[0].Limits.Count;
                foreach (LimitStatsByDay limit in assetLimits[0].Limits)
                {
                    CanvasJSBarData data = new CanvasJSBarData
                    {
                        name = limit.Criticality.ToString() + " - " + limit.LevelName,
                        type = "stackedColumn",
                        legendText = limit.Criticality.ToString() + " - " + limit.LevelName,
                        showInLegend = true,
                        color = ChartColors.Criticality(limit.Criticality),
                        dataPoints = new List<CanvasJSBarDataPoints>()
                    };

                    // Now add the daily records
                    if (limit.Days != null)
                    {
                        foreach (LimitStatDays day in limit.Days)
                        {
                            CanvasJSBarDataPoints point = new CanvasJSBarDataPoints
                            {
                                y = day.DurationHours,
                                label = day.Day.ToString("m")
                            };
                            data.dataPoints.Add(point);
                        }
                    }
                    output.CanvasJS.data.Add(data);
                }
            }
            return output;
        }

    public GetAssetLevelChartOutput GetAssetLevelChartCanvasJS(GetAssetLevelChartInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");

            // Start the chart
            GetAssetLevelChartOutput output = new GetAssetLevelChartOutput
            {
                CanvasJS = new CanvasJSBar
                {
                    exportEnabled = true,
                    title = new CanvasJSBarTitle { },
                    axisX = new CanvasJSBarAxisX { interval=1 },
                    axisY = new CanvasJSBarAxisY { },
                    data = new List<CanvasJSBarData>()
                }
            };

            // Get the list of assets
            List<Asset> assets = _assetManager.GetAssetList().OrderBy(p => p.Name).ToList();
            if (assets == null || assets.Count <= 0) return output;

            // Get the asset/level/time combinations. Not all assets will appear.
            // Specify an aggregation of 0, which means one time record per asset/level.
            // Sort by criticality and level first, to simplify matching later.
            AssetDeviationSummaryOutput assetSummary = _assetHealthManager.GetAssetLevelTimeSummary(input.StartTimestamp, 0);
            if (assetSummary == null || assetSummary.AssetDeviations == null) return output;
            output.StartTimestamp = assetSummary.StartTimestamp;
            output.EndTimestamp = assetSummary.EndTimestamp;
            output.NumberPeriods = assetSummary.NumberPeriods;
            output.HoursInPeriod = assetSummary.HoursInPeriod;

            List<AssetDeviationSummary> assetDeviations = assetSummary.AssetDeviations
                .OrderBy(p => p.Criticality).ThenBy(p => p.LevelName).ThenBy(p => p.AssetName)
                .ToList();

            // i is the index into the current record in assetDeviations
            int i = 0;

            // Get the set of distinct level names
            var query = (from x in assetDeviations
                         select new { x.Criticality, x.LevelName })
                        .Distinct().OrderBy(x => x.Criticality).ThenBy(x => x.LevelName);
            var allLevels = query.ToList();
            if (allLevels == null || allLevels.Count <= 0) return output;

            // Build the chart
            //output.CanvasJS.title.text = "Asset health by level over time";
            output.CanvasJS.axisY.title = "Deviation hours from " + assetSummary.StartTimestamp.ToString("d") + " to now";

            // Each distinct level will be its own bar in the stack
            foreach (var level in allLevels)
            {
                CanvasJSBarData data = new CanvasJSBarData
                {
                    name = level.Criticality.ToString() + " - " + level.LevelName,
                    type = "stackedBar",
                    legendText = level.Criticality.ToString() + " - " + level.LevelName,
                    showInLegend = true,
                    color = ChartColors.Criticality(level.Criticality),
                    dataPoints = new List<CanvasJSBarDataPoints>()
                };

                // Now add the assets to the level structure
                foreach(Asset asset in assets)
                {
                    CanvasJSBarDataPoints points = new CanvasJSBarDataPoints
                    {
                        y = 0,
                        label = asset.Name
                    };

                    // assetDeviations is sorted by criticality, then level name, then asset name
                    // Go forward in assetDeviations until the end or a match on the first criteria (criticality)
                    for (; i < assetDeviations.Count && assetDeviations[i].Criticality < level.Criticality; i++);

                    // Go forward in assetDeviations until the end or a match on the first and second criteria
                    for (; i < assetDeviations.Count && assetDeviations[i].Criticality <= level.Criticality && string.Compare(assetDeviations[i].LevelName, level.LevelName) < 0; i++) ;

                    // Go forward in assetDeviations until the end or a match on all three criteria
                    for (; i < assetDeviations.Count && assetDeviations[i].Criticality <= level.Criticality && string.Compare(assetDeviations[i].LevelName, level.LevelName) <= 0 && string.Compare(assetDeviations[i].AssetName, asset.Name) < 0; i++) ;

                    // If a match, then use the duration hours as the Y value
                    // Note that assetDeviations was generated for a single time period, so there should only be one DeviationDetails record.
                    if (i < assetDeviations.Count && 
                        assetDeviations[i].Criticality == level.Criticality &&
                        string.Compare(assetDeviations[i].LevelName, level.LevelName) == 0 &&
                        string.Compare(assetDeviations[i].AssetName, asset.Name) == 0)
                    {
                        points.y += assetDeviations[i].DeviationDetails[0].DurationHours;
                    }

                    data.dataPoints.Add(points);
                }
                output.CanvasJS.data.Add(data);
            }

            return output;
        }

        public GetAssetLevelStatsOutput GetAssetLevelStats(GetAssetLevelStatsInput input)
        {
            GetAssetLevelStatsOutput output = new GetAssetLevelStatsOutput { };
            output.StartTimestamp = _iowManager.NormalizeStartDay(input.StartTimestamp);
            output.EndTimestamp = _iowManager.NormalizeEndTimestamp(output.StartTimestamp, input.EndTimestamp);
            output.DurationHours = (output.EndTimestamp - output.StartTimestamp).TotalHours;
            output.AssetLevelStats = _assetHealthManager.GetAssetLevelStats(input.AssetId, input.AssetName, true, true, output.StartTimestamp, output.EndTimestamp, null, null);

            return output;
        }

        public GetAssetLimitCurrentStatusOutput GetAssetLimitCurrentStatus(GetAssetLimitCurrentStatusInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");
            string[] localizedDirectionNames = new string[4]
            {
                localize.GetString("DirectionNone"),
                localize.GetString("DirectionLow"),
                localize.GetString("DirectionFocus"),
                localize.GetString("DirectionHigh") };

            // Get all the variable/limit combinations that match the input
            List<AssetVariable> assetVariables = _assetHealthManager.GetAssetVariableList(input.AssetId, input.AssetName, null, null);
            if( assetVariables == null || assetVariables.Count <= 0 )
                return new GetAssetLimitCurrentStatusOutput { variablelimits = null };

            List<long> variableIds = new List<long>();
            foreach (AssetVariable av in assetVariables)
                variableIds.Add(av.IOWVariableId);

            List<IOWLimit> limits = _iowManager.GetAllLimits(variableIds);

            // Transform into our output format
            List<VariableLimitStatusDto> output = new List<VariableLimitStatusDto>();
            foreach (IOWLimit limit in limits)
            {
                string severityMessage1 = "";
                string severityMessage2 = "";
                string severityClass = "";
                if (limit.LastStatus == IOWStatus.OpenDeviation)
                {
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgActive");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                    else if (limit.Level.Criticality == 3)
                        severityClass = "label label-default";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue && (DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalHours <= 24)
                {
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgLast24Hours");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue)
                {
                    double days = Math.Round((DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalDays, 0);
                    severityMessage1 = "";
                    severityMessage2 = String.Format(localize.GetString("IowMsgNotRecent"), days);
                    severityClass = "";
                }

                output.Add(new VariableLimitStatusDto
                {
                    VariableId = limit.Variable.Id,
                    VariableName = limit.Variable.Name,
                    VariableDescription = limit.Variable.Description,
                    TagId = limit.Variable.TagId,
                    TagName = limit.Variable.Tag.Name,
                    UOM = limit.Variable.UOM,
                    LastTimestamp = limit.Variable.Tag.LastTimestamp,
                    LastValue = limit.Variable.Tag.LastValue,
                    LastQuality = limit.Variable.Tag.LastQuality,

                    IOWLevelId = limit.IOWLevelId,
                    LevelName = limit.Level.Name,
                    LevelDescription = limit.Level.Description,
                    Criticality = limit.Level.Criticality,
                    ResponseGoal = limit.Level.ResponseGoal,
                    MetricGoal = limit.Level.MetricGoal,

                    LimitName = string.Format("{0}-", limit.Level.Criticality) + limit.Level.Name + "-" + localizedDirectionNames[Convert.ToInt32(limit.Direction)],
                    Direction = limit.Direction,
                    LimitValue = limit.Value,
                    Cause = limit.Cause,
                    Consequences = limit.Consequences,
                    Action = limit.Action,

                    LastStatus = limit.LastStatus,
                    LastDeviationStartTimestamp = limit.LastDeviationStartTimestamp,
                    LastDeviationEndTimestamp = limit.LastDeviationEndTimestamp,

                    SeverityMessage1 = severityMessage1,
                    SeverityMessage2 = severityMessage2,
                    SeverityClass = severityClass
                });
            }
            return new GetAssetLimitCurrentStatusOutput { variablelimits = output };
        }

        public GetAssetHealthMetricValuesOutput GetAssetHealthMetricValues()
        {
            return new GetAssetHealthMetricValuesOutput
            {
                Metrics = _assetHealthManager.GetAssetHealthMetricValues()
            };
        }
    }
}
