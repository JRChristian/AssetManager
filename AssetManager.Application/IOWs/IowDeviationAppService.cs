using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.IOWs.Dtos;
using AssetManager.Tags;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public class IowDeviationAppService : AssetManagerAppServiceBase, IIowDeviationAppService
    {
        private readonly IIowManager _iowManager;
        private readonly ITagManager _tagManager;

        public IowDeviationAppService(
            IIowManager iowManager,
            ITagManager tagManager)
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
        }

        public GetVariableDeviationsOutput GetVariableDeviations(GetVariableDeviationsInput input)
        {
            DateTime now = DateTime.Now;
            GetVariableDeviationsOutput output = new GetVariableDeviationsOutput
            {
                Deviations = new List<DeviationsDto>()
            };

            IOWLimit limit = _iowManager.FirstOrDefaultLimit(input.VariableId, input.VariableName, input.LevelId, input.LevelName);
            if( limit != null )
            {
                output.VariableId = limit.Variable.Id;
                output.VariableName = limit.Variable.Name;
                output.LevelId = limit.Level.Id;
                output.LevelName = limit.Level.Name;
                output.Criticality = limit.Level.Criticality;

                List<IOWDeviation> allDeviations = _iowManager.GetLimitDeviations(limit.Id);
                foreach(IOWDeviation one in allDeviations)
                {
                    // If not specified, use "now" as the end time of the deviation to calculate duration
                    DateTime end = one.EndTimestamp.HasValue ? one.EndTimestamp.Value : now;

                    output.Deviations.Add(new DeviationsDto
                    {
                        StartTimestamp = one.StartTimestamp,
                        EndTimestamp = one.EndTimestamp,
                        LimitValue = one.LimitValue,
                        WorstValue = one.WorstValue,
                        Direction = one.Direction,
                        Status = one.EndTimestamp.HasValue ? IOWStatus.Deviation : IOWStatus.OpenDeviation,
                        DurationHours = (end - one.StartTimestamp).TotalHours 
                    });
                }
            }

            return output;
        }

        public GetDeviationSummaryOutput GetDeviationSummary(GetDeviationSummaryInput input)
        {
            // Get the input and set defaults for any missing values (defaults: do NOT include all variables, all levels, last 24 hours)
            bool includeAllVariables = input.includeAllVariables.HasValue ? input.includeAllVariables.Value : false;
            int maxCriticality = input.maxCriticality.HasValue ? input.maxCriticality.Value : 0;
            double hoursBack = input.hoursBack.HasValue ? input.hoursBack.Value : 24;

            GetDeviationSummaryOutput output = new GetDeviationSummaryOutput
            {
                includeAllVariables = includeAllVariables,
                maxCriticality = maxCriticality,
                hoursBack = hoursBack,
                deviations = new List<VariableDeviation>()
            };
            output.deviations = _iowManager.GetDeviationSummary(includeAllVariables, maxCriticality, hoursBack);

            return output;
        }


        public void DetectDeviations(DetectDeviationsInput input)
        {
            // Set defaults for the time range
            DateTime startTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : DateTime.Now.AddHours(-24);
            DateTime endTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : DateTime.Now.AddHours(1);

            // Validate the tag
            if( input.TagName == "*")
            {
                List<Tag> tags = _tagManager.GetAllListTag();
                foreach(Tag tag in tags)
                    _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
            else
            {
                Tag tag = _tagManager.FirstOrDefaultTag(input.TagId, input.TagName);
                if( tag != null )
                    _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
        }
    }
}
