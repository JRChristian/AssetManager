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
using System.Data.Entity.SqlServer;

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

        public IOWLimit FirstOrDefaultLimit(long? variableId, string VariableName, long? levelId, string levelName)
        {
            if (variableId.HasValue && levelId.HasValue)
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Id == variableId && p.Level.Id == levelId.Value);
            else if( variableId.HasValue )
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Id == variableId && p.Level.Name == levelName);
            else if (levelId.HasValue)
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Name == VariableName && p.Level.Id == levelId.Value);
            else
                return _iowLimitRespository.FirstOrDefault(p => p.Variable.Name == VariableName && p.Level.Name == levelName);
        }

        public List<IOWLimit> GetAllLimits(long variableId)
        {
            return GetAllLimits(variableId, null, null, null);
        }

        public List<IOWLimit> GetAllLimits(string variableName)
        {
            return GetAllLimits(null, variableName, null, null);
        }
        public List<IOWLimit> GetAllLimits(string variableName, string levelName)
        {
            return GetAllLimits(null, variableName, null, levelName);
        }

        public List<IOWLimit> GetAllLimits(long? variableId, string variableName, long? levelId, string levelName)
        {
            List<IOWLimit> output = null;

            if ( variableId.HasValue && levelId.HasValue ) // Case 1: variable id and level id ==> high/low information for specified variable and level
                output = _iowLimitRespository.GetAll().Where(p => p.Variable.Id == variableId.Value && p.Level.Id == levelId.Value).OrderBy(p => p.Direction).ToList();

            else if (!string.IsNullOrEmpty(variableName) && levelId.HasValue) // Case 2: variable name and level id ==> high/low information for specified variable and level
                output = _iowLimitRespository.GetAll().Where(p => p.Variable.Name == variableName && p.Level.Id == levelId.Value).OrderBy(p => p.Direction).ToList();

            else if (variableId.HasValue && !string.IsNullOrEmpty(levelName)) // Case 3: variable id and level name ==> high/low information for specified variable and level
                output = _iowLimitRespository.GetAll().Where(p => p.Variable.Id == variableId.Value && p.Level.Name == levelName).OrderBy(p => p.Direction).ToList();

            else if (!string.IsNullOrEmpty(variableName) && !string.IsNullOrEmpty(levelName)) // Case 4: variable name and level name ==> high/low information for specified variable and level
                output = _iowLimitRespository.GetAll().Where(p => p.Variable.Name == variableName && p.Level.Name == levelName).OrderBy(p => p.Direction).ToList();

            else if (!string.IsNullOrEmpty(variableName)) // Case 5: variable name but nothing on levels ==> all levels for specified variable
                output = _iowLimitRespository.GetAll().Where(p => p.Variable.Name == variableName).OrderBy(p => p.Level.Criticality).ThenBy(p => p.Level.Name).ThenBy(p => p.Direction).ToList();

            else if (!string.IsNullOrEmpty(levelName)) // Case 6: level name but nothing on variables ==> all variables for specified level
                output = _iowLimitRespository.GetAll().Where(p => p.Level.Name == levelName).OrderBy(p => p.Variable.Name).ThenBy(p => p.Direction).ToList();

            else // Case 7: neither variable nor level specified ==> all variables and all levels
                output = _iowLimitRespository.GetAll().OrderBy(p => p.Variable.Name).ThenBy(p => p.Level.Criticality).ThenBy(p => p.Level.Name).ThenBy(p => p.Direction).ToList();

            return output;
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

        public List<IOWDeviation> GetLimitDeviations(long limitId)
        {
            return _iowDeviationRepository.GetAllList(p => p.IOWLimitId == limitId).OrderBy(p => p.StartTimestamp).ToList();
        }
        

        public List<VariableDeviation> GetDeviationSummary(bool includeAllVariables, int maxCriticality, double hoursBack)
        {
            List<VariableDeviation> output = new List<VariableDeviation>();
            DateTime earliestTimestamp = DateTime.Now.AddHours(-hoursBack);

            if ( includeAllVariables && maxCriticality > 0)
            {
                var deviations = _iowDeviationRepository.GetAll()
                    .Where(t => SqlFunctions.DateDiff("hour", (t.EndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack && t.IOWLimits.Level.Criticality <= maxCriticality)
                    .GroupBy(a => new
                    {
                        LimitId = a.IOWLimitId,
                    })
                    .Select(b => new
                    {
                        LimitId = b.Key.LimitId,
                        DeviationCount = b.Count(),
                        DurationHours = b.Sum(d => SqlFunctions.DateDiff("second", (d.StartTimestamp > earliestTimestamp ? d.StartTimestamp : earliestTimestamp), (d.EndTimestamp ?? DateTime.Now))) / 3600.0
                    })
                    .ToList();

                var query =
                    from lim in _iowLimitRespository.GetAllList(c => c.Level.Criticality <= maxCriticality)
                    from dev in deviations
                        .Where(dev => dev.LimitId == lim.Id)
                        .DefaultIfEmpty()
                    select new
                    {
                        VariableId = lim.Variable.Id,
                        VariableName = lim.Variable.Name,
                        TagId = lim.Variable.TagId,
                        TagName = lim.Variable.Tag.Name,
                        UOM = lim.Variable.UOM,
                        LimitId = lim.Id,
                        LevelName = lim.Level.Name,
                        Criticality = lim.Level.Criticality,
                        Direction = lim.Direction,
                        DeviationCount = dev != null ? dev.DeviationCount : 0,
                        DurationHours = dev != null ? dev.DurationHours : 0
                    };
                var results = query.ToList();
                foreach( var r in results )
                    output.Add(new VariableDeviation { VariableId=r.VariableId, VariableName=r.VariableName, TagId=r.TagId, TagName=r.TagName, UOM=r.UOM, LimitId=r.LimitId, LevelName=r.LevelName, Criticality=r.Criticality, Direction=r.Direction, DeviationCount=r.DeviationCount, DurationHours=r.DurationHours.HasValue ? r.DurationHours.Value : 0 });
            }
            else if( includeAllVariables && maxCriticality <= 0 )
            {
                var deviations = _iowDeviationRepository.GetAll()
                    .Where(t => SqlFunctions.DateDiff("hour", (t.EndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack)
                    .GroupBy(a => new
                    {
                        LimitId = a.IOWLimitId,
                    })
                    .Select(b => new
                    {
                        LimitId = b.Key.LimitId,
                        DeviationCount = b.Count(),
                        DurationHours = b.Sum(d => SqlFunctions.DateDiff("second", (d.StartTimestamp > earliestTimestamp ? d.StartTimestamp : earliestTimestamp), (d.EndTimestamp ?? DateTime.Now))) / 3600.0
                    })
                    .ToList();

                var query =
                    from lim in _iowLimitRespository.GetAllList()
                    from dev in deviations
                        .Where(dev => dev.LimitId == lim.Id)
                        .DefaultIfEmpty()
                    select new
                    {
                        VariableId = lim.Variable.Id,
                        VariableName = lim.Variable.Name,
                        TagId = lim.Variable.TagId,
                        TagName = lim.Variable.Tag.Name,
                        UOM = lim.Variable.UOM,
                        LimitId = lim.Id,
                        LevelName = lim.Level.Name,
                        Criticality = lim.Level.Criticality,
                        Direction = lim.Direction,
                        DeviationCount = dev != null ? dev.DeviationCount : 0,
                        DurationHours = dev != null ? dev.DurationHours : 0
                    };
                var results = query.ToList();
                foreach (var r in results)
                    output.Add(new VariableDeviation { VariableId = r.VariableId, VariableName = r.VariableName, TagId = r.TagId, TagName = r.TagName, UOM = r.UOM, LimitId = r.LimitId, LevelName = r.LevelName, Criticality = r.Criticality, Direction = r.Direction, DeviationCount = r.DeviationCount, DurationHours = r.DurationHours.HasValue ? r.DurationHours.Value : 0 });
            }
            else if (! includeAllVariables && maxCriticality > 0)
            {
                var deviations = _iowDeviationRepository.GetAll()
                    .Where(t => SqlFunctions.DateDiff("hour", (t.EndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack && t.IOWLimits.Level.Criticality <= maxCriticality)
                    .GroupBy(a => new
                    {
                        VariableId = a.IOWLimits.Variable.Id,
                        VariableName = a.IOWLimits.Variable.Name,
                        TagId = a.IOWLimits.Variable.TagId,
                        TagName = a.IOWLimits.Variable.Tag.Name,
                        UOM = a.IOWLimits.Variable.UOM,
                        LimitId = a.IOWLimitId,
                        LevelName = a.IOWLimits.Level.Name,
                        Criticality = a.IOWLimits.Level.Criticality,
                        Direction = a.Direction
                    })
                    .Select(b => new
                    {
                        VariableId = b.Key.VariableId,
                        VariableName = b.Key.VariableName,
                        TagId = b.Key.TagId,
                        TagName = b.Key.TagName,
                        UOM = b.Key.UOM,
                        LimitId = b.Key.LimitId,
                        LevelName = b.Key.LevelName,
                        Criticality = b.Key.Criticality,
                        Direction = b.Key.Direction,
                        DeviationCount = b.Count(),
                        DurationHours = b.Sum(d => SqlFunctions.DateDiff("second", (d.StartTimestamp > earliestTimestamp ? d.StartTimestamp : earliestTimestamp), (d.EndTimestamp ?? DateTime.Now))) / 3600.0
                    })
                    .ToList();
                foreach (var r in deviations)
                    output.Add(new VariableDeviation { VariableId = r.VariableId, VariableName = r.VariableName, TagId = r.TagId, TagName = r.TagName, UOM = r.UOM, LimitId = r.LimitId, LevelName = r.LevelName, Criticality = r.Criticality, Direction = r.Direction, DeviationCount = r.DeviationCount, DurationHours = r.DurationHours.HasValue ? r.DurationHours.Value : 0 });
            }
            else // !includeAllVariables && maxCriticality <= 0
            {
                var deviations = _iowDeviationRepository.GetAll()
                    .Where(t => SqlFunctions.DateDiff("hour", (t.EndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack)
                    .GroupBy(a => new
                    {
                        VariableId = a.IOWLimits.Variable.Id,
                        VariableName = a.IOWLimits.Variable.Name,
                        TagId = a.IOWLimits.Variable.TagId,
                        TagName = a.IOWLimits.Variable.Tag.Name,
                        UOM = a.IOWLimits.Variable.UOM,
                        LimitId = a.IOWLimitId,
                        LevelName = a.IOWLimits.Level.Name,
                        Criticality = a.IOWLimits.Level.Criticality,
                        Direction = a.Direction
                    })
                    .Select(b => new
                    {
                        VariableId = b.Key.VariableId,
                        VariableName = b.Key.VariableName,
                        TagId = b.Key.TagId,
                        TagName = b.Key.TagName,
                        UOM = b.Key.UOM,
                        LimitId = b.Key.LimitId,
                        LevelName = b.Key.LevelName,
                        Criticality = b.Key.Criticality,
                        Direction = b.Key.Direction,
                        DeviationCount = b.Count(),
                        DurationHours = b.Sum(d => SqlFunctions.DateDiff("second", (d.StartTimestamp > earliestTimestamp ? d.StartTimestamp : earliestTimestamp), (d.EndTimestamp ?? DateTime.Now))) / 3600.0
                    })
                    .ToList();
                foreach (var r in deviations)
                    output.Add(new VariableDeviation { VariableId = r.VariableId, VariableName = r.VariableName, TagId = r.TagId, TagName = r.TagName, UOM = r.UOM, LimitId = r.LimitId, LevelName = r.LevelName, Criticality = r.Criticality, Direction = r.Direction, DeviationCount = r.DeviationCount, DurationHours = r.DurationHours.HasValue ? r.DurationHours.Value : 0 });
            }
            /*
            var query = 
                from lim in _iowLimitRespository.GetAllList()
                join dev in _iowDeviationRepository.GetAllList(t => SqlFunctions.DateDiff("hour", (t.EndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack)
                    on lim.Id equals dev.IOWLimitId into all
                from dev in all.DefaultIfEmpty() 
                group all by new
                {
                    VariableId = lim.Variable.Id,
                    VariableName = lim.Variable.Name,
                    LimitId = lim.Id,
                    LevelName = lim.Level.Name,
                    Criticality = lim.Level.Criticality,
                    Direction = lim.Direction
                } into g
                select new
                {
                    VariableId = g.Key.VariableId,
                    VariableName = g.Key.VariableName,
                    LimitId = g.Key.LimitId,
                    LevelName = g.Key.LevelName,
                    Criticality = g.Key.Criticality,
                    Direction = g.Key.Direction,
                    DeviationCount = g.Count(),
                    DeviationHours = g.Sum(d => SqlFunctions.DateDiff("second", d.StartTimestamp, (d.EndTimestamp ?? DateTime.Now))) / 3600.0
                }
                //orderby g.VariableName ascending, g.Criticality ascending, g.Direction descending, g.LevelName ascending
                ;*/

            return output;
        }


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
                                    (!x.EndTimestamp.HasValue || x.EndTimestamp.Value >= data.Timestamp));
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
                                    if( !limit.LastDeviationStartTimestamp.HasValue || limit.LastDeviationStartTimestamp.Value < data.Timestamp )
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

                                    // Update the overall limit record -- maybe (not if the overall record describes a future deviation)
                                    if ( !limit.LastDeviationStartTimestamp.HasValue || limit.LastDeviationStartTimestamp.Value < data.Timestamp )
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

        public void ResetLastDeviationStatus()
        {
            // Loop through all limits in the system
            List<IOWLimit> allLimits = _iowLimitRespository.GetAllList();
            foreach( IOWLimit limit in allLimits )
            {
                if( limit.IOWDeviations.Count > 0 )
                {
                    // There is at least one deviation. Get the latest
                    DateTime latestStartTimestamp = _iowDeviationRepository.GetAllList(p => p.IOWLimitId == limit.Id).Max(p => p.StartTimestamp);
                    IOWDeviation latestDeviation = _iowDeviationRepository.FirstOrDefault(p => p.IOWLimitId == limit.Id && p.StartTimestamp == latestStartTimestamp);
                    limit.LastStatus = latestDeviation.EndTimestamp.HasValue ? IOWStatus.Deviation : IOWStatus.OpenDeviation;
                    limit.LastDeviationStartTimestamp = latestDeviation.StartTimestamp;
                    limit.LastDeviationEndTimestamp = latestDeviation.EndTimestamp;
                }
                else
                {
                    // There are no deviations for this limit
                    limit.LastStatus = IOWStatus.Normal;
                    limit.LastDeviationStartTimestamp = null;
                    limit.LastDeviationEndTimestamp = null;
                }

                _iowLimitRespository.Update(limit);
            }
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
