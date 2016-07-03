using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.DomainServices;
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
using Abp.Domain.Uow;

namespace AssetManager.DomainServices
{
    public class IowManager : DomainService, IIowManager
    {
        // Validation information
        private static int minCriticality = 1;
        private static int maxCriticality = 5;
        private readonly double minimumLevel = 0; // 0%
        private readonly double maximumLevel = 100; // 100%

        private readonly IOWLevelRepository _iowLevelRepository;
        private readonly IOWVariableRepository _iowVariableRepository;
        private readonly IOWLimitRepository _iowLimitRespository;
        private readonly IOWDeviationRepository _iowDeviationRepository;
        private readonly IOWStatsByDayRepository _iowStatsByDayRepository;
        private readonly TagManager _tagManager;

        public IowManager(
            IOWLevelRepository iowLevelRepository,
            IOWVariableRepository iowVariableRepository,
            IOWLimitRepository iowLimitRepository,
            IOWDeviationRepository iowDeviationRepository,
            IOWStatsByDayRepository iowStatsByDayRepository,
            TagManager tagManager
            )
        {
            _iowLevelRepository = iowLevelRepository;
            _iowVariableRepository = iowVariableRepository;
            _iowLimitRespository = iowLimitRepository;
            _iowDeviationRepository = iowDeviationRepository;
            _iowStatsByDayRepository = iowStatsByDayRepository;
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
            // Validate information

            // Make sure criticality is in the proper range
            input.Criticality = input.Criticality.Clamp(minCriticality, maxCriticality);

            if (input.MetricType == MetricType.PercentLimitsInDeviation)
                input.GoodDirection = Direction.Low;
            else if (input.MetricType == MetricType.PercentTimeInDeviation)
                input.GoodDirection = Direction.Low;

            // If the good direction is down, then the warning level must be below the error level (going down: error, warning, good)
            if (input.GoodDirection == Direction.Low)
            {
                input.ErrorLevel = input.ErrorLevel.Clamp(minimumLevel, maximumLevel);
                input.WarningLevel = input.WarningLevel.Clamp(minimumLevel, input.ErrorLevel);
            }
            // If the good direction is up, then the warning level must be above the error level (going down: good, warning, error)
            else if (input.GoodDirection == Direction.High)
            {
                input.ErrorLevel = input.ErrorLevel.Clamp(minimumLevel, maximumLevel);
                input.WarningLevel = input.WarningLevel.Clamp(input.ErrorLevel, maximumLevel);
            }
            // Ignore other settings for good direction

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
                level.MetricType = input.MetricType;
                level.GoodDirection = input.GoodDirection;
                level.WarningLevel = input.WarningLevel;
                level.ErrorLevel = input.ErrorLevel;
            }
            else
            {
                // Did not find a record. Use the input as is.
                level = input;
            }

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

