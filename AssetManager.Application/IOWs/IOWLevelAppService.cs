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

        public IowLevelDto GetLevel(GetLevelInput input)
        {
            IOWLevel level = null;
            if (input.Id.HasValue)
                level = _iowManager.FirstOrDefaultLevel(input.Id.Value);
            else if( !string.IsNullOrEmpty(input.Name) )
                level = _iowManager.FirstOrDefaultLevel(input.Name);

            return level.MapTo<IowLevelDto>();
        }


        public GetAllLevelsOutput GetAllLevels()
        {
            List<IOWLevel> levels = _iowManager.GetAllLevels();
            return new GetAllLevelsOutput
            {
                IOWLevels = levels.MapTo<List<IowLevelDto>>()
            };
        }

        public IowLevelDto UpdateLevel(UpdateLevelInput input)
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Updating an IOW level for input: " + input.Name);

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            IOWLevel level = new IOWLevel
            {
                Name = input.Name,
                Description = input.Description,
                Criticality = input.Criticality,
                ResponseGoal = input.ResponseGoal,
                MetricGoal = input.MetricGoal,
                TenantId = tenantid
            };
            if (input.Id.HasValue)
                level.Id = input.Id.Value;

            level = _iowManager.InsertOrUpdateLevel(level);
            return level.MapTo<IowLevelDto>();
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

            if (input.Id.HasValue)
                output.Success = _iowManager.DeleteLevel(input.Id.Value);
            else
                output.Success = _iowManager.DeleteLevel(input.Name);

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
