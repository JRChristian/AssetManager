using Abp.Domain.Services;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace AssetManager.DomainServices
{
    public class IowManager : DomainService, IIowManager
    {
        private readonly IOWLevelRepository _iowLevelRepository;
        private readonly IOWVariableRepository _iowVariableRepository;
        private readonly IOWDeviationRepository _iowDeviationRepository;

        private static int minCriticality = 1;
        private static int maxCriticality = 5;

        public IowManager(
            IOWLevelRepository iowLevelRepository,
            IOWVariableRepository iowVariableRepository,
            IOWDeviationRepository iowDeviationRepository
            )
        {
            _iowLevelRepository = iowLevelRepository;
            _iowVariableRepository = iowVariableRepository;
            _iowDeviationRepository = iowDeviationRepository;
        }

        public IOWLevel FirstOrDefaultLevel(long id)
        {
            return _iowLevelRepository.FirstOrDefault(id);
        }

        public IOWLevel FirstOrDefaultLevel(string name)
        {
            return _iowLevelRepository.FirstOrDefault(p => p.Name == name);
        }

        public IOWLevel FirstOrDefaultLevel(Expression<Func<IOWLevel, bool>> predicate)
        {
            return _iowLevelRepository.FirstOrDefault(predicate);
        }

        public List<IOWLevel> GetAllLevels()
        {
            return _iowLevelRepository.GetAll().OrderBy(p => p.Criticality).ToList();
        }

        public bool DeleteLevel(long id)
        {
            bool success = false;

            IOWLevel level = _iowLevelRepository.FirstOrDefault(id);
            if (level != null)
            {
                if (level.IOWLimits.Count == 0)
                {
                    // Limit exists and is not used ==> okay to delete
                    _iowLevelRepository.Delete(id);
                    success = true;
                }
                else // Limit exists and is used ==> do not allow it to be deleted
                    success = false;
            }
            else // Limit does not exists ==> call this a failure
                success = false;

            return success;
        }

        public bool DeleteLevel(string name)
        {
            IOWLevel level = _iowLevelRepository.FirstOrDefault(p => p.Name == name);
            return DeleteLevel(level.Id);
        }

        public bool DeleteLevel(Expression<Func<IOWLevel, bool>> predicate)
        {
            IOWLevel level = _iowLevelRepository.FirstOrDefault(predicate);
            return DeleteLevel(level.Id);

        }

        public IOWLevel InsertOrUpdateLevel(IOWLevel input)
        {
            // Make sure criticality is in the proper range
            if (input.Criticality < minCriticality)
                input.Criticality = minCriticality;
            if (input.Criticality > maxCriticality)
                input.Criticality = maxCriticality;

            return _iowLevelRepository.InsertOrUpdate(input);
        }

        public List<IOWDeviation> DetectDeviations(TagDataRaw tagdata)
        {
            List<IOWDeviation> output = new List<IOWDeviation>();

            // Only consider good data
            if ( tagdata.Quality == TagDataQuality.Good )
            {

                // Get all variables associated with this tag
                List<IOWVariable> variables = _iowVariableRepository.GetAllList(t => t.TagId == tagdata.TagId);

                foreach(IOWVariable v in variables )
                {
                    foreach( IOWLimit limit in v.IOWLimits )
                    {
                        double? lowLimit = null, highLimit = null;
                        if (limit.HighLimit.HasValue)
                            highLimit = limit.HighLimit.Value;
                        if (limit.LowLimit.HasValue)
                            lowLimit = limit.LowLimit.Value;

                        // Do we already have a deviation record?
                        // Find a deviation record (if one exists) matching the current limit AND 
                        // where the time period of the deviation record overlaps the current tag value.
                        IOWDeviation deviation = _iowDeviationRepository.FirstOrDefault(x => 
                            x.IOWLimitId == limit.Id && x.StartDate < tagdata.Timestamp && 
                                (!x.EndDate.HasValue || x.EndDate.Value > tagdata.Timestamp));

                        if( deviation != null )
                        {
                            // Found an old record, so update it as necessary. May need to add an additional record.
                            double worstValue = deviation.WorstValue.HasValue ? deviation.WorstValue.Value : tagdata.Value;

                            // If the old record has the same deviation type (low or high) as the input
                            // then update the old record in place. The start and end times should not change, since either the
                            // end time is missing (for an ongoing deviation) or the end time is later than our current time.
                            if( WhatDeviationType(tagdata.Value, lowLimit, highLimit) == WhatDeviationType(worstValue, lowLimit, highLimit) )
                            {
                                deviation.LowLimit = lowLimit;
                                deviation.HighLimit = highLimit;
                                if (WhatDeviationType(tagdata.Value, lowLimit, highLimit) == DeviationType.Low)
                                    deviation.WorstValue = Math.Min(tagdata.Value, worstValue);
                                else
                                    deviation.WorstValue = Math.Max(tagdata.Value, worstValue);

                                output.Add(deviation);
                            }

                            // The old record has a different deviation type (low or high) as the input, so close the old record
                            // and create a new one. This handles the case where the new data is later than anything already processed,
                            // so the status is swinging from low to high (or vice versa) and we need to close the "low" record and
                            // create a new "high" record. There is the possibility, not handled here, that the new data are showing up
                            // out of order. Suppose a new "low" value shows up in the midst of a "high" deviation. We need to split
                            // the old deviation record. The new situation could be any of the following: high/low/high or high/low.
                            // We can't tell these cases apart with the information at hand--we need more raw data just before and just
                            // after the data passed as input. A problem to be solved later. For now, assume that the high/low case.
                            else
                            {
                                deviation.EndDate = tagdata.Timestamp.AddMinutes(-1);
                                output.Add(deviation);

                                deviation = new IOWDeviation
                                {
                                    TenantId = v.TenantId,
                                    IOWLimitId = limit.Id,
                                    StartDate = tagdata.Timestamp,
                                    //EndDate = tagdata.Timestamp, // No end time
                                    LowLimit = lowLimit,
                                    HighLimit = highLimit,
                                    WorstValue = tagdata.Value
                                };
                                output.Add(deviation);
                            }
                        } // if( deviation != null )
                        else // if( deviation == null )
                        {
                            // Didn't find an old record. 
                            // Check for a deviation in the input data and create a new record if necessary.
                            if ( IsDeviation(tagdata.Value, lowLimit, highLimit ) )
                            {
                                deviation = new IOWDeviation
                                {
                                    TenantId = v.TenantId,
                                    IOWLimitId = limit.Id,
                                    StartDate = tagdata.Timestamp,
                                    //EndDate = tagdata.Timestamp, // No end time
                                    LowLimit = lowLimit,
                                    HighLimit = highLimit,
                                    WorstValue = tagdata.Value
                                };
                                output.Add(deviation);
                            }
                        } // if( deviation == null )
                    }  // foreach( IOWLimit limit in v.IOWLimits )
                } // foreach(IOWVariable v in variables )
            } //if ( tagdata.Quality == TagDataQuality.Good )

            return output;
        }

        private enum DeviationType { None, Low, High }

        private DeviationType WhatDeviationType( double value, double? low, double? high)
        {
            if (low.HasValue && value < low.Value)
                return DeviationType.Low;
            else if (high.HasValue && value > high.Value)
                return DeviationType.High;
            else return DeviationType.None;
        }

        private bool IsDeviation(double value, double? low, double? high)
        {
            return (low.HasValue && value < low.Value) || (high.HasValue && value > high.Value);
        }
    }
}
