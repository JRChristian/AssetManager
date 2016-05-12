using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.IOWs.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public class IowLevelAppService : AssetManagerAppServiceBase, IIowLevelAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IIowManager _iowManager;

        public IowLevelAppService(IIowManager iowManager)
        {
            _iowManager = iowManager;
        }

        public GetLevelOutput GetLevel(GetLevelInput input)
        {
            IOWLevel level = _iowManager.FirstOrDefaultLevel(input.Id, input.Name);

            return new GetLevelOutput
            {
                level = level.MapTo<LevelDto>(),
                LevelUseCount = (level != null) ? level.IOWLimits.Count : 0
            };
        }


        public GetAllLevelsOutput GetAllLevels()
        {
            List<IOWLevel> levels = _iowManager.GetAllLevels();
            return new GetAllLevelsOutput
            {
                IOWLevels = levels.MapTo<List<LevelDto>>()
            };
        }

        public LevelDto UpdateLevel(UpdateLevelInput input)
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Updating an IOW level for input: " + input.Name);

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            IOWLevel level = new IOWLevel
            {
                Name = input.Name,
                Description = !string.IsNullOrEmpty(input.Description) ? input.Description : input.Name,
                Criticality = input.Criticality,
                ResponseGoal = !string.IsNullOrEmpty(input.ResponseGoal) ? input.ResponseGoal : "",
                MetricGoal = !string.IsNullOrEmpty(input.MetricGoal) ? input.MetricGoal : "",
                TenantId = tenantid
            };
            if (input.Id.HasValue)
                level.Id = input.Id.Value;

            level = _iowManager.InsertOrUpdateLevel(level);
            return level.MapTo<LevelDto>();
        }

        public DeleteLevelOutput DeleteLevel(GetLevelInput input)
        {
            DeleteLevelOutput output = new DeleteLevelOutput
            {
                Id = input.Id.HasValue ? input.Id.Value : 0,
                Name = input.Name,
                Success = false
            };
            Logger.Info("Deleting an IOW level for input id: " + output.Id.ToString() + " name: " + input.Name);

            output.Success = _iowManager.DeleteLevel(input.Id, input.Name);

            return output;
        }

        public void CreateDefaultLevels()
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Creating default IOW levels");

            UpdateLevel(new UpdateLevelInput {
                Name = "Critical",
                Description = "Critical limit. Failure occurs quickly.",
                Criticality = 1,
                ResponseGoal = "One hour",
                MetricGoal = "Zero incidents"
            });

            UpdateLevel(new UpdateLevelInput
            {
                Name = "Standard",
                Description = "Standard limit. Failure occurs with sustained operations.",
                Criticality = 2,
                ResponseGoal = "12 hours",
                MetricGoal = "Zero incidents laster longer than 24 hours"
            });

            UpdateLevel(new UpdateLevelInput
            {
                Name = "Target",
                Description = "Optimal operating range. Inefficient or uneconomical with sustained operations.",
                Criticality = 3,
                ResponseGoal = "7 days",
                MetricGoal = "Zero incidents lasting longer than 10 days"
            });

            UpdateLevel(new UpdateLevelInput
            {
                Name = "Information",
                Description = "Information only. No consequences expected with sustained operations.",
                Criticality = 4,
                ResponseGoal = "N/A",
                MetricGoal = "N/A"
            });
        }
    }
}
