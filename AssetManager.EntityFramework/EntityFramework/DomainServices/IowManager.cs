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
using AssetManager.Utilities;

namespace AssetManager.DomainServices
{
    public class IowManager : DomainService, IIowManager
    {
        private readonly IOWLevelRepository _iowLevelRepository;
        private readonly IOWVariableRepository _iowVariableRepository;
        private readonly IOWLimitRepository _iowLimitRespository;
        private readonly IOWDeviationRepository _iowDeviationRepository;
        private readonly TagManager _tagManager;

        // Validation information
        private static int minCriticality = 1;
        private static int maxCriticality = 5;

        public IowManager(
            IOWLevelRepository iowLevelRepository,
            IOWVariableRepository iowVariableRepository,
            IOWLimitRepository iowLimitRepository,
            IOWDeviationRepository iowDeviationRepository,
            TagManager tagManager
            )
        {
            _iowLevelRepository = iowLevelRepository;
            _iowVariableRepository = iowVariableRepository;
            _iowLimitRespository = iowLimitRepository;
            _iowDeviationRepository = iowDeviationRepository;
            _tagManager = tagManager;
        }

        public IOWLevel FirstOrDefaultLevel(long id)
        {
            return _iowLevelRepository.FirstOrDefault(id);
        }

        public IOWLevel FirstOrDefaultLevel(string name)
        {
            return _iowLevelRepository.FirstOrDefault(p => p.Name == name);
        }

        public IOWLevel FirstOrDefaultLevel(long? id, string name)
        {
            IOWLevel level = null;

            if (id.HasValue)
                level = _iowLevelRepository.FirstOrDefault(id.Value);

            else if( !string.IsNullOrEmpty(name) )
                level = _iowLevelRepository.FirstOrDefault(p => p.Name == name);

            return level;
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

        public bool DeleteLevel(long? id, string name)
        {
            bool success = false;

            if( id.HasValue )
                success = DeleteLevel(id.Value);

            else if( !string.IsNullOrEmpty(name) )
                success = DeleteLevel(name);

            return success;
        }

        public bool DeleteLevel(Expression<Func<IOWLevel, bool>> predicate)
        {
            IOWLevel level = _iowLevelRepository.FirstOrDefault(predicate);
            return DeleteLevel(level.Id);
        }

        public IOWLevel InsertOrUpdateLevel(IOWLevel input)
        {
            // Look to see if a level already exists. If so, fetch it.
            IOWLevel level = _iowLevelRepository.FirstOrDefault(input.Id);
            if( level == null )
                level = _iowLevelRepository.FirstOrDefault(p => p.Name == input.Name );

            if (level != null)
            {
                // Found a record. Use everything from the input except the Id and tenant.
                // level.TenantId = input.TenantId;
                level.Name = input.Name;
                level.Description = input.Description;
                level.Criticality = input.Criticality;
                level.ResponseGoal = input.ResponseGoal;
                level.MetricGoal = input.MetricGoal;
            }
            else
            {
                // Did not find a record. Use the input as is.
                level = input;
            }

            // Make sure criticality is in the proper range
            level.Criticality = level.Criticality.Clamp(minCriticality, maxCriticality);

            return _iowLevelRepository.InsertOrUpdate(level);
        }

        // Variable

        public IOWVariable FirstOrDefaultVariable(long id)
        {
            return _iowVariableRepository.FirstOrDefault(id);
        }

        public IOWVariable FirstOrDefaultVariable(string name)
        {
            return _iowVariableRepository.FirstOrDefault(p => p.Name == name);
        }

        public IOWVariable FirstOrDefaultVariable(long? id, string name)
        {
            IOWVariable variable = null;

            if (id.HasValue)
                variable = _iowVariableRepository.FirstOrDefault(id.Value);
            else if (!string.IsNullOrEmpty(name))
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == name);

            return variable;
        }

        public IOWVariable FirstOrDefaultVariable(Expression<Func<IOWVariable, bool>> predicate)
        {
            return _iowVariableRepository.FirstOrDefault(predicate);
        }

        public List<IOWVariable> GetAllVariables()
        {
            return _iowVariableRepository.GetAll().OrderBy(p => p.Name).ToList();
        }

        public List<IOWVariable> GetAllVariables(Expression<Func<IOWVariable, bool>> predicate)
        {
            return _iowVariableRepository.GetAll().Where(predicate).OrderBy(p => p.Name).ToList();
        }

