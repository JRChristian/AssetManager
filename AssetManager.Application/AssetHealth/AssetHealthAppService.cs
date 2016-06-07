using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Localization;
using AssetManager.AssetHealth.Dtos;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
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
                    output.AssetVariables.Add(new AssetVariableDto { Id = av.Id, AssetName = av.Asset.Name, VariableName = av.IOWVariable.Name });
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

            List<AssetLimitStatsByDay> assetLimits = _assetHealthManager.GetAssetLimitStatsByDay(input.AssetId, input.AssetName, input.StartTimestamp, input.EndTimestamp);

            if (assetLimits != null)
                output.AssetLimits = assetLimits.MapTo<List<AssetLimitStatsByDayDto>>();

            return output;
        }

        public GetAssetLevelChartOutput GetAssetLevelChartCanvasJS(GetAssetLevelChartInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");

            // The color index corresponds to the criticality
            string[] colors =
            {
                "rgba(0,75,141,0.7)",   // 0 - blue
                "rgba(255,0,0,0.7)",    // 1 - red
                "rgba(255,102,0,0.7)",  // 2 - orange
                "darkgreen",            // 3 - green
                "rgba(51,51,51,0.7)",   // 4 - gray
                "rgba(100,100,100,0.7)" // 5 - lighter gray
            };

            // Start the chart
            GetAssetLevelChartOutput output = new GetAssetLevelChartOutput
            {
                CanvasJS = new CanvasJSHorizontalBar
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
                    color = colors[level.Criticality],
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
    }
}