        public IQueryable<IOWVariable> GetAllVariablesQueryable()
        {
            return _iowVariableRepository.GetAll().OrderBy(p => p.Name);
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

        public List<IOWLimit> GetAllLimits()
        {
            return GetAllLimits(null, null, null, null);
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

        public List<IOWLimit> GetAllLimits(List<long> variableIds)
        {
            List<IOWLimit> output = null;
            if(variableIds != null )
            {
                var query = from limit in _iowLimitRespository.GetAll()
                            join variableId in variableIds on limit.IOWVariableId equals variableId
                            select limit;
                var results = query.ToList();
                if( results != null && results.Count > 0 )
                {
                    output = new List<IOWLimit>();
                    foreach (var r in results)
                        output.Add(r);
                }

            }
            return output;
        }

        public List<IOWLimit> GetAllLimits(int maxCriticality, double hoursBack)
        {
            if (maxCriticality <= 0)
                maxCriticality = 9999;
            if (hoursBack <= 0)
                hoursBack = 24;

            List<IOWLimit> output = null;
            // First part of where clause includes limits with active deviations (end time is null) and those recently ended (end time < threshold)
            // Second part excludes limits there have never had a deviation (both start and end times are null)
            // Third part includes just limits in the desired range of criticality.
            var query = from limit in _iowLimitRespository.GetAll()
                .Where(t => SqlFunctions.DateDiff("hour", (t.LastDeviationEndTimestamp ?? SqlFunctions.GetDate()), SqlFunctions.GetDate()) < hoursBack
                         && (t.LastDeviationStartTimestamp ?? SqlFunctions.DateAdd("day",1,SqlFunctions.GetDate())) < SqlFunctions.GetDate()
                         && t.Level.Criticality <= maxCriticality)
                select limit;
            var results = query.ToList();
            if (results != null && results.Count > 0)
            {
                output = new List<IOWLimit>();
                foreach (var r in results)
                    output.Add(r);
            }
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
        public List<IOWDeviation> GetDeviations()
        {
            return _iowDeviationRepository.GetAllList().OrderBy(p => p.IOWLimitId).ThenBy(p => p.StartTimestamp).ToList();
        }

        public List<IOWDeviation> GetDeviations(long limitId)
        {
            return _iowDeviationRepository.GetAllList(p => p.IOWLimitId == limitId).OrderBy(p => p.StartTimestamp).ToList();
        }

        public List<IOWDeviation> GetDeviations(long limitId, DateTime startTimestamp)
        {
            return _iowDeviationRepository.GetAllList(p => p.IOWLimitId == limitId && (p.EndTimestamp > startTimestamp || !p.EndTimestamp.HasValue )).OrderBy(p => p.StartTimestamp).ToList();
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

            return output;
        }


        public DetectDeviationsOut DetectDeviations(Tag tag, DateTime startTimestamp, DateTime endTimestamp)
        {
            DetectDeviationsOut output = new DetectDeviationsOut
            {
                StartTimestamp = startTimestamp,
                EndTimestamp = endTimestamp,
                NumberVariables = 0,
                NumberLimits = 0,
                NumberDeviations = 0
            };
            DateTime lastCheckDate = DateTime.Now;

            // Get all variables associated with this tag
            List<IOWVariable> variables = _iowVariableRepository.GetAllList(t => t.TagId == tag.Id);

            if( variables != null && variables.Count > 0 )
            {
                // Get all the data for the specified tag in the specified time range. Only consider good data.
                List<TagDataRaw> tagdata = _tagManager.GetAllListData(p => p.TagId == tag.Id && p.Timestamp >= startTimestamp && p.Timestamp <= endTimestamp && p.Quality == TagDataQuality.Good);

                // Continue only if we found some data.
                if (tagdata != null)
                {
                    foreach (IOWVariable v in variables)
                    {
                        output.NumberVariables++;

                        foreach (IOWLimit limit in v.IOWLimits)
                        {
                            output.NumberLimits++;
                            IOWDeviation deviation = null;

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

                                // If we do not have a deviation record in memory, check so see if the database has one.
                                // Find a deviation record (if one exists) where the time period of the deviation record 
                                // overlaps the current tag value.
                                // Need to do this for every tag data record, in case we are reprocessing history and
                                // we encounter new deviations as we go along.
                                if( deviation == null )
                                {
                                    deviation = limit.IOWDeviations.FirstOrDefault(x => x.StartTimestamp <= data.Timestamp &&
                                        (!x.EndTimestamp.HasValue || x.EndTimestamp.Value >= data.Timestamp));

                                    //deviation = _iowDeviationRepository.FirstOrDefault(x =>
                                    //    x.IOWLimitId == limit.Id && x.StartTimestamp < data.Timestamp &&
                                    //    (!x.EndTimestamp.HasValue || x.EndTimestamp.Value >= data.Timestamp));
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
                                        //_iowDeviationRepository.Update(deviation);

                                        // No updates needed to the overall limit record
                                    }
                                    else
                                    {
                                        // Case 1B - The existing deviation is over ==> update and close the old record
                                        deviation.EndTimestamp = data.Timestamp;
                                        //_iowDeviationRepository.Update(deviation);
                                        deviation = null;

                                        // Update the overall limit record -- maybe (not if the overall record describes a future deviation)
                                        if( !limit.LastDeviationStartTimestamp.HasValue || limit.LastDeviationStartTimestamp.Value < data.Timestamp )
                                        {
                                            limit.LastStatus = IOWStatus.Normal;
                                            limit.LastDeviationEndTimestamp = data.Timestamp;
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
                                        output.NumberDeviations++;

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

                            // And, update the statistics for this limit
                            CalculateStatisticsForOneLimit(limit, startTimestamp, endTimestamp);
                        } // foreach( IOWLimit limit in v.IOWLimits )
                    } // foreach(IOWVariable v in variables )
                } // if( tagdata != null )
            } // if( variables != null && variables.Count > 0 )
            return output;
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

        public List<LimitStatsByDay> GetLimitStatsByDay(IOWLimit limit, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<LimitStatsByDay> stats = new List<LimitStatsByDay>();

            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            if (limit != null)
                stats = GetLimitStatsOneLimit(limit, startDay, endDay);

            return stats;
        }

        public List<LimitStatsByDay> GetLimitStatsByDay(IOWVariable variable, DateTime? startTimestamp, DateTime? endTimestamp)
        {

            List<LimitStatsByDay> stats = new List<LimitStatsByDay>();
            if( variable != null && variable.IOWLimits != null )
            {
                // Get all limits for the specified variable
                List<long> limitIds = new List<long>();
                foreach (IOWLimit limit in variable.IOWLimits)
                    limitIds.Add(limit.Id);

                stats = GetLimitStatsByDay(limitIds, startTimestamp, endTimestamp);
            }
            return stats;
        }

        public List<LimitStatsByDay> GetLimitStatsByDay(long? variableId, string variableName, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            IOWVariable variable = FirstOrDefaultVariable(variableId, variableName);
            return GetLimitStatsByDay(variable, startTimestamp, endTimestamp);
        }

        public List<LimitStatsByDay> GetLimitStatsByDay(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<LimitStatsByDay> stats = new List<LimitStatsByDay>();

            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            // Build a date array for what SHOULD be in the statistics table for each variable
            List<DateTime> datetimes = new List<DateTime>();
            for (DateTime dt = startDay; dt < endDay; dt = dt.AddDays(1))
                datetimes.Add(dt);

            // Get the unique set of limits
            var query0 = from lim in _iowLimitRespository.GetAllList()
                         join l in limitIds on lim.Id equals l
                         orderby l
                         select new { LimitId = lim.Id, LevelName = lim.Level.Name, Criticality = lim.Level.Criticality, Direction = lim.Direction };
            var allLimits = query0.ToList();

            // This query returns the Cartesian product of all limits and dates
            var query1 = from lim in allLimits
                         from dt in datetimes
                         orderby lim.LimitId, dt
                         select new { LimitId = lim.LimitId, LevelName = lim.LevelName, Criticality = lim.Criticality, Direction = lim.Direction, Day = dt };
            var allLimitsAndDates = query1.ToList();

            // This query joins the Cartesian product to the stats table, and fills in zeros whenever the stats table lacks a record
            var query2 = from a in allLimitsAndDates
                         join s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay && p.Day <= endDay)
                         on new { LimitId = a.LimitId, Day = a.Day } equals new { LimitId = s.IOWLimitId, Day = s.Day } into joinedStats
                         from t in joinedStats.DefaultIfEmpty(new IOWStatsByDay { NumberDeviations = 0, DurationHours = 0 })
                         orderby a.LimitId, a.Day
                         select new { LimitId = a.LimitId, LevelName = a.LevelName, Criticality = a.Criticality, Direction = a.Direction, Day = a.Day, NumberDeviations = t.NumberDeviations, DurationHours = t.DurationHours };
            var dataset = query2.ToList();

            var query3 = from z in dataset
                         group z by new { z.LimitId, z.LevelName, z.Criticality, z.Direction, z.Day } into g
                         select new { LimitId = g.Key.LimitId, LevelName = g.Key.LevelName, Criticality = g.Key.Criticality, Direction = g.Key.Direction, Day = g.Key.Day, NumberDeviations = g.Sum(x => x.NumberDeviations), DurationHours = g.Sum(x => x.DurationHours) };
            var results = query3.ToList();

            if (results != null)
            {
                LimitStatsByDay stat = null;
                LimitStatsByDay lastStat = null;
                foreach (var d in results)
                {
                    if (lastStat == null || lastStat.LimitId != d.LimitId)
                    {
                        if (lastStat != null)
                            stats.Add(stat);

                        stat = new LimitStatsByDay
                        {
                            LimitId = d.LimitId,
                            LevelName = d.LevelName,
                            Criticality = d.Criticality,
                            Direction = d.Direction,
                            Days = new List<LimitStatDays>()
                        };
                    }
                    stat.Days.Add(new LimitStatDays { Day = d.Day, NumberDeviations = d.NumberDeviations, DurationHours = d.DurationHours });
                    lastStat = stat;
                }
                stats.Add(stat);
            }
            return stats;
        }

        public List<LimitStatsByDay> GetLimitStatsByDayGroupByLevel(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<LimitStatsByDay> stats = new List<LimitStatsByDay>();

            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            // Build a date array for what SHOULD be in the statistics table for each variable
            List<DateTime> datetimes = new List<DateTime>();
            for (DateTime dt = startDay; dt < endDay; dt = dt.AddDays(1))
                datetimes.Add(dt);

            // Get the unique set of limits
            var query0 = from lim in _iowLimitRespository.GetAllList()
                         join l in limitIds on lim.Id equals l
                         orderby l
                         select new { LimitId = lim.Id, LevelName = lim.Level.Name, Criticality = lim.Level.Criticality, Direction = lim.Direction };
            var allLimits = query0.ToList();

            // This query returns the Cartesian product of all limits and dates
            var query1 = from lim in allLimits
                         from dt in datetimes
                         orderby lim.LimitId, dt
                         select new { LimitId = lim.LimitId, LevelName = lim.LevelName, Criticality = lim.Criticality, Direction = lim.Direction, Day = dt };
            var allLimitsAndDates = query1.ToList();

            // This query joins the Cartesian product to the stats table, and fills in zeros whenever the stats table lacks a record
            var query2 = from a in allLimitsAndDates
                         join s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay && p.Day <= endDay)
                         on new { LimitId = a.LimitId, Day = a.Day } equals new { LimitId = s.IOWLimitId, Day = s.Day } into joinedStats
                         from t in joinedStats.DefaultIfEmpty(new IOWStatsByDay { NumberDeviations = 0, DurationHours = 0 })
                         orderby a.LimitId, a.Day
                         select new { LimitId = a.LimitId, LevelName = a.LevelName, Criticality = a.Criticality, Direction = a.Direction, Day = a.Day, NumberDeviations = t.NumberDeviations, DurationHours = t.DurationHours };
            var dataset = query2.ToList();

            var query3 = from z in dataset
                         group z by new { /*z.LimitId,*/ z.LevelName, z.Criticality, /*z.Direction,*/ z.Day } into g
                         select new { LimitId = 0 /*g.Key.LimitId*/, LevelName = g.Key.LevelName, Criticality = g.Key.Criticality, Direction = Direction.None /*g.Key.Direction*/, Day = g.Key.Day, NumberDeviations = g.Sum(x => x.NumberDeviations), DurationHours = g.Sum(x => x.DurationHours) };
            var results = query3.ToList();

            if (results != null)
            {
                LimitStatsByDay stat = null;
                LimitStatsByDay lastStat = null;
                foreach (var d in results)
                {
                    if (lastStat == null || lastStat.LimitId != d.LimitId || lastStat.LevelName != d.LevelName || lastStat.Criticality != d.Criticality)
                    {
                        if (lastStat != null)
                            stats.Add(stat);

                        stat = new LimitStatsByDay
                        {
                            LimitId = d.LimitId,
                            LevelName = d.LevelName,
                            Criticality = d.Criticality,
                            Direction = d.Direction,
                            Days = new List<LimitStatDays>()
                        };
                    }
                    stat.Days.Add(new LimitStatDays { Day = d.Day, NumberDeviations = d.NumberDeviations, DurationHours = d.DurationHours });
                    lastStat = stat;
                }
                stats.Add(stat);
            }
            return stats;
        }

        public List<LimitStatsByDay> GetLimitStatsByDay(List<IOWVariable> variables, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<long> limitIds = new List<long>();

            if( variables != null && variables.Count > 0 )
            {
                foreach(IOWVariable variable in variables)
                {
                    if( variable.IOWLimits != null && variable.IOWLimits.Count > 0 )
                    {
                        foreach (IOWLimit limit in variable.IOWLimits)
                            limitIds.Add(limit.Id);
                    }
                }
            }
            return GetLimitStatsByDay(limitIds, startTimestamp, endTimestamp);
        }

        private List<LimitStatsByDay> GetLimitStatsOneLimit(IOWLimit limit, DateTime startDay, DateTime endDay)
        {
            List<LimitStatsByDay> stats = new List<LimitStatsByDay>();

            // Build a date array for what SHOULD be in the statistics table for each variable
            List<DateTime> datetimes = new List<DateTime>();
            for (DateTime dt = startDay; dt < endDay; dt = dt.AddDays(1))
                datetimes.Add(dt);

            // This query returns the Cartesian product of all limits and dates
            var query1 = from lim in _iowLimitRespository.GetAllList(p => p.Id == limit.Id)
                        from dt in datetimes
                        orderby lim.Id, dt
                        select new { LimitId = lim.Id, LevelName = lim.Level.Name, Criticality = lim.Level.Criticality, Direction = lim.Direction, Day = dt };
            var allLimitsAndDates = query1.ToList();

            // This query joins the Cartesian product to the stats table, and fills in zeros whenever the stats table lacks a record
            var query2 = from a in allLimitsAndDates
                         join s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay && p.Day <= endDay)
                         on new { LimitId = a.LimitId, Day = a.Day } equals new { LimitId = s.IOWLimitId, Day = s.Day } into joinedStats
                         from t in joinedStats.DefaultIfEmpty( new IOWStatsByDay { NumberDeviations=0, DurationHours=0 })
                         orderby a.LimitId, a.Day
                         select new { LimitId=a.LimitId, LevelName=a.LevelName, Criticality=a.Criticality, Direction=a.Direction, Day=a.Day, NumberDeviations=t.NumberDeviations,  DurationHours=t.DurationHours};
            var results = query2.ToList();

            if( results != null )
            {
                LimitStatsByDay stat = null;
                LimitStatsByDay lastStat = null;
                foreach(var d in results)
                {
                    if( lastStat == null || lastStat.LimitId != d.LimitId )
                    {
                        if (lastStat != null)
                            stats.Add(stat);

                        stat = new LimitStatsByDay
                        {
                            LimitId = d.LimitId,
                            LevelName = d.LevelName,
                            Criticality = d.Criticality,
                            Direction = d.Direction,
                            Days = new List<LimitStatDays>()
                        };
                    }
                    stat.Days.Add(new LimitStatDays { Day = d.Day, NumberDeviations = d.NumberDeviations, DurationHours = d.DurationHours });
                    lastStat = stat;
                }
                stats.Add(stat);
            }
            return stats;
        }

        /*
         * Get statistics for each limit in a list of limits for a time period. Return totals for the entire time period.
         */
        public List<LimitStats> GetPerLimitStatsOverTime(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<LimitStats> stats = new List<LimitStats>();

            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            // Get the unique set of limits
            var query0 = from lim in _iowLimitRespository.GetAllList()
                         join l in limitIds on lim.Id equals l
                         orderby l
                         select new { LimitId = lim.Id, LevelName = lim.Level.Name, Criticality = lim.Level.Criticality, Direction = lim.Direction };
            var allLimits = query0.ToList();

            // This query joins the limits to the stats table, fills in zeros whenever the stats table lacks a record, and sums across all days
            var query2 = from all in allLimits
                         join s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay && p.Day <= endDay)
                         on new { LimitId = all.LimitId } equals new { LimitId = s.IOWLimitId } into joinedStats
                         from t in joinedStats.DefaultIfEmpty(new IOWStatsByDay { NumberDeviations = 0, DurationHours = 0 })
                         group joinedStats by new
                         {
                             LimitId = all.LimitId,
                             LevelName = all.LevelName,
                             Criticality = all.Criticality
                         } into g
                         orderby g.Key.LimitId, g.Key.LevelName, g.Key.Criticality
                         select new
                         {
                             LimitId = g.Key.LimitId,
                             LevelName = g.Key.LevelName,
                             Criticality = g.Key.Criticality,
                             NumberDeviations = g.Sum(x => x.Sum(y => y.NumberDeviations)),
                             DurationHours = g.Sum(x => x.Sum(y => y.DurationHours))
                         };
            var results = query2.ToList();

            if (results != null && results.Count > 0 )
            {
                foreach (var d in results)
                {
                    stats.Add( new LimitStats
                    {
                        LimitId = d.LimitId,
                        LevelName = d.LevelName,
                        Criticality = d.Criticality,
                        NumberDeviations = d.NumberDeviations,
                        DurationHours = d.DurationHours
                    });
                }
            }
            return stats;
        }

        /*
         * Get statistics by level for all limits in a list for a time period. Return totals for the entire time period.
         */
        public List<LevelStats> GetPerLevelStatsOverTime(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<LevelStats> stats = new List<LevelStats>();

            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);
            double totalHours = (endDay - startDay).TotalHours;

            // Calculate the total duration possible for deviations, which is from the starting time to the earlier of the specified end time and now.
            double durationHours = totalHours;
            if (endDay > DateTime.Now)
                durationHours = (DateTime.Now - startDay).TotalHours;

            // Get the unique set of limits
            var query1 = from lim in _iowLimitRespository.GetAllList()
                         join l in limitIds on lim.Id equals l
                         select new
                         {
                             LimitId = lim.Id,
                             LevelName = lim.Level.Name,
                             Criticality = lim.Level.Criticality,
                             MetricType = lim.Level.MetricType,
                             GoodDirection = lim.Level.GoodDirection,
                             WarningLevel = lim.Level.WarningLevel,
                             ErrorLevel = lim.Level.ErrorLevel,
                             Direction = lim.Direction
                         };
            var allLimits = query1.ToList();

            // Add statistics to these limits
            var query2 = from lim in allLimits
                         join s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay && p.Day <= endDay) 
                         on lim.LimitId equals s.IOWLimitId into joined
                         from s in joined.DefaultIfEmpty()
                         select new
                         {
                             LimitId = lim.LimitId,
                             LevelName = lim.LevelName,
                             Criticality = lim.Criticality,
                             MetricType = lim.MetricType,
                             GoodDirection = lim.GoodDirection,
                             WarningLevel = lim.WarningLevel,
                             ErrorLevel = lim.ErrorLevel,
                             Direction = lim.Direction,
                             Day = (s != null) ? s.Day : startDay,
                             NumberDeviations = (s != null) ? s.NumberDeviations : 0,
                             DurationHours = (s != null) ? s.DurationHours : 0
                         };
            var allLimitsWithStats = query2.ToList();

            // This query groups by limit and sums across all days
            var query3 = from lim in allLimitsWithStats
                         group lim by new 
                         {
                             LimitId = lim.LimitId,
                             LevelName = lim.LevelName,
                             Criticality = lim.Criticality,
                             MetricType = lim.MetricType,
                             GoodDirection = lim.GoodDirection,
                             WarningLevel = lim.WarningLevel,
                             ErrorLevel = lim.ErrorLevel
                         } into g
                         select new
                         {
                             LimitId = g.Key.LimitId,
                             LevelName = g.Key.LevelName,
                             Criticality = g.Key.Criticality,
                             MetricType = g.Key.MetricType,
                             GoodDirection = g.Key.GoodDirection,
                             WarningLevel = g.Key.WarningLevel,
                             ErrorLevel = g.Key.ErrorLevel,
                             NumberDeviations = g.Sum(x => x.NumberDeviations),
                             DurationHours = g.Sum(x => x.DurationHours)
                         };
            var allLimitsWithStatsSummed = query3.ToList();

            // This query groups by level, so combines multiple limits having the same level
            var query4 = from lim in allLimitsWithStatsSummed
                         group lim by new
                         {
                             LevelName = lim.LevelName,
                             Criticality = lim.Criticality,
                             MetricType = lim.MetricType,
                             GoodDirection = lim.GoodDirection,
                             WarningLevel = lim.WarningLevel,
                             ErrorLevel = lim.ErrorLevel
                         } into g
                         orderby g.Key.Criticality, g.Key.LevelName
                         select new
                         {
                             LevelName = g.Key.LevelName,
                             Criticality = g.Key.Criticality,
                             MetricType = g.Key.MetricType,
                             GoodDirection = g.Key.GoodDirection,
                             WarningLevel = g.Key.WarningLevel,
                             ErrorLevel = g.Key.ErrorLevel,
                             NumberDeviations = g.Sum(x => x.NumberDeviations),
                             DurationHours = g.Sum(x => x.DurationHours),
                             NumberLimits = g.Count(),
                             NumberDeviatingLimits = g.Count(x => x.NumberDeviations > 0)
                         };
            var results = query4.ToList();

            if (results != null && results.Count > 0)
            {
                foreach (var d in results)
                {
                    double metricValue = d.GoodDirection == Direction.High ? 100.0 : 0.0;
                    if (d.NumberLimits > 0)
                    {
                        if (d.MetricType == MetricType.PercentLimitsInDeviation)
                            metricValue = d.NumberDeviatingLimits / d.NumberLimits * 100;
                        else if (d.MetricType == MetricType.PercentTimeInDeviation)
                            metricValue = d.DurationHours / durationHours / d.NumberLimits * 100;
                    }

                    stats.Add(new LevelStats
                    {
                        LevelName = d.LevelName,
                        Criticality = d.Criticality,
                        MetricType = d.MetricType,
                        GoodDirection = d.GoodDirection,
                        WarningLevel = d.WarningLevel,
                        ErrorLevel = d.ErrorLevel,
                        NumberDeviations = d.NumberDeviations,
                        DurationHours = d.DurationHours,
                        NumberLimits = d.NumberLimits,
                        NumberDeviatingLimits = d.NumberDeviatingLimits,
                        MetricValue = metricValue
                    });
                }
            }
            return stats;
        }


