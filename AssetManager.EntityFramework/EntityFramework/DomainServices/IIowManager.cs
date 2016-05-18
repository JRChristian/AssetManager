using Abp.Domain.Services;
using AssetManager.Entities;
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
        List<IOWVariable> GetAllVariables();
        List<IOWVariable> GetAllVariables(Expression<Func<IOWVariable, bool>> predicate);
        IOWVariable InsertOrUpdateVariable(IOWVariable input);
        bool DeleteVariable(long id);
        bool DeleteVariable(string name);
        bool DeleteVariable(long? id, string name);

        IOWLimit FirstOrDefaultLimit(long id);
        IOWLimit FirstOrDefaultLimit(long variableId, long levelId);
        IOWLimit FirstOrDefaultLimit(string variableName, string levelName);
        IOWLimit FirstOrDefaultLimit(long variableId, long? levelId, string levelName);
        IOWLimit FirstOrDefaultLimit(long? variableId, string VariableName, long? levelId, string levelName);
        List<IOWLimit> GetAllLimits(long variableId);
        List<IOWLimit> GetAllLimits(string VariableName, string LevelName);
        long InsertOrUpdateLimitAndGetId(IOWLimit input);
        bool DeleteLimit(long id);

        // Deviations
        List<IOWDeviation> GetLimitDeviations(long limitId);
        List<VariableDeviation> GetDeviationSummary(bool includeAllVariables, int maxCriticality, double hoursBack);
        void DetectDeviations(Tag tag, DateTime startTimestamp, DateTime? endTimestamp);
    }
}
