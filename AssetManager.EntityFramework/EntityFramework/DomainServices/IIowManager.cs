﻿using Abp.Domain.Services;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public interface IIowManager : IDomainService
    {
        // Levels
        IOWLevel FirstOrDefaultLevel(long id);
        IOWLevel FirstOrDefaultLevel(string name);
        IOWLevel FirstOrDefaultLevel(long? id, string name);
        IOWLevel FirstOrDefaultLevel(Expression<Func<IOWLevel, bool>> predicate);
        List<IOWLevel> GetAllLevels();
        IOWLevel InsertOrUpdateLevel(IOWLevel input);
        bool DeleteLevel(long id);
        bool DeleteLevel(string name);
        bool DeleteLevel(long? id, string name);
        bool DeleteLevel(Expression<Func<IOWLevel, bool>> predicate);

        // Variables
        IOWVariable FirstOrDefaultVariable(long id);
        IOWVariable FirstOrDefaultVariable(string name);
        IOWVariable FirstOrDefaultVariable(long? id, string name);
        IOWVariable FirstOrDefaultVariable(Expression<Func<IOWVariable, bool>> predicate);
        IQueryable<IOWVariable> GetAllVariablesQueryable();
        List<IOWVariable> GetAllVariables();
        List<IOWVariable> GetAllVariables(Expression<Func<IOWVariable, bool>> predicate);
        IOWVariable InsertOrUpdateVariable(IOWVariable input);
        bool DeleteVariable(long id);
        bool DeleteVariable(string name);
        bool DeleteVariable(long? id, string name);

        // Limits
        IOWLimit FirstOrDefaultLimit(long id);
        IOWLimit FirstOrDefaultLimit(long variableId, long levelId);
        IOWLimit FirstOrDefaultLimit(string variableName, string levelName);
        IOWLimit FirstOrDefaultLimit(long variableId, long? levelId, string levelName);
        IOWLimit FirstOrDefaultLimit(long? variableId, string VariableName, long? levelId, string levelName);
        List<IOWLimit> GetAllLimits();
        List<IOWLimit> GetAllLimits(long variableId);
        List<IOWLimit> GetAllLimits(string variableName);
        List<IOWLimit> GetAllLimits(string variableName, string levelName);
        List<IOWLimit> GetAllLimits(long? variableId, string variableName, long? levelId, string levelName);
        List<IOWLimit> GetAllLimits(List<long> variableIds);
        List<IOWLimit> GetProblematicLimits(int maxCriticality, double hoursBack);
        List<IOWLimit> GetProblematicLimits(List<long> variableIds, int maxCriticality, double hoursBack);
        long InsertOrUpdateLimitAndGetId(IOWLimit input);
        bool DeleteLimit(long id);

        // Deviations
        List<IOWDeviation> GetDeviations();
        List<IOWDeviation> GetDeviations(long limitId);
        List<IOWDeviation> GetDeviations(long limitId, DateTime startTimestamp);
        List<VariableDeviation> GetDeviationSummary(bool includeAllVariables, int maxCriticality, double hoursBack);
        DetectDeviationsOut DetectDeviations(Tag tag, DateTime startTimestamp, DateTime endTimestamp);
        void ResetLastDeviationStatus();

        // Statistics
        List<LimitStatsByDay> GetLimitStatsByDay(IOWLimit limit, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStatsByDay> GetLimitStatsByDay(IOWVariable variable, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStatsByDay> GetLimitStatsByDay(long? variableId, string variableName, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStatsByDay> GetLimitStatsByDay(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStatsByDay> GetLimitStatsByDay(List<IOWVariable> variables, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStatsByDay> GetLimitStatsByDayGroupByLevel(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LimitStats> GetPerLimitStatsOverTime(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp);
        List<LevelStats> GetPerLevelStatsOverTime(List<long> limitIds, DateTime? startTimestamp, DateTime? endTimestamp);

        int CalculateStatisticsForAllLimits(DateTime? startTimestamp, DateTime? endTimestamp);
        int CalculateStatisticsForOneLimit(IOWLimit limit, DateTime? startTimestamp, DateTime? endTimestamp);

        DateTime NormalizeStartDay(DateTime? startTimestamp);
        DateTime NormalizeEndDay(DateTime startDay, DateTime? endTimestamp);
        DateTime NormalizeEndTimestamp(DateTime? startTimestamp, DateTime? endTimestamp);
    }
}