        /*
         * CalculateStatisticsForAllLimits()
         * 
         * Reads the deviation table (IOWDeviation), chunks up deviations by day, and stores
         * the results in the stats table (IOWStatsByDay). This routine processes all limits for a specified
         * time period. CalculateStatisticsForOneLimit() does the work for each limit.
         */
        public int CalculateStatisticsForAllLimits(DateTime? startTimestamp, DateTime? endTimestamp)
        {
            int numberRecordsUpdated = 0;

            // The start and end times must be at midnight. Limit the time period to the last 60 days.
            // Default calculations to today.
            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            // Calculate statistics for each limit
            List<IOWLimit> allLimits = GetAllLimits();
            if(allLimits != null && allLimits.Count > 0 )
            {
                foreach(IOWLimit limit in allLimits)
                {
                    numberRecordsUpdated += CalculateStatisticsForOneLimit(limit, startDay, endDay);
                }
            }

            // Make sure that the statistics records exist
            //int numberRecordsInserted = FillInMissingStatRecords(startDay);

            return numberRecordsUpdated;
        }

        /*
         * CalculateStatisticsForOneLimit()
         * 
         * Handles a single limit for a specified time period. It reads the deviation table (IOWDeviation), 
         * chunks up deviations by day, and stores the results in the stats table (IOWStatsByDay).
         * 
         * Since deviations can change as new tag data are received, the statistics table (which is derived from the deviations
         * table) can also change. As changes are hard to figure out, this routine simply zeros out the stats table for the limit
         * and time period in question, and recalculates the statistics for each day of interest.
         * 
         * May be called by UpdateStatistics(), which loops through all limits. Can also be triggered when deviations are updated.
         */
        public int CalculateStatisticsForOneLimit(IOWLimit limit, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            int numberRecordsUpdated = 0;

            // This routine calculates IOW deviation statistics for one limit for a specified time range.
            // The start and end time must be at midnight, to match the records expected in IOWStatsByDay.
            // Validation must be done by the caller.
            // The start and end times must be at midnight. Limit the time period to the last 60 days.
            // Default calculations to today.
            DateTime startDay = NormalizeStartDay(startTimestamp);
            DateTime endDay = NormalizeEndDay(startDay, endTimestamp);

            // Get any stats that already exist for the time period of interest, and zero out the data.
            // Do this before getting the deviations because it is possible that there are not any deviations, in which case
            // the stats should be zeroed.
            List<IOWStatsByDay> allStats = _iowStatsByDayRepository.GetAllList(p => p.IOWLimitId == limit.Id && p.Day >= startDay && p.Day <= endDay).ToList();
            if( allStats != null && allStats.Count > 0 )
            {
                allStats.Select(p => { p.NumberDeviations = 0; p.DurationHours = 0; return p; }).ToList();
                /*foreach(IOWStatsByDay stat in allStats)
                {
                    stat.NumberDeviations = 0;
                    stat.DurationHours = 0;
                }*/
            }

            List<IOWDeviation> deviations = GetDeviations(limit.Id, startDay);
            if( deviations != null && deviations.Count > 0 )
            {
                IOWStatsByDay stat = null;

                // Set the last end day to before the start of the processing period
                DateTime startDeviation, endDeviation;

                foreach(IOWDeviation dev in deviations)
                {
                    // startDay       = midnight on the day when the deviation starts; clamped to be no earlier than startTimestamp in the arguments
                    // endDay         = midnight on the day after startDay
                    // startDeviation = start of the deviation for this day's statistics; clamped to be no earlier than startDay
                    // endDeviation   = end of the deviation for this day's statistics; allowed to be beyond the end of this day; defaults to now
                    if (dev.StartTimestamp < startDay)
                    {
                        startDeviation = startDay;
                    }
                    else
                    {
                        startDay = dev.StartTimestamp.Date;
                        startDeviation = dev.StartTimestamp;
                    }
                    if (dev.EndTimestamp.HasValue)
                        endDeviation = dev.EndTimestamp.Value;
                    else
                        endDeviation = DateTime.Now;

                    // Process this deviation until we run out of days.
                    bool insertNewRecord = false;
                    while ( startDeviation < endDeviation )
                    {
                        endDay = startDay.AddDays(1);
                        double durationHours = ((endDeviation <= endDay ? endDeviation : endDay) - startDeviation).TotalHours;

                        // If we already have a stat record (from earlier in the loop) and it was for a different time period and it was created here,
                        // then insert it and start over. Otherwise, keep the last stat record as we might want to reuse it.
                        if (stat != null && stat.Day != startDay && insertNewRecord)
                        {
                            _iowStatsByDayRepository.Insert(stat);
                            allStats.Add(stat);
                            stat = null;
                            insertNewRecord = false;
                        }

                        // Look for an already existing stat record to update.
                        // If we do not already have a stat record OR the dates don't match, look for an existing one in the database
                        // If we can't find a stat record in the database, then create a new record. We'll insert it later.
                        if (stat == null || stat.Day != startDay)
                        {
                            stat = allStats.FirstOrDefault(p => p.Day == startDay);
                            if (stat == null)
                            {
                                stat = new IOWStatsByDay { IOWLimitId = limit.Id, Day = startDay, NumberDeviations = 0, DurationHours = 0, TenantId = limit.TenantId };
                                insertNewRecord = true;
                            }
                            else
                                insertNewRecord = false;
                        }
                        else
                            insertNewRecord = false;


                        // Update the stat record with new information
                        stat.NumberDeviations++;
                        stat.DurationHours += durationHours;
                        numberRecordsUpdated++;

                        // Move to the next day. Look to see if this deviation slides into the following day
                        startDay = startDay.AddDays(1);
                        endDay = startDay.AddDays(1);
                        startDeviation = startDay;
                    }
                    // Handle the last record, if any
                    if (insertNewRecord)
                    {
                        _iowStatsByDayRepository.Insert(stat);
                        allStats.Add(stat);
                    }
                    // ABP will automatically update open records, so no need to call the Update() method on IOWStatByDay.
                } // foreach(IOWDeviation dev in deviations)
            } // if( deviations != null && deviations.Count > 0 )

            return numberRecordsUpdated;
        }

