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

            limit.StartDate = input.StartDate;
            limit.EndDate = input.EndDate;
            limit.Direction = input.Direction;
            limit.Value = input.Value;
            limit.Cause = input.Cause;
            limit.Consequences = input.Consequences;
            limit.Action = input.Action;
            limit.LastCheckDate = input.LastCheckDate;
            limit.LastStatus = input.LastStatus;
            limit.LastDeviationStartTimestamp = input.LastDeviationStartTimestamp;
            limit.LastDeviationEndTimestamp = input.LastDeviationEndTimestamp;

            return _iowLimitRespository.InsertOrUpdateAndGetId(limit);
        }

        public bool DeleteLimit(long id)
        {
            _iowLimitRespository.Delete(id);
            return true;
        }


        // Deviations

        public void DetectDeviations(Tag tag, DateTime startTimestamp, DateTime? endTimestampx)
        {
            DateTime lastCheckDate = DateTime.Now;

            // If an end time isn't specified, look as far as one hour ahead of now.
            DateTime endTimestamp = endTimestampx.HasValue ? endTimestampx.Value : DateTime.Now.AddHours(1);

            // Get all the data for the specified tag in the specified time range. Only consider good data.
            List<TagDataRaw> tagdata = _tagManager.GetAllListData(p => p.TagId == tag.Id && p.Timestamp >= startTimestamp && p.Timestamp <= endTimestamp && p.Quality == TagDataQuality.Good);

            // Continue only if we found some data.
            if (tagdata != null)
            {
                // Get all variables associated with this tag
                List<IOWVariable> variables = _iowVariableRepository.GetAllList(t => t.TagId == tag.Id);

                foreach (IOWVariable v in variables)
                {
                    foreach (IOWLimit limit in v.IOWLimits)
                    {
                        IOWDeviation deviation = null;
                        bool isFirstData = true;

                        foreach (TagDataRaw data in tagdata)
                        {
                            // There are several cases here.
                            //  1. A deviation already exists. With new data:
                            //     A. Still a deviation ==> update the existing deviation
                            //     B. Deviation is over ==> update the existing deviation and close the record
                            //  2. There isn't an existing deviation. WIth new data:
                            //     A. A deviation has started ==> insert a new one
                            //     B. No deviation ==> do nothing

                            // Do we have a deviation with this particular tag record?
                            Direction newDirection = WhatDeviationType(limit.Direction, limit.Value, data.Value);
                            bool isDeviation = IsDeviation(limit.Direction, limit.Value, data.Value);

                            // For the first value in the data array, check so see if we already have a deviation record.
                            // Find a deviation record (if one exists) matching the current limit AND 
                            // where the time period of the deviation record overlaps the current tag value.
                            // No need to do this after the first value in the tag data collection.
                            if( isFirstData )
                            {
                                deviation = _iowDeviationRepository.FirstOrDefault(x =>
                                    x.IOWLimitId == limit.Id && x.StartTimestamp < data.Timestamp &&
                                    (!x.EndTimestamp.HasValue || x.EndTimestamp.Value > data.Timestamp));
                                isFirstData = false;
                            }

                            if (deviation != null)
                            {
                                // Case 1 - A deviation already exists
                                if (isDeviation)
                                {
                                    // Case 1A - The existing deviation continues ==> update the new record and leave it open
                                    // No need to update timestamps, since the current time must fall within the deviation time range OR 
                                    // there isn't an end time to the existing deviation.
                                    if (deviation.Direction == Direction.Low)
                                    {
                                        deviation.LimitValue = Math.Min(deviation.LimitValue, limit.Value);
                                        deviation.WorstValue = Math.Min(deviation.WorstValue, data.Value);
                                    }
                                    else if (deviation.Direction == Direction.High)
                                    {
                                        deviation.LimitValue = Math.Max(deviation.LimitValue, limit.Value);
                                        deviation.WorstValue = Math.Max(deviation.WorstValue, data.Value);
                                    }
                                    _iowDeviationRepository.Update(deviation);

                                    // No updates needed to the overall limit record
                                }
                                else
                                {
                                    // Case 1B - The existing deviation is over ==> update and close the old record
                                    deviation.EndTimestamp = data.Timestamp.AddMinutes(-1);
                                    _iowDeviationRepository.Update(deviation);
                                    deviation = null;

                                    // Update the overall limit record -- maybe (not if the overall record describes a future deviation)
                                    if( limit.LastDeviationStartTimestamp < data.Timestamp )
                                    {
                                        limit.LastStatus = IOWStatus.Normal;
                                        limit.LastDeviationEndTimestamp = data.Timestamp.AddMinutes(-1);
                                    }
                                }
                            } // if( deviation != null )
                            else // if( deviation == null )
                            {
                                // Case 2 - There isn't already a deviation
                                if (isDeviation)
                                {
                                    // Case 2A - A new deviation has started ==> insert a new record
                                    deviation = new IOWDeviation
                                    {
                                        TenantId = v.TenantId,
                                        IOWLimitId = limit.Id,
                                        StartTimestamp = data.Timestamp,
                                        //EndTimestamp = tagdata.Timestamp, // No end time
                                        Direction = newDirection,
                                        LimitValue = limit.Value,
                                        WorstValue = data.Value,
                                    };

                                    // Insert the new record. We may need to update it later.
                                    long Id = _iowDeviationRepository.InsertAndGetId(deviation);
                                    deviation.Id = Id;

                                    // Update the overall limit record -- maybe
                                    if( !limit.LastDeviationStartTimestamp.HasValue || limit.LastDeviationStartTimestamp.Value < data.Timestamp )
                                    {
                                        limit.LastStatus = IOWStatus.OpenDeviation;
                                        limit.LastDeviationStartTimestamp = data.Timestamp;
                                        limit.LastDeviationEndTimestamp = null;
                                    }
                                }
                                else
                                {
                                    // Case 2B - there wasn't a deviation and still isn't one ==> do nothing

                                    // No updates needed to the overall limit record
                                }

                            } // if( deviation == null )
                        } // foreach (TagDataRaw data in tagdata)

                        // Okay, we processed all the data for this limit. Update the overall limit information.
                        limit.LastCheckDate = lastCheckDate;
                        _iowLimitRespository.Update(limit);

                    }  // foreach( IOWLimit limit in v.IOWLimits )
                } // foreach(IOWVariable v in variables )
            } // if( tagdata != null )
            return;
        }

        private bool IsDeviation(Direction LimitDirection, double LimitValue, double Value)
        {
            if ((LimitDirection == Direction.Low && Value < LimitValue) ||
                (LimitDirection == Direction.High && Value > LimitValue))
                return true;
            else
                return false;
        }

        private Direction WhatDeviationType(Direction LimitDirection, double LimitValue, double Value )
        {
            Direction output = Direction.None;

            if (LimitDirection == Direction.Low && Value < LimitValue)
                output = Direction.Low;
            else if (LimitDirection == Direction.High && Value > LimitValue)
                output = Direction.High;

            return output;
        }
    }
}
