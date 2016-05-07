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

        public GetIOWLevelOutput GetIOWLevels()
        {
            List<IOWLevel> levels = _iowManager.GetAllLevels();
            return new GetIOWLevelOutput
            {
                IOWLevels = levels.MapTo<List<IOWLevelDto>>()
            };
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetIOWLevelOutput> GetIOWLevelsAsync()
        {
            var levels = await _iowLevelRepository.GetAllListAsync();
            var sorted = levels.OrderBy(result => result.Criticality);
            return new GetIOWLevelOutput
            {
                IOWLevels = sorted.MapTo<List<IOWLevelDto>>()
            };
        }

        public void UpdateIOWLevel(UpdateIOWLevelInput input)
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Updating an IOW level for input: " + input);

            //Retrieving an IOWLevel entity with given id (if specified) or name (if id is not specified).
            //FirstOrDefault() returns null if nothing is found.
            IOWLevel theLevel = null;
            if (input.Id.HasValue)
                theLevel = _iowManager.FirstOrDefaultLevel(input.Id.Value);
            else
                theLevel = _iowManager.FirstOrDefaultLevel(input.Name);

            if(theLevel != null )
            {
                if (!string.IsNullOrEmpty(input.Name))
                    theLevel.Name = input.Name;

                if (!string.IsNullOrEmpty(input.Description))
                    theLevel.Description = input.Description;

                if (input.Criticality.HasValue && input.Criticality.Value > 0 && input.Criticality.Value <= 5)
                    theLevel.Criticality = input.Criticality.Value;

                if (!string.IsNullOrEmpty(input.ResponseGoal))
                    theLevel.ResponseGoal = input.ResponseGoal;

                if (!string.IsNullOrEmpty(input.MetricGoal))
                    theLevel.MetricGoal = input.MetricGoal;

                IOWLevel output = _iowManager.InsertOrUpdateLevel(theLevel);
            }
        }

        public void CreateIOWLevel(CreateIOWLevelInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating an IOW level for input: " + input.Name );

            //Check to see if this name already exists
            var theLevel = _iowLevelRepository.FirstOrDefault(p => p.Name == input.Name);
            if (theLevel == null)
            {
                // All assets belong to a tenant. If not specified, put them in the default tenant.
                int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

                var New = new IOWLevel
                {
                    Name = input.Name,
                    Description = input.Description,
                    Criticality = input.Criticality,
                    ResponseGoal = input.ResponseGoal,
                    MetricGoal = input.MetricGoal,
                    TenantId = tenantid
                };

                _iowManager.InsertOrUpdateLevel(New);
            }
        }

        public void DeleteIOWLevel(GetOneIOWLevelInput input)
        {
            IOWLevel CurrentLevel = null;
            
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Deleting an IOW level for input: " + input);

            //Get the IOWLevel entity using either the Id or the name--whatever is passed through
            if(input.Id.HasValue)
                CurrentLevel = _iowLevelRepository.Get(input.Id.Value);
            else
                CurrentLevel = _iowLevelRepository.FirstOrDefault(p => p.Name == input.Name);

            if( CurrentLevel != null)
                _iowLevelRepository.Delete(CurrentLevel);
        }

        public void CreateDefaultIOWLevels()
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Creating default IOW levels");

            CreateIOWLevel(new CreateIOWLevelInput {
                Name = "Critical",
                Description = "Critical limit. Failure occurs quickly.",
                Criticality = 1,
                ResponseGoal = "One hour",
                MetricGoal = "Zero incidents"
            });

            CreateIOWLevel(new CreateIOWLevelInput
            {
                Name = "Standard",
                Description = "Standard limit. Failure occurs with sustained operations.",
                Criticality = 2,
                ResponseGoal = "12 hours",
                MetricGoal = "Zero incidents laster longer than 24 hours"
            });

            CreateIOWLevel(new CreateIOWLevelInput
            {
                Name = "Target",
                Description = "Optimal operating range. Inefficient or uneconomical with sustained operations.",
                Criticality = 3,
                ResponseGoal = "7 days",
                MetricGoal = "Zero incidents lasting longer than 10 days"
            });

            CreateIOWLevel(new CreateIOWLevelInput
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