        /* Translate an optional, arbitrary start TIME to a normalized start DAY that is timestamped at midnight.
         * Basically we round back and limit the time period to 60 days.
         */
        public DateTime NormalizeStartDay(DateTime? startTimestamp)
        {
            DateTime startDay = startTimestamp.HasValue ? startTimestamp.Value.Date : DateTime.Now.Date;
            if (startDay < DateTime.Now.AddDays(-60))
                startDay = DateTime.Now.AddDays(-60).Date;

            return startDay;
        }

        /*
         *  Translate an optional, arbitrary end TIME to a normalized end DAY that is timestamped at midnight.
         *  Basically we take the given time, make sure it falls between the start and now, and go forward to the next midnight.
         */
        public DateTime NormalizeEndDay(DateTime startDay, DateTime? endTimestamp)
        {
            DateTime endDay = endTimestamp.HasValue ? endTimestamp.Value : DateTime.Now;
            if (endDay != endDay.Date)
                endDay = endDay.AddDays(1).Date;
            if (endDay <= startDay || endDay > DateTime.Now.AddDays(1).Date)
                endDay = DateTime.Now.AddDays(1).Date;
            return endDay;
        }

        /*
         *  Translate an optional, arbitrary end time.
         *  Basically we take the given time and make sure it falls between the start and now.
         */
        public DateTime NormalizeEndTimestamp(DateTime? startTimestamp, DateTime? endTimestamp)
        {
            DateTime end = endTimestamp.HasValue ? endTimestamp.Value : DateTime.Now;
            if (startTimestamp.HasValue && startTimestamp.Value > end)
                end = startTimestamp.Value;
            if (end > DateTime.Now )
                end = DateTime.Now;
            return end;
        }


