using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFramework;
using Abp.Runtime.Session;
using AssetManager.DomainServices;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using Abp.Domain.Uow;
using AssetManager.Utilities;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetHealthManager : DomainService, IAssetHealthManager
    {
        private readonly IAssetVariableRepository _assetVariableRepository;
        private readonly IHealthMetricRepository _healthMetricRepository;
        private readonly IAssetManager _assetManager;
        private readonly IIowManager _iowManager;

        private readonly int minimumPeriod = 1; // Days
        private readonly int maximumPeriod = 30; // Days
        private readonly double minimumLevel = 0; // 0%
        private readonly double maximumLevel = 100; // 100%

        public AssetHealthManager(
            IAssetVariableRepository assetVariableRepository,
            IHealthMetricRepository healthMetricRepository,
            IAssetManager assetManager,
            IIowManager iowManager
            )
        {
            _assetVariableRepository = assetVariableRepository;
            _healthMetricRepository = healthMetricRepository;
            _assetManager = assetManager;
            _iowManager = iowManager;
        }

        // Variable-Asset assignments
        public List<AssetVariable> GetAssetVariableList()
        {
            return _assetVariableRepository.GetAll().OrderBy(p => p.Asset.Name).ThenBy(p => p.IOWVariable.Name).ToList();
        }

        public List<AssetVariable> GetAssetVariableList(long? assetId, string assetName, long? variableId, string variableName)
        {
            List<AssetVariable> output = null;

            // Get the asset and/or variable specified in the input
            Asset asset = _assetManager.GetAsset(assetId, assetName);
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(variableId, variableName);

            // If the input included a valid asset and/or variable, use them to subset the list
            if (asset != null && variable != null) // Should be just 0 or 1 items found
                output = _assetVariableRepository.GetAllList(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
            else if (asset != null) // All variables for a specified asset
                output = _assetVariableRepository.GetAll().Where(p => p.AssetId == asset.Id).OrderBy(p => p.IOWVariable.Name).ToList();
            else if (variable != null) // All assets for a specified variable
                output = _assetVariableRepository.GetAll().Where(p => p.IOWVariableId == variable.Id).OrderBy(p => p.Asset.Name).ToList();
            else // Everything
                output = _assetVariableRepository.GetAll().OrderBy(p => p.Asset.Name).ThenBy(p => p.IOWVariable.Name).ToList();

            return output;
        }

        public List<AssetVariable> UpdateAssetVariableList(List<AssetVariableCombinations> input)
        {
            List<AssetVariable> output = new List<AssetVariable>();
            Asset asset = null;
            IOWVariable variable = null;
            AssetVariable av = null;

            // Loop through the input, validate the inputs, and then insert or update the cross-combination record
            foreach (AssetVariableCombinations one in input)
            {
                asset = _assetManager.GetAsset(one.AssetId, one.AssetName);
                variable = _iowManager.FirstOrDefaultVariable(one.VariableId, one.VariableName);
                if (asset != null && variable != null)
                {
                    av = _assetVariableRepository.FirstOrDefault(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
                    if (av == null)
                    {
                        av = new AssetVariable { AssetId = asset.Id, IOWVariableId = variable.Id, TenantId = asset.TenantId };
                        av = _assetVariableRepository.InsertOrUpdate(av);
                    }

                    output.Add(av);
                }
            }
            return output;
        }

        public bool DeleteAssetVariable(long? assetId, string assetName, long? variableId, string variableName)
        {
            bool success = false;

            Asset asset = _assetManager.GetAsset(assetId, assetName);
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(variableId, variableName);
            if (asset != null && variable != null)
            {
                AssetVariable input = _assetVariableRepository.FirstOrDefault(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
                if (input != null)
                {
                    _assetVariableRepository.Delete(input);
                    success = true;
                }
            }
            return success;
        }

        public HealthMetric GetHealthMetric(long? Id, string Name)
        {
            HealthMetric metric = null;
            if (Id.HasValue)
                metric = _healthMetricRepository.FirstOrDefault(p => p.Id == Id.Value);
            else if (!string.IsNullOrEmpty(Name))
                metric = _healthMetricRepository.FirstOrDefault(p => p.Name == Name);

            return metric;
        }

        public List<HealthMetric> GetHealthMetricList()
        {
            return _healthMetricRepository.GetAll().OrderBy(p => p.Order).ThenBy(p => p.Name).ToList();
        }

        public HealthMetric UpdateHealthMetric(HealthMetric input)
        {
            // Validate information
            if (string.IsNullOrEmpty(input.Name))
                return null;

            if (input.Period < minimumPeriod)
                input.Period = minimumPeriod;
            if (input.Period > maximumPeriod)
                input.Period = maximumPeriod;

            // If the good direction is down, then the warning level must be below the error level (going down: error, warning, good)
            if (input.GoodDirection == Direction.Low)
            {
                if (input.WarningLevel < minimumLevel)
                    input.WarningLevel = minimumLevel;
                if (input.ErrorLevel > maximumLevel)
                    input.ErrorLevel = maximumLevel;
                if (input.WarningLevel > input.ErrorLevel)
                    input.WarningLevel = input.ErrorLevel;
            }
            // If the good direction is up, then the warning level must be above the error level (going down: good, warning, error)
            else if (input.GoodDirection == Direction.High)
            {
                if (input.WarningLevel > maximumLevel)
                    input.WarningLevel = maximumLevel;
                if (input.ErrorLevel < minimumLevel)
                    input.ErrorLevel = minimumLevel;
                if (input.WarningLevel < input.ErrorLevel)
                    input.WarningLevel = input.ErrorLevel;
            }
            else
                // Good direction is not valid
                return null;

            // Do we have this metric?
            HealthMetric metric = GetHealthMetric(input.Id, input.Name);
            if (metric == null)
            {
                metric = new HealthMetric
                {
                    TenantId = input.TenantId,
                    Name = input.Name,
                    AssetTypeId = input.AssetTypeId,
                    ApplyToEachAsset = input.ApplyToEachAsset,
                    IOWLevelId = input.IOWLevelId,
                    Period = input.Period,
                    MetricType = input.MetricType,
                    GoodDirection = input.GoodDirection,
                    WarningLevel = input.WarningLevel,
                    ErrorLevel = input.ErrorLevel,
                    Order = input.Order
                };
                metric.Id = _healthMetricRepository.InsertAndGetId(metric);
                return metric;
            }

            // No need to call the update method

            return metric;
        }

        public bool DeleteHealthMetric(long? Id, string Name)
        {
            bool successfullyDeleted = false;
            HealthMetric metric = GetHealthMetric(Id, Name);
            if (metric != null)
            {
                // To do: prevent deleting metrics that are in use
                _healthMetricRepository.Delete(metric);
                successfullyDeleted = true;
            }
            return successfullyDeleted;
        }



        [UnitOfWork]
        public AssetDeviationSummaryOutput GetAssetLevelTimeSummary(DateTime? startTime, int? hoursInPeriodInput)
        {
            // Process the arguments and build a datetime array.
            // The starting time must be within the last 1000 days. If omitted, go back 30 days.
            DateTime startTimestamp;
            if (startTime.HasValue && (DateTime.Now - startTime.Value).TotalDays < 1000.0)
                startTimestamp = startTime.Value;
            else
                startTimestamp = DateTime.Today.AddDays(-29); // 30th day will be the current (partial) day

            int hoursInPeriod;
            int numberPeriods;
            if (hoursInPeriodInput.HasValue && hoursInPeriodInput.Value > 0 && hoursInPeriodInput.Value < 1000)
            {
                hoursInPeriod = hoursInPeriodInput.Value;
                numberPeriods = (int)((DateTime.Now - startTimestamp).TotalHours / hoursInPeriod) + 1;
            }
            else
            {
                numberPeriods = 1;
                hoursInPeriod = (int)Math.Ceiling((DateTime.Now - startTimestamp).TotalHours); // Math.Ceiling() rounds up
            }

            // Initialize the output
            AssetDeviationSummaryOutput output = new AssetDeviationSummaryOutput
            {
                StartTimestamp = startTimestamp,
                EndTimestamp = DateTime.Now,
                NumberPeriods = numberPeriods,
                HoursInPeriod = hoursInPeriod,
                AssetDeviations = new List<AssetDeviationSummary>()
            };

            // Build a datetime array for the output
            List<DateTime> datetimes = new List<DateTime>();
            DateTime dt = startTimestamp;
            for (int i = 0; i < numberPeriods; i++)
            {
                datetimes.Add(dt);
                dt = dt.AddHours(hoursInPeriod);
            }


            var query = from av in _assetVariableRepository.GetAllList()
                        from a in _assetManager.GetAssetList()
                            .Where(a => a.Id == av.AssetId)
                        from v in _iowManager.GetAllVariables()
                            .Where(v => v.Id == av.IOWVariableId)
                        from l in _iowManager.GetAllLimits()
                            .Where(l => l.IOWVariableId == v.Id)
                        orderby a.Name, v.Name, l.Level.Criticality, l.Level.Name
                        select new
                        {
                            AssetId = a.Id,
                            AssetName = a.Name,
                            VariableId = v.Id,
                            VariableName = v.Name,
                            LimitId = l.Id,
                            LevelName = l.Level.Name,
                            Criticality = l.Level.Criticality,
                            Direction = l.Direction
                        };
            var allLimits = query.ToList();

            // allLimits contains all asset/variable/limit combinations. Now get the deviations and build the output.
            // Note that allLimits may contain multiple asset/variable/limit combinations that need to be mapped to a single asset/level combination,
            // as there can be multiple variables for a single asset, and we are lumping together high and low violations. If there are N input
            // records, expect M output records, where M <= N.
            if (allLimits != null && allLimits.Count > 0)
            {
                var lastLimit = allLimits[0];
                bool isFirstTimeThrough = true;
                AssetDeviationSummary oneItem = null;

                foreach (var limit in allLimits)
                {
                    // If this is the first time through OR the asset/level information changed, save the previous record (if any) and start a new one
                    if (isFirstTimeThrough || limit.AssetId != lastLimit.AssetId || limit.LevelName != lastLimit.LevelName)
                    {
                        if (!isFirstTimeThrough)
                            output.AssetDeviations.Add(oneItem);

                        oneItem = new AssetDeviationSummary
                        {
                            AssetId = limit.AssetId,
                            AssetName = limit.AssetName,
                            LevelName = limit.LevelName,
                            Criticality = limit.Criticality,
                            DeviationDetails = new List<DeviationDetails>()
                        };
                        foreach (var d in datetimes)
                        {
                            oneItem.DeviationDetails.Add(new DeviationDetails { Timestamp = d, DeviationCount = 0, DurationHours = 0 });
                        }
                    }

                    // Get deviations
                    List<IOWDeviation> deviations = _iowManager.GetDeviations(limit.LimitId, startTimestamp);
                    if (deviations != null && deviations.Count > 0)
                    {
                        int i = 0;
                        foreach (IOWDeviation dev in deviations)
                        {
                            // Deviations are in ascending date order.
                            // Each entry in the output array represents a range from [i].Timestamp to [i+1].Timestamp, typically an hour, a day, or several days.
                            // Call [i].Timestamp the "index start" and [i+1].Timestamp the "index end".
                            // Move ahead in the output array until the time range overlaps the duration: until index end > deviation start
                            for (; i < numberPeriods && oneItem.DeviationDetails[i].Timestamp.AddHours(hoursInPeriod) < dev.StartTimestamp; i++) ;

                            // Continue moving ahead in the output array, adding the deviation to the output array records until we reach the end of the deviation.
                            for (; i < numberPeriods && oneItem.DeviationDetails[i].Timestamp <= (dev.EndTimestamp ?? DateTime.Now); i++)
                            {
                                DateTime usableStartTimestamp = dev.StartTimestamp > oneItem.DeviationDetails[i].Timestamp ? dev.StartTimestamp : oneItem.DeviationDetails[i].Timestamp;
                                DateTime usableEndTimestamp = (dev.EndTimestamp ?? DateTime.Now) < oneItem.DeviationDetails[i].Timestamp.AddHours(hoursInPeriod) ? (dev.EndTimestamp ?? DateTime.Now) : oneItem.DeviationDetails[i].Timestamp.AddHours(hoursInPeriod);
                                oneItem.DeviationDetails[i].DeviationCount++;
                                oneItem.DeviationDetails[i].DurationHours += (usableEndTimestamp - usableStartTimestamp).TotalHours;
                            }
                        }
                    }
                    lastLimit = limit;
                    isFirstTimeThrough = false;

                } // foreach (var limit in allLimits)
                output.AssetDeviations.Add(oneItem);

            } // if( allLimits != null && allLimits.Count > 0 )
            return output;
        }

        public List<AssetLimitStatsByDay> GetAssetLimitStatsByDay(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, bool includeAsset, bool includeChildren)
        {
            List<Asset> assets = null;

            // Get all top level children of the specified asset. Possibly add the parent asset (specified in the arguments) to the list as well.
            if (includeChildren)
                assets = _assetManager.GetAssetChildren(assetId, assetName, includeAsset);
            else
            {
                Asset oneAsset = _assetManager.GetAsset(assetId, assetName);
                if (oneAsset != null)
                {
                    assets = new List<Asset>();
                    assets.Add(oneAsset);
                }
            }

            List<AssetLimitStatsByDay> assetLimits = null;
            if (assets != null)
            {
                assetLimits = new List<AssetLimitStatsByDay>();
                foreach (Asset asset in assets)
                {
                    AssetLimitStatsByDay assetLimit = new AssetLimitStatsByDay
                    {
                        AssetId = asset.Id,
                        AssetName = asset.Name,
                        Limits = new List<LimitStatsByDay>()
                    };

                    // Get the list of unique limits for this asset
                    List<long> limitIds = (from av in _assetVariableRepository.GetAllList()
                                           join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                           where av.AssetId == asset.Id
                                           select l.Id).Distinct().ToList();

                    // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                    if (limitIds != null && limitIds.Count > 0)
                    {
                        List<LimitStatsByDay> limitStats = _iowManager.GetLimitStatsByDayGroupByLevel(limitIds, startTimestamp, endTimestamp);
                        if (limitStats != null && limitStats.Count > 0)
                        {
                            foreach (LimitStatsByDay ls in limitStats)
                            {
                                LimitStatsByDay onels = new LimitStatsByDay
                                {
                                    LimitId = ls.LimitId,
                                    LevelName = ls.LevelName,
                                    Criticality = ls.Criticality,
                                    Direction = ls.Direction,
                                    Days = new List<LimitStatDays>()
                                };
                                if (ls.Days != null && ls.Days.Count > 0)
                                {
                                    foreach (LimitStatDays oneDay in ls.Days)
                                        onels.Days.Add(new LimitStatDays { Day = oneDay.Day, NumberDeviations = oneDay.NumberDeviations, DurationHours = oneDay.DurationHours });
                                }
                                assetLimit.Limits.Add(onels);
                            }
                        }
                    }
                    assetLimits.Add(assetLimit);
                }
            }
            return assetLimits;
        }

        /*
         * Get statistics for assets in a variety of ways
         *   - For a specified asset
         *   - For a specified asset and its children
         *   - For the children of a specified asset (but not the asset itself - useful when there isn't a top level, or the top level isn't known)
         *   - For all assets of a particular type
         */
        public List<AssetLevelStats> GetAssetLevelStatsForAsset(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<Asset> assets = null;

            Asset oneAsset = _assetManager.GetAsset(assetId, assetName);
            if (oneAsset != null)
            {
                assets = new List<Asset>();
                assets.Add(oneAsset);
            }
            return GetAssetLevelStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }

        public List<AssetLevelStats> GetAssetLevelStatsForAssetType(long? assetTypeId, string assetTypeName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<AssetLevelStats> output = new List<AssetLevelStats>();

            AssetType assetType = _assetManager.GetAssetType(assetTypeId, assetTypeName);
            if( assetType != null )
            {
                List<Asset> assets = _assetManager.GetAssetListForType(assetType.Id);
                output = GetAssetLevelStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
            }
            return output;
        }

        public List<AssetLevelStats> GetAssetLevelStatsByAssetType(DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<AssetLevelStats> output = new List<AssetLevelStats>();

            List<AssetType> assetTypes = _assetManager.GetAssetTypeList();
            if (assetTypes != null)
            {
                foreach(AssetType assetType in assetTypes)
                {
                    List<Asset> assets = _assetManager.GetAssetListForType(assetType.Id);
                    int numberChildren = (assets != null) ? assets.Count : 0;
                    List<LevelStats> levels = GetLevelStatsForAssets(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
                    if( levels != null && levels.Count > 0)
                        output.Add(new AssetLevelStats { AssetTypeId = assetType.Id, AssetTypeName = assetType.Name, Levels = levels, NumberChildren = numberChildren });
                }
            }
            return output;
        }

        public List<AssetLevelStats> GetAssetLevelStatsForTopLevel(DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            // This call to GetAssetChildren() returns all assets that do not have a parent
            List<Asset> assets = _assetManager.GetAssetChildren(null, null, false);
            return GetAssetLevelStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }

        public List<AssetLevelStats> GetAssetLevelStatsForChildren(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            // This call to GetAssetChildren() returns all assets that are children of the specified asset, and does not include the parent
            List<Asset> assets = _assetManager.GetAssetChildren(assetId, assetName, false);
            return GetAssetLevelStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }

        public List<AssetLevelStats> GetAssetLevelStats(long? assetId, string assetName, bool includeAsset, bool includeChildren, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<Asset> assets = null;

            // Get all top level children of the specified asset. Possibly add the parent asset (specified in the arguments) to the list as well.
            if (includeChildren)
                assets = _assetManager.GetAssetChildren(assetId, assetName, includeAsset);
            else
            {
                Asset oneAsset = _assetManager.GetAsset(assetId, assetName);
                if (oneAsset != null)
                {
                    assets = new List<Asset>();
                    assets.Add(oneAsset);
                }
            }

            return GetAssetLevelStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }

        public List<AssetLevelStats> GetAssetLevelStats(List<Asset> assets, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<AssetLevelStats> assetLevels = null;
            if (assets != null)
            {
                assetLevels = new List<AssetLevelStats>();
                foreach (Asset asset in assets)
                {
                    // Get the list of unique limits for this asset
                    List<long> limitIds = null;
                    if (minCriticality.HasValue || maxCriticality.HasValue)
                    {
                        // Criticality was specified in the input, so get just limits matching the specified criticality range
                        int lowerCriticality = minCriticality.HasValue ? minCriticality.Value : -1;
                        int upperCriticality = maxCriticality.HasValue ? maxCriticality.Value : 999;

                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    where av.AssetId == asset.Id && l.Level.Criticality >= lowerCriticality && l.Level.Criticality <= upperCriticality
                                    select l.Id).Distinct().ToList();
                    }
                    else
                    {
                        // Criticality was not specified in the input, so get all limits
                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    where av.AssetId == asset.Id
                                    select l.Id).Distinct().ToList();
                    }

                    // Does this asset have children?
                    int numberChildren = _assetManager.GetAssetChildrenCount(asset.Id, asset.Name);

                    AssetLevelStats assetLevel = new AssetLevelStats
                    {
                        AssetId = asset.Id,
                        AssetName = asset.Name,
                        AssetDescription = asset.Description,
                        AssetTypeId = asset.AssetTypeId,
                        AssetTypeName = (asset.AssetType != null) ? asset.AssetType.Name : "",
                        AssetMaterials = asset.Materials,
                        Levels = null,
                        NumberChildren = numberChildren
                    };

                    // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                    if (limitIds != null && limitIds.Count > 0)
                        assetLevel.Levels = _iowManager.GetPerLevelStatsOverTime(limitIds, startTimestamp, endTimestamp);

                    assetLevels.Add(assetLevel);
                }
            }
            return assetLevels;
        }

        public List<LevelStats> GetLevelStatsForAssets(List<Asset> assets, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<LevelStats> levelStats = null;
            if (assets != null)
            {
                    // Get the list of unique limits for this asset
                    List<long> limitIds = null;
                    if (minCriticality.HasValue || maxCriticality.HasValue)
                    {
                        // Criticality was specified in the input, so get just limits matching the specified criticality range
                        int lowerCriticality = minCriticality.HasValue ? minCriticality.Value : -1;
                        int upperCriticality = maxCriticality.HasValue ? maxCriticality.Value : 999;

                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    join a in assets on av.AssetId equals a.Id
                                    where l.Level.Criticality >= lowerCriticality && l.Level.Criticality <= upperCriticality
                                    select l.Id).Distinct().ToList();
                    }
                    else
                    {
                        // Criticality was not specified in the input, so get all limits
                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    join a in assets on av.AssetId equals a.Id
                                    select l.Id).Distinct().ToList();
                    }

                    // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                    if (limitIds != null && limitIds.Count > 0)
                        levelStats = _iowManager.GetPerLevelStatsOverTime(limitIds, startTimestamp, endTimestamp);
            }
            return levelStats;
        }

        public List<AssetLimitStats> GetAssetLimitStatsForAsset(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<Asset> assets = null;

            Asset oneAsset = _assetManager.GetAsset(assetId, assetName);
            if (oneAsset != null)
            {
                assets = new List<Asset>();
                assets.Add(oneAsset);
            }
            return GetAssetLimitStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }
        public List<AssetLimitStats> GetAssetLimitStatsForAssetType(long? assetTypeId, string assetTypeName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<Asset> assets = null;
            AssetType assetType = _assetManager.GetAssetType(assetTypeId, assetTypeName);
            if (assetType != null)
                assets = _assetManager.GetAssetListForType(assetType.Id);

            return GetAssetLimitStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }
        public List<AssetLimitStats> GetAssetLimitStatsForTopLevel(DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            // This call to GetAssetChildren() returns all assets that do not have a parent
            List<Asset> assets = _assetManager.GetAssetChildren(null, null, false);
            return GetAssetLimitStats(assets, startTimestamp, endTimestamp, minCriticality, maxCriticality);
        }

        public List<AssetLimitStats> GetAssetLimitStats(List<Asset> assets, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<AssetLimitStats> assetLimits = null;
            if (assets != null)
            {
                assetLimits = new List<AssetLimitStats>();
                foreach (Asset asset in assets)
                {
                    // Get the list of unique limits for this asset
                    List<long> limitIds = null;
                    if (minCriticality.HasValue || maxCriticality.HasValue)
                    {
                        // Criticality was specified in the input, so get just limits matching the specified criticality range
                        int lowerCriticality = minCriticality.HasValue ? minCriticality.Value : -1;
                        int upperCriticality = maxCriticality.HasValue ? maxCriticality.Value : 999;

                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    where av.AssetId == asset.Id && l.Level.Criticality >= lowerCriticality && l.Level.Criticality <= upperCriticality
                                    select l.Id).Distinct().ToList();
                    }
                    else
                    {
                        // Criticality was not specified in the input, so get all limits
                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    where av.AssetId == asset.Id
                                    select l.Id).Distinct().ToList();
                    }

                    // Does this asset have children?
                    int numberChildren = _assetManager.GetAssetChildrenCount(asset.Id, asset.Name);

                    AssetLimitStats assetLimit = new AssetLimitStats
                    {
                        AssetId = asset.Id,
                        AssetName = asset.Name,
                        AssetDescription = asset.Description,
                        AssetTypeId = asset.AssetTypeId,
                        AssetTypeName = (asset.AssetType != null) ? asset.AssetType.Name : "",
                        AssetMaterials = asset.Materials,
                        Limits = null,
                        NumberChildren = numberChildren
                    };

                    // Get the stats for these limits and add them to the output.
                    if (limitIds != null && limitIds.Count > 0)
                        assetLimit.Limits = _iowManager.GetPerLimitStatsOverTime(limitIds, startTimestamp, endTimestamp);

                    assetLimits.Add(assetLimit);
                }
            }
            return assetLimits;
        }

        public List<LimitStats> GetLimitStatsForAssets(List<Asset> assets, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<LimitStats> limitStats = null;
            if (assets != null)
            {
                // Get the list of unique limits for these assets
                List<long> limitIds = null;
                if (minCriticality.HasValue || maxCriticality.HasValue)
                {
                    // Criticality was specified in the input, so get just limits matching the specified criticality range
                    int lowerCriticality = minCriticality.HasValue ? minCriticality.Value : -1;
                    int upperCriticality = maxCriticality.HasValue ? maxCriticality.Value : 999;

                    limitIds = (from av in _assetVariableRepository.GetAllList()
                                join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                join a in assets on av.AssetId equals a.Id
                                where l.Level.Criticality >= lowerCriticality && l.Level.Criticality <= upperCriticality
                                select l.Id).Distinct().ToList();
                }
                else
                {
                    // Criticality was not specified in the input, so get all limits
                    limitIds = (from av in _assetVariableRepository.GetAllList()
                                join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                join a in assets on av.AssetId equals a.Id
                                select l.Id).Distinct().ToList();
                }

                // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                if (limitIds != null && limitIds.Count > 0)
                    limitStats = _iowManager.GetPerLimitStatsOverTime(limitIds, startTimestamp, endTimestamp);
            }
            return limitStats;
        }

        public List<IOWLimit> GetProblematicLimitsForAsset(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp, int? minCriticality, int? maxCriticality)
        {
            List<IOWLimit> limits = new List<IOWLimit>();
            List<Asset> assets = new List<Asset>();

            // Maximum criticality? If provided, use the value in the argument list. If not provided, get everything;
            int maxCrit = maxCriticality.HasValue ? maxCriticality.Value : 9999;

            // How far back to look? If provided, use the starting time in the argument list. If not, use midnight at yesterday.
            DateTime startDay = startTimestamp.HasValue ? startTimestamp.Value : DateTime.Now.Date.AddDays(-1);
            double hoursBack = (DateTime.Now - startDay).TotalHours;

            // Look for one asset. If the asset in the input list isn't valid, get all assets that do not have a parent.
            Asset asset = _assetManager.GetAsset(assetId, assetName);
            if( asset != null )
                assets.Add(asset);
            else
                assets = _assetManager.GetAssetChildren(null, null, false);

            if( assets != null && assets.Count > 0 )
            {
                // Get list of variables for the specified asset (or assets)
                var query = from a in assets
                            join av in GetAssetVariableList() on a.Id equals av.AssetId
                            orderby av.IOWVariable.Name
                            select av.IOWVariable.Id;
                var variableIds = query.Distinct().ToList();

                // Get problematic limits for our list of variables
                limits = _iowManager.GetProblematicLimits(variableIds, maxCrit, hoursBack);
            }
            return limits;
        }


        public List<AssetTypeMetricValue> GetAssetHealthMetricValues()
        {
            List<AssetTypeMetricValue> output = new List<AssetTypeMetricValue>();

            // Get all the metrics, and process them individually
            List<HealthMetric> metrics = GetHealthMetricList();
            if (metrics != null && metrics.Count > 0)
            {
                foreach (HealthMetric m in metrics)
                {

                    // Get a list of unique limits for the specified asset type and other dimensions
                    List<long> limitIds = null;
                    if( m.ApplyToEachAsset )
                    {
                        // Get metrics for each asset of the specified asset type
                        List<Asset> assets = _assetManager.GetAssetListForType(m.AssetTypeId);
                        if( assets != null && assets.Count > 0 )
                        {
                            foreach( Asset a in assets )
                            {
                                limitIds = (from av in _assetVariableRepository.GetAllList()
                                    join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                    where av.Asset.Id == a.Id && l.Level.Name == m.Level.Name
                                    select l.Id).Distinct().ToList();

                                AssetTypeMetricValue n = new AssetTypeMetricValue
                                {
                                    Id = m.Id,
                                    Name = m.Name,
                                    AssetTypeId = m.AssetTypeId,
                                    AssetTypeName = m.AssetType.Name,
                                    ApplyToEachAsset = m.ApplyToEachAsset,
                                    AssetId = a.Id,
                                    AssetName = a.Name,
                                    LevelId = m.IOWLevelId,
                                    LevelName = m.Level.Name,
                                    Criticality = m.Level.Criticality,
                                    MetricType = m.MetricType,
                                    GoodDirection = m.GoodDirection,
                                    Period = m.Period,
                                    WarningLevel = m.WarningLevel,
                                    ErrorLevel = m.ErrorLevel,
                                    Value = 0,
                                    RecentValue = 0,
                                    StartTimestamp = DateTime.Now.Date.AddDays(-m.Period),
                                    EndTimestamp = DateTime.Now,
                                    NumberLimits = 0,
                                    Order = m.Order
                                };
                                n.DurationHours = (n.EndTimestamp - n.StartTimestamp).TotalHours;

                                // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                                if (limitIds != null && limitIds.Count > 0)
                                {
                                    List<LevelStats> levelStats = _iowManager.GetPerLevelStatsOverTime(limitIds, n.StartTimestamp, n.EndTimestamp);

                                    // There should be just one member in the array (or zero), since there was only one level in the input and the above function groups by level name.
                                    if (levelStats != null && levelStats.Count > 0 && levelStats[0].NumberLimits > 0)
                                    {
                                        if (n.MetricType == MetricType.PercentLimitsInDeviation)
                                            n.Value = levelStats[0].NumberDeviatingLimits / levelStats[0].NumberLimits * 100;
                                        else if (n.MetricType == MetricType.PercentTimeInDeviation)
                                            n.Value = levelStats[0].DurationHours / n.DurationHours / levelStats[0].NumberLimits * 100;
                                        n.NumberLimits = (int)levelStats[0].NumberLimits;
                                    }

                                    // Repeat for recent values, defined as since midnight yesterday
                                    DateTime yesterday = DateTime.Now.Date.AddDays(-1);
                                    levelStats = _iowManager.GetPerLevelStatsOverTime(limitIds, yesterday, n.EndTimestamp);
                                    if (levelStats != null && levelStats.Count > 0 && levelStats[0].NumberLimits > 0)
                                    {
                                        if (n.MetricType == MetricType.PercentLimitsInDeviation)
                                            n.RecentValue = levelStats[0].NumberDeviatingLimits / levelStats[0].NumberLimits * 100;
                                        else if (n.MetricType == MetricType.PercentTimeInDeviation)
                                            n.RecentValue = levelStats[0].DurationHours / (n.EndTimestamp - yesterday).TotalHours / levelStats[0].NumberLimits * 100;
                                    }
                                }
                                output.Add(n);
                            }
                        }
                    }
                    else
                    {
                        // Get overall metrics for all assets of the specified asset type
                        limitIds = (from av in _assetVariableRepository.GetAllList()
                                           join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                           where av.Asset.AssetTypeId == m.AssetTypeId && l.Level.Name == m.Level.Name
                                           select l.Id).Distinct().ToList();

                        AssetTypeMetricValue n = new AssetTypeMetricValue
                        {
                            Id = m.Id,
                            Name = m.Name,
                            AssetTypeId = m.AssetTypeId,
                            AssetTypeName = m.AssetType.Name,
                            ApplyToEachAsset = m.ApplyToEachAsset,
                            AssetId = 0,
                            AssetName = "",
                            LevelId = m.IOWLevelId,
                            LevelName = m.Level.Name,
                            Criticality = m.Level.Criticality,
                            MetricType = m.MetricType,
                            GoodDirection = m.GoodDirection,
                            Period = m.Period,
                            WarningLevel = m.WarningLevel,
                            ErrorLevel = m.ErrorLevel,
                            Value = 0,
                            StartTimestamp = DateTime.Now.Date.AddDays(-m.Period),
                            EndTimestamp = DateTime.Now,
                            NumberLimits = 0,
                            Order = m.Order
                        };
                        n.DurationHours = (n.EndTimestamp - n.StartTimestamp).TotalHours;
                    
                        // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                        if (limitIds != null && limitIds.Count > 0)
                        {
                            List<LevelStats> levelStats = _iowManager.GetPerLevelStatsOverTime(limitIds, n.StartTimestamp, n.EndTimestamp);

                            // There should be just one member in the array (or zero), since there was only one level in the input and the above function groups by level name.
                            if( levelStats != null && levelStats.Count > 0 && levelStats[0].NumberLimits > 0 )
                            {
                                if (n.MetricType == MetricType.PercentLimitsInDeviation)
                                    n.Value = levelStats[0].NumberDeviatingLimits / levelStats[0].NumberLimits * 100;
                                else if (n.MetricType == MetricType.PercentTimeInDeviation)
                                    n.Value = levelStats[0].DurationHours / n.DurationHours / levelStats[0].NumberLimits * 100;
                                n.NumberLimits = (int) levelStats[0].NumberLimits;
                            }
                        }
                        output.Add(n);
                    }
                }
            }
            return output;
        }
    }
}
