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
    public class IOWLevelAppService : AssetManagerAppServiceBase, IIOWLevelAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IIOWLevelRepository _iowLevelRepository;
        private readonly IIowManager _iowManager;

        public IOWLevelAppService(IIOWLevelRepository iowLevelRepository, IIowManager iowManager)
        {
            _iowLevelRepository = iowLevelRepository;
            _iowManager = iowManager;
        }

        public IOWLevelDto GetOneIOWLevel(GetOneIOWLevelInput input)
        {
            IOWLevel level = null;
            if (input.Id.HasValue)
                level = _iowManager.FirstOrDefaultLevel(input.Id.Value);
            else if( !string.IsNullOrEmpty(input.Name) )
                level = _iowManager.FirstOrDefaultLevel(input.Name);

            return level.MapTo<IOWLevelDto>();
        }


        public GetIOWLevelOutput GetIOWLevels()
        {
            List<IOWLevel> levels = _iowManager.GetAllLevels();
            return new GetIOWLevelOutput
            {
                IOWLevels = levels.MapTo<List<IOWLevelDto>>()
            };
        }

        public void UpdateIOWLevel(UpdateIOWLevelInput input)
        {

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            //Retrieving an IOWLevel entity with given id (if specified) or name (if id is not specified).
            //FirstOrDefault() returns null if nothing is found.
            IOWLevel level = null;
            if (input.Id.HasValue)
                level = _iowManager.FirstOrDefaultLevel(input.Id.Value);
            else
                level = _iowManager.FirstOrDefaultLevel(input.Name);

            if (level == null )
            {
                // Level does not exist
                //We can use Logger, it is defined in ApplicationService base class.
                Logger.Info("Creating an IOW level for input: " + input.Name);

                IOWLevel New = new IOWLevel
                {
                    Name = input.Name,
                    Description = input.Description,
                    Criticality = input.Criticality,
                    ResponseGoal = input.ResponseGoal,
                    MetricGoal = input.MetricGoal,
                    TenantId = tenantid
                };
            }
            else // Level exists
            {
                //We can use Logger, it is defined in ApplicationService base class.
                Logger.Info("Updating an IOW level for input: " + input.Name);

                if (!string.IsNullOrEmpty(input.Name))
                    level.Name = input.Name;

                if (!string.IsNullOrEmpty(input.Description))
                    level.Description = input.Description;

                level.Criticality = input.Criticality;

                if (!string.IsNullOrEmpty(input.ResponseGoal))
                    level.ResponseGoal = input.ResponseGoal;

                if (!string.IsNullOrEmpty(input.MetricGoal))
                    level.MetricGoal = input.MetricGoal;
            }

            IOWLevel output = _iowManager.InsertOrUpdateLevel(level);
        }

        public void DeleteIOWLevel(GetOneIOWLevelInput input)
        {
            bool success = false;

            if (input.Id.HasValue)
                success = _iowManager.DeleteLevel(input.Id.Value);
            else
                success = _iowManager.DeleteLevel(input.Name);

            Logger.Info("Deleting an IOW level for input id: " + 
                (input.Id.HasValue ? input.Id.Value.ToString() : "n/a") + " name: " + input.Name +
                " with result: " + (success ? "succeeded" : "failed") );
        }

        public void CreateDefaultIOWLevels()
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Creating default IOW levels");

            UpdateIOWLevel(new UpdateIOWLevelInput {
                Name = "Critical",
                Description = "Critical limit. Failure occurs quickly.",
                Criticality = 1,
                ResponseGoal = "One hour",
                MetricGoal = "Zero incidents"
            });

            UpdateIOWLevel(new UpdateIOWLevelInput
            {
                Name = "Standard",
                Description = "Standard limit. Failure occurs with sustained operations.",
                Criticality = 2,
                ResponseGoal = "12 hours",
                MetricGoal = "Zero incidents laster longer than 24 hours"
            });

            UpdateIOWLevel(new UpdateIOWLevelInput
            {
                Name = "Target",
                Description = "Optimal operating range. Inefficient or uneconomical with sustained operations.",
                Criticality = 3,
                ResponseGoal = "7 days",
                MetricGoal = "Zero incidents lasting longer than 10 days"
            });

            UpdateIOWLevel(new UpdateIOWLevelInput
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
