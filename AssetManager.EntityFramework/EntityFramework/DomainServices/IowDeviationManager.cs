using Abp.Domain.Services;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public class IowDeviationManager : DomainService
    {
        private readonly IOWVariableRepository _iowVariableRepository;
        private readonly IOWDeviationRepository _iowDeviationRepository;

        public IowDeviationManager(
            IOWVariableRepository iowVariableRepository,
            IOWDeviationRepository iowDeviationRepository
            )
        {
            _iowVariableRepository = iowVariableRepository;
            _iowDeviationRepository = iowDeviationRepository;
        }

        public List<IOWDeviation> DetectDeviations(TagDataRaw tagdata)
        {
            List<IOWDeviation> output = null;

            // Only consider good data
            if ( tagdata.Quality == TagDataQuality.Good )
            {

                // Get all variables associated with this tag
                List<IOWVariable> variables = _iowVariableRepository.GetAllList(t => t.Id == tagdata.TagId);

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