        public bool DeleteVariable(long id)
        {
            bool success = false;

            IOWVariable variable = _iowVariableRepository.FirstOrDefault(id);
            if (variable != null)
            {
                // First delete any limits attached to this variable
                _iowLimitRespository.Delete(p => p.IOWVariableId == variable.Id);

                // Now delete the variable
                _iowVariableRepository.Delete(variable.Id);
                success = true;
            }
            else // Variable does not exist ==> call this a failure
                success = false;

            return success;
        }

        public bool DeleteVariable(string name)
        {
            IOWVariable variable = _iowVariableRepository.FirstOrDefault(p => p.Name == name);
            return DeleteVariable(variable.Id);
        }

        public bool DeleteVariable(long? id, string name)
        {
            bool success = false;

            if (id.HasValue)
                success = DeleteVariable(id.Value);

            else if (!string.IsNullOrEmpty(name))
                success = DeleteVariable(name);

            return success;
        }

        public IOWVariable InsertOrUpdateVariable(IOWVariable input)
        {
            IOWVariable variable = null;

            // Check to see if a variable already exists. If so, update it in place. Otherwise, create a new one.
            if (input.Id > 0)
                variable = _iowVariableRepository.FirstOrDefault(input.Id);
            else if (!string.IsNullOrEmpty(input.Name))
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            // No variable? Then create one.
            if( variable == null )
            {
                variable = new IOWVariable
                {
                    Name = input.Name,
                    IOWLimits = new List<IOWLimit>(),
                    TenantId = input.TenantId
                };
            }

            variable.Description = input.Description;
            // Validate the tag
            Tag tag = _tagManager.FirstOrDefaultTag(input.TagId);
            if( tag != null)
                variable.TagId = input.TagId;

            // Set the UOM to, in order: (1) input, (2) original variable, (3) tag.
            if (!string.IsNullOrEmpty(input.UOM))
                variable.UOM = input.UOM;
            else if (string.IsNullOrEmpty(variable.UOM) && tag != null)
                variable.UOM = tag.UOM;

            variable.IOWLimits = input.IOWLimits;

            return _iowVariableRepository.InsertOrUpdate(variable);
        }

        // Limits

        public IOWLimit FirstOrDefaultLimit(long id)
        {
            return _iowLimitRespository.FirstOrDefault(id);
        }

        public IOWLimit FirstOrDefaultLimit(long variableId, long levelId)
        {
            return _iowLimitRespository.FirstOrDefault(p => p.IOWVariableId == variableId && p.IOWLevelId == levelId);
        }

        public IOWLimit FirstOrDefaultLimit(string variableName, string levelName)
        {
            return _iowLimitRespository.FirstOrDefault(p => p.Variable.Name == variableName && p.Level.Name == levelName);
        }

        public IOWLimit FirstOrDefaultLimit(long variableId, long? levelId, string levelName)
        {
            if( levelId.HasValue )
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Id == variableId && p.Level.Id == levelId.Value);
            else
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Id == variableId && p.Level.Name == levelName);
        }

        public List<IOWLimit> GetAllLimits(long variableId)
        {
            return _iowLimitRespository.GetAll().Where(p => p.IOWVariableId == variableId).OrderBy(p => p.Level.Criticality).ThenBy(p => p.Level.Name).ToList();
        }

        public long InsertOrUpdateLimitAndGetId(IOWLimit input)
        {
            IOWLimit limit = null;

            // If the limit id is present, assume the limit exists.
            // Otherwise check to see if this limit exists.
            if (input.Id > 0)
                limit = FirstOrDefaultLimit(input.Id);
            else
                limit = FirstOrDefaultLimit(input.IOWVariableId, input.IOWLevelId);

            if (limit == null)
                limit = new IOWLimit
                {
                    TenantId = input.TenantId,
                    IOWVariableId = input.IOWVariableId,
                    IOWLevelId = input.IOWLevelId
                };

            limit.Cause = input.Cause;
            limit.Consequences = input.Consequences;
            limit.Action = input.Action;
            limit.LowLimit = input.LowLimit;
            limit.HighLimit = input.HighLimit;
            limit.LastCheckDate = input.LastCheckDate;
            limit.LastStatus = input.LastStatus;
            limit.LastDeviationStartDate = input.LastDeviationEndDate;
            limit.LastDeviationEndDate = input.LastDeviationEndDate;

            return _iowLimitRespository.InsertOrUpdateAndGetId(limit);
        }

        public bool DeleteLimit(long id)
        {
            _iowLimitRespository.Delete(id);
            return true;
        }


        // Deviations

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