        private int FillInMissingStatRecords(DateTime startDay)
        {
            int numberRecordsInserted = 0;

            // Build a datetime array for what SHOULD be in the statistics table for each variable
            List<DateTime> datetimes = new List<DateTime>();
            for (DateTime dt = startDay; dt < DateTime.Now; dt = dt.AddDays(1))
                datetimes.Add(dt);

            // This query takes the Cartesian product of the limit table and our datetime array.
            // The Cartesian product gives every limit-day combination (in the time period of interest) that should exist.
            // It then does an outer join with the statistics table. We should have the same number of records as the Cartesian product.
            // It then removes any instances where the statistic table does not match the Cartesian product.
            // The result shows all records that are missing from the statistics table.
            var query = from k in
                            (from a in (
                                from l in _iowLimitRespository.GetAllList()
                                from d in datetimes
                                select new { Id = l.Id, Day = d, TenantId = l.TenantId })
                             from s in _iowStatsByDayRepository.GetAllList(p => p.Day >= startDay)
                                 .Where(s => s.IOWLimitId == a.Id && s.Day == a.Day)
                                 .DefaultIfEmpty()
                             select new { Id = a.Id, Day = a.Day, TenantId = a.TenantId, IsMissing = (s == null) })
                            .Where(k => k.IsMissing == true)
                        orderby k.Id, k.Day
                        select new { Id = k.Id, Day = k.Day, k.TenantId };

            var missing = query.ToList();

            // Walk the list of stat records and insert new records for anything missing
            foreach (var one in missing)
            {
                _iowStatsByDayRepository.Insert(new IOWStatsByDay
                {
                    TenantId = one.TenantId,
                    IOWLimitId = one.Id,
                    Day = one.Day,
                    NumberDeviations = 0,
                    DurationHours = 0
                });
                numberRecordsInserted++;
            }
            return numberRecordsInserted;
        }
    }
}