using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Localization;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
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
        private readonly ILocalizationManager _localizationManager;

        public IowDeviationAppService(
            IIowManager iowManager,
            ITagManager tagManager,
            ILocalizationManager localizationManager)
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
            _localizationManager = localizationManager;
        }

        public GetVariableDeviationsOutput GetVariableDeviations(GetVariableDeviationsInput input)
        {
            GetVariableDeviationsOutput output = null;

            // Set the earliest time. If the argument is 0, use a default of 720 hours (30 days). Round back to a whole hour.
            int hoursBack = (input.hoursBack.HasValue && input.hoursBack.Value > 0) ? input.hoursBack.Value : 720;
            DateTime earliestTimestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0).AddHours(-hoursBack);

            // Make sure we a variable in the input
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.Id, input.VariableName);
            if( variable != null )
            {
                output = new GetVariableDeviationsOutput
                {
                    Id = variable.Id,
                    Name = variable.Name,
                    Description = variable.Description,
                    UOM = variable.UOM,
                    TagId = variable.TagId,
                    TagName = variable.Tag.Name,
                    EarliestTimestamp = earliestTimestamp,
                    Limits = new List<LimitDeviationDto>()
                };

                foreach( IOWLimit limit in variable.IOWLimits )
                {
                    output.Limits.Add(new LimitDeviationDto
                    {
                        Id = limit.Id,
                        IOWLevelId = limit.IOWLevelId,
                        LevelName = limit.Level.Name,
                        LevelDescription = limit.Level.Description,
                        Criticality = limit.Level.Criticality,
                        ResponseGoal = limit.Level.ResponseGoal,
                        MetricGoal = limit.Level.MetricGoal,
                        Direction = limit.Direction,
                        LimitValue = limit.Value,
                        Cause = limit.Cause,
                        Consequences = limit.Consequences,
                        Action = limit.Action,
                        Deviations = new List<DeviationDto>()
                    });

                    // Get the array index of the limit we just added
                    int i = output.Limits.Count - 1;
                    var someDeviations = from one in limit.IOWDeviations
                                         where !one.EndTimestamp.HasValue || one.EndTimestamp.Value >= earliestTimestamp
                                         select one;
               
                    foreach (var one in someDeviations)
                    {
                        output.Limits[i].Deviations.Add(new DeviationDto
                        {
                            StartTimestamp = one.StartTimestamp,
                            EndTimestamp = one.EndTimestamp,
                            LimitValue = one.LimitValue,
                            WorstValue = one.WorstValue
                        });
                    }
                    output.Limits[i].Deviations = output.Limits[i].Deviations.OrderByDescending(p => p.StartTimestamp).ToList();
                }
            }
            return output;
        }

        public GetVariableLimitCurrentOutput GetVariableLimitCurrent(GetVariableLimitCurrentInput input)
        {
            // Get the input and set defaults for any missing values (defaults: do NOT include all variables, all levels, last 24 hours)
            bool includeAllVariables = input.includeAllVariables.HasValue ? input.includeAllVariables.Value : false;
            int maxCriticality = input.maxCriticality.HasValue ? input.maxCriticality.Value : 0;
            double hoursBack = input.hoursBack.HasValue ? input.hoursBack.Value : 24;

            GetVariableLimitCurrentOutput output = new GetVariableLimitCurrentOutput
            {
                includeAllVariables = includeAllVariables,
                maxCriticality = maxCriticality,
                hoursBack = hoursBack,
                limitstatus = new List<VariableDeviation>()
            };
            output.limitstatus = _iowManager.GetDeviationSummary(includeAllVariables, maxCriticality, hoursBack);

            return output;
        }


        public DetectDeviationsOutput DetectDeviations(DetectDeviationsInput input)
        {
            DetectDeviationsOut output = new DetectDeviationsOut();

            // Set defaults for the time range
            DateTime startTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : DateTime.Now.AddHours(-24);
            DateTime endTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : DateTime.Now.AddHours(1);

            // Validate the tag
            if( input.TagName == "*")
            {
                List<Tag> tags = _tagManager.GetAllListTag();
                foreach(Tag tag in tags)
                    output = _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
            else
            {
                Tag tag = _tagManager.FirstOrDefaultTag(input.TagId, input.TagName);
                if( tag != null )
                    output = _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
            return output.MapTo<DetectDeviationsOutput>();
        }

        public void ResetLastDeviationStatus()
        {
            _iowManager.ResetLastDeviationStatus();
        }

        public GetLimitStatsByDayOutput GetLimitStatsByDay(GetLimitStatsByDayInput input)
        {
            List<LimitStatsByDay> stats = null;

            if (input.LimitId.HasValue)
            {
                IOWLimit limit = _iowManager.FirstOrDefaultLimit(input.LimitId.Value);
                stats = _iowManager.GetLimitStatsByDay(limit, input.startTimestamp, input.endTimestamp);
            }
            else
            {
                stats = _iowManager.GetLimitStatsByDay(input.VariableId, input.VariableName, input.startTimestamp, input.endTimestamp);
            }
            return new GetLimitStatsByDayOutput
            {
                Stats = stats.MapTo<List<LimitStatsByDayDto>>()
            };
        }

        public GetLimitStatsChartByDayOutput GetLimitStatsChartByDay(GetLimitStatsChartByDayInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");

            GetLimitStatsChartByDayOutput output = new GetLimitStatsChartByDayOutput
            {
                VariableName = input.VariableName,
                CanvasJS = new CanvasJSBar
                {
                    exportEnabled = true,
                    title = new CanvasJSBarTitle { },
                    axisX = new CanvasJSBarAxisX { },
                    axisY = new CanvasJSBarAxisY { },
                    data = new List<CanvasJSBarData>()
                }
            };

            // Look for the variable
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.VariableId, input.VariableName);

            if (variable != null)
            {
                output.VariableName = variable.Name;
                output.Description = variable.Description;
                output.TagId = variable.TagId;
                output.TagName = variable.Tag.Name;
                output.UOM = !string.IsNullOrEmpty(variable.UOM) ? variable.UOM : variable.Tag.UOM;

                output.StartTimestamp = _iowManager.NormalizeStartDay(input.StartTimestamp);
                output.EndTimestamp = _iowManager.NormalizeEndDay(output.StartTimestamp, input.EndTimestamp);

                output.CanvasJS.axisY.title = "Deviation hours";
                output.CanvasJS.axisY.minimum = 0;
                output.CanvasJS.axisY.maximum = 24; // Stacked bars could add up to more than 24 hours, but truncate anyway
                //output.CanvasJS.axisX.interval = 1;
                
                List<LimitStatsByDay> limitStats = _iowManager.GetLimitStatsByDay(variable, output.StartTimestamp, output.EndTimestamp);

                if(limitStats != null )
                {
                    // Each limit will be its own series
                    foreach(LimitStatsByDay limit in limitStats)
                    {
                        CanvasJSBarData data = new CanvasJSBarData
                        {
                            name = limit.Criticality.ToString() + " - " + limit.LevelName,
                            type = "stackedColumn",
                            legendText = limit.Criticality.ToString() + " - " + limit.LevelName,
                            showInLegend = true,
                            color = ChartColors.Criticality(limit.Criticality),
                            dataPoints = new List<CanvasJSBarDataPoints>()
                        };

                        // Now add the daily records
                        if( limit.Days != null )
                        {
                            foreach(LimitStatDays day in limit.Days )
                            {
                                CanvasJSBarDataPoints point = new CanvasJSBarDataPoints
                                {
                                    y = day.DurationHours,
                                    label = day.Day.ToString("m")
                                };
                                data.dataPoints.Add(point);
                            }
                        }
                        output.CanvasJS.data.Add(data);
                    }
                }
            }
            return output;
        }

        public CalculateStatisticsForAllLimitsOutput CalculateStatisticsForAllLimits(CalculateStatisticsForAllLimitsInput input)
        {
            CalculateStatisticsForAllLimitsOutput output = new CalculateStatisticsForAllLimitsOutput
            {
                NumberRecordsUpdated = _iowManager.CalculateStatisticsForAllLimits(input.startTimestamp, input.endTimestamp)
            };

            return output;
        }

        public CalculateStatisticsForOneLimitOutput CalculateStatisticsForOneLimit(CalculateStatisticsForOneLimitInput input)
        {
            CalculateStatisticsForOneLimitOutput output = new CalculateStatisticsForOneLimitOutput
            {
                NumberRecordsUpdated = 0
            };

           if (input.LimitId.HasValue && input.LimitId.Value > 0)
            {
                IOWLimit limit = _iowManager.FirstOrDefaultLimit(input.LimitId.Value);
                if (limit != null)
                    output.NumberRecordsUpdated = _iowManager.CalculateStatisticsForOneLimit(limit, input.startTimestamp, input.endTimestamp);
            }
            else if (!string.IsNullOrEmpty(input.VariableName) && !string.IsNullOrEmpty(input.LevelName))
            {
                List<IOWLimit> limits = _iowManager.GetAllLimits(input.VariableName, input.LevelName);
                if( limits != null )
                {
                    foreach(IOWLimit limit in limits )
                        output.NumberRecordsUpdated = _iowManager.CalculateStatisticsForOneLimit(limit, input.startTimestamp, input.endTimestamp);
                }
            }
            return output;
        }

    }
}
