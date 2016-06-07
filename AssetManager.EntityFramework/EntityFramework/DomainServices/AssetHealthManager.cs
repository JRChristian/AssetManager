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

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetHealthManager : DomainService, IAssetHealthManager
    {
        private readonly IAssetVariableRepository _assetVariableRepository;
        private readonly IAssetManager _assetManager;
        private readonly IIowManager _iowManager;

        public AssetHealthManager(
            IAssetVariableRepository assetVariableRepository,
            IAssetManager assetManager,
            IIowManager iowManager
            )
        {
            _assetVariableRepository = assetVariableRepository;
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
                numberPeriods = (int)((DateTime.Now - startTimestamp).TotalHours/hoursInPeriod) + 1;
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
            for(int i=0; i < numberPeriods; i++)
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
            if( allLimits != null && allLimits.Count > 0 )
            {
                var lastLimit = allLimits[0];
                bool isFirstTimeThrough = true;
                AssetDeviationSummary oneItem = null;

                foreach (var limit in allLimits)
                {
                    // If this is the first time through OR the asset/level information changed, save the previous record (if any) and start a new one
                    if (isFirstTimeThrough || limit.AssetId != lastLimit.AssetId || limit.LevelName != lastLimit.LevelName )
                    {
                        if( !isFirstTimeThrough )
                            output.AssetDeviations.Add(oneItem);

                        oneItem = new AssetDeviationSummary
                        {
                            AssetId = limit.AssetId,
                            AssetName = limit.AssetName,
                            LevelName = limit.LevelName,
                            Criticality = limit.Criticality,
                            DeviationDetails = new List<DeviationDetails>()
                        };
                        foreach(var d in datetimes)
                        {
                            oneItem.DeviationDetails.Add(new DeviationDetails { Timestamp = d, DeviationCount = 0, DurationHours = 0 });
                        }
                    }

                    // Get deviations
                    List<IOWDeviation> deviations = _iowManager.GetDeviations(limit.LimitId, startTimestamp);
                    if( deviations != null && deviations.Count > 0 )
                    {
                        int i = 0;
                        foreach(IOWDeviation dev in deviations)
                        {
                            // Deviations are in ascending date order.
                            // Each entry in the output array represents a range from [i].Timestamp to [i+1].Timestamp, typically an hour, a day, or several days.
                            // Call [i].Timestamp the "index start" and [i+1].Timestamp the "index end".
                            // Move ahead in the output array until the time range overlaps the duration: until index end > deviation start
                            for (; i < numberPeriods && oneItem.DeviationDetails[i].Timestamp.AddHours(hoursInPeriod) < dev.StartTimestamp; i++) ;

                            // Continue moving ahead in the output array, adding the deviation to the output array records until we reach the end of the deviation.
                            for( ; i < numberPeriods && oneItem.DeviationDetails[i].Timestamp <= (dev.EndTimestamp ?? DateTime.Now); i++ )
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

        public List<AssetLimitStatsByDay> GetAssetLimitStatsByDay(long? assetId, string assetName, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<AssetLimitStatsByDay> assetLimits = new List<AssetLimitStatsByDay>();

            // Get all top level children of the specified asset. Add the parent asset (specified in the arguments) to the list as well.
            List<Asset> assets = _assetManager.GetAssetChildren(assetId, assetName, true);

            if( assets != null )
            {
                foreach( Asset asset in assets )
                {
                    AssetLimitStatsByDay assetLimit = new AssetLimitStatsByDay { AssetId = asset.Id, AssetName = asset.Name, Limits = new List<LimitStatsByDay>() };

                    // Get the list of unique limits for this asset
                    List<long> limitIds = (from av in _assetVariableRepository.GetAllList()
                                join l in _iowManager.GetAllLimits() on av.IOWVariableId equals l.IOWVariableId
                                where av.AssetId == asset.Id
                                select l.Id).Distinct().ToList();

                    // Get the stats for these limits and add them to the output. Group statistics by level name and criticality. (This does something different only if this asset has multiple variables.)
                    if ( limitIds != null )
                    {
                        List<LimitStatsByDay> stats = _iowManager.GetLimitStatsByDayGroupByLevel(limitIds, startTimestamp, endTimestamp);
                        if( stats != null)
                        {
                            foreach( LimitStatsByDay s in stats)
                                assetLimit.Limits.Add(s);
                        }
                    }
                    assetLimits.Add(assetLimit);
                }
            }
            return assetLimits;
        }
    }
}
