﻿using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Localization;
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
    public class IowVariableAppService : AssetManagerAppServiceBase, IIowVariableAppService
    {
        private readonly IIowManager _iowManager;
        private readonly ITagManager _tagManager;
        private readonly ILocalizationManager _localizationManager;

        public IowVariableAppService(
            IIowManager iowManager,
            ITagManager tagManager,
            ILocalizationManager localizationManager)
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
            _localizationManager = localizationManager;
        }


        public GetVariableOutput GetVariable(GetVariableInput input)
        {
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.Id, input.Name);

            return new GetVariableOutput
            {
                variable = variable.MapTo<VariableDto>(),
                LimitCount = (variable != null) ? variable.IOWLimits.Count : 0
            };
        }

        public GetAllVariablesOutput GetAllVariables(GetAllVariablesInput input)
        {
            List<IOWVariable> allVariables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if (!string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName))
                allVariables = _iowManager.GetAllVariables(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName));

            else if (!string.IsNullOrEmpty(input.Name))
                allVariables = _iowManager.GetAllVariables(v => v.Name.Contains(input.Name));

            else if (!string.IsNullOrEmpty(input.TagName))
                allVariables = _iowManager.GetAllVariables(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName));

            else
                allVariables = _iowManager.GetAllVariables();

            return new GetAllVariablesOutput
            {
                variables = allVariables.MapTo<List<VariableDto>>()
            };
        }

        public GetVariableLimitsOutput GetVariableLimits(GetVariableLimitsInput input)
        {
            int i, j;
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.Id, input.Name);

            GetVariableLimitsOutput output = new GetVariableLimitsOutput
            {
                Id = variable.Id,
                Name = variable.Name,
                Description = variable.Description,
                TagId = variable.TagId,
                TagName = variable.Tag.Name,
                UOM = variable.UOM,
                LastTimestamp = variable.Tag.LastTimestamp,
                LastValue = variable.Tag.LastValue,
                LastQuality = variable.Tag.LastQuality,
                Limits = new List<LimitDto>()
            };

            // Get limits currently in use for this variable, sorted by level name and direction. There could be 0, 1 or 2 limits for each level name.
            List<IOWLimit> limits = variable.IOWLimits.OrderBy(p => p.Level.Name).ThenBy(p => p.Direction).ToList();

            // Get all available levels defined at this site, sorted by name.
            List<IOWLevel> levels = _iowManager.GetAllLevels().OrderBy(p => p.Name).ToList();

            // Now build the output the hard way, since we have our own logic.
            // "j" is our counter into the list of limits. This list will have between zero items
            // and twice the number of levels. (One each--potentially--for the low and high limit.)
            j = 0;
            bool isLimitPresent = false;

            // Loop through the levels. For each level, there will be 0, 1 or 2 limits to consider.
            for (i = 0; i < levels.Count; i++)
            {
                // Is the low limit present?
                // It is *IF* we haven't run out of limits AND the names match AND the direction is low
                isLimitPresent = j < limits.Count && 
                    limits[j].Level.Name == levels[i].Name && 
                    limits[j].Direction == Direction.Low;
                if (isLimitPresent || input.IncludeUnusedLimits)
                {
                    // Add a low limit to the working list
                    output.Limits.Add(new LimitDto
                    {
                        Id = isLimitPresent ? limits[j].Id : 0,
                        IOWVariableId = variable.Id,
                        IsActive = isLimitPresent,
                        IOWLevelId = levels[i].Id,
                        Name = levels[i].Name,
                        Description = levels[i].Description,
                        Criticality = levels[i].Criticality,
                        ResponseGoal = levels[i].ResponseGoal,
                        MetricGoal = levels[i].MetricGoal,
                        StartDate = isLimitPresent ? limits[j].StartDate : new DateTime(2014, 1, 1),
                        EndDate = isLimitPresent ? limits[j].EndDate : null,
                        Direction = isLimitPresent ? limits[j].Direction : Direction.Low,
                        Value = isLimitPresent ? limits[j].Value : double.NaN,
                        Cause = isLimitPresent ? limits[j].Cause : "",
                        Consequences = isLimitPresent ? limits[j].Consequences : "",
                        Action = isLimitPresent ? limits[j].Action : ""
                    });
                    if( isLimitPresent) // If we "used up" a limit, move on to the next one
                        j++;
                }

                // Is the high limit present?
                // It is *IF* we haven't run out of limits AND the names match AND the direction is high
                isLimitPresent = j < limits.Count &&
                    limits[j].Level.Name == levels[i].Name &&
                    limits[j].Direction == Direction.High;
                if (isLimitPresent || input.IncludeUnusedLimits)
                {
                    // Add a High limit to the working list
                    output.Limits.Add(new LimitDto
                    {
                        Id = isLimitPresent ? limits[j].Id : 0,
                        IOWVariableId = variable.Id,
                        IsActive = isLimitPresent,
                        IOWLevelId = levels[i].Id,
                        Name = levels[i].Name,
                        Description = levels[i].Description,
                        Criticality = levels[i].Criticality,
                        ResponseGoal = levels[i].ResponseGoal,
                        MetricGoal = levels[i].MetricGoal,
                        StartDate = isLimitPresent ? limits[j].StartDate : new DateTime(2014, 1, 1),
                        EndDate = isLimitPresent ? limits[j].EndDate : null,
                        Direction = isLimitPresent ? limits[j].Direction : Direction.High,
                        Value = isLimitPresent ? limits[j].Value : double.NaN,
                        Cause = isLimitPresent ? limits[j].Cause : "",
                        Consequences = isLimitPresent ? limits[j].Consequences : "",
                        Action = isLimitPresent ? limits[j].Action : ""
                    });
                    if (isLimitPresent)  // If we "used up" a limit, move on to the next one
                        j++;
                }

            } // for (i = 0; i < levels.Count; i++)

            // The working list is currently sorted by limit name. Provide a better sort for consumers.
            output.Limits = output.Limits.OrderBy(p => p.SortOrder).ToList();
            return output;
        }

        public DeleteVariableOutput DeleteVariable(GetVariableInput input)
        {
            Logger.Info("Delete a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            bool success = _iowManager.DeleteVariable(input.Id, input.Name);
            return new DeleteVariableOutput
            {
                Id = input.Id.HasValue ? input.Id.Value : 0,
                Name = input.Name,
                Success = success
            };
        }

        public VariableDto UpdateVariable(UpdateVariableInput input)
        {
            Logger.Info("Update a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            // Look for the variable
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.Id, input.Name);

            if( variable == null && !string.IsNullOrEmpty(input.Name) )
            {
                // No variable, so create one
                variable = new IOWVariable
                {
                    Name = input.Name,
                    TenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1
                };
            }

            if( variable != null )
            {
                variable.Description = input.Description;
                variable.UOM = input.UOM;

                // Check the tag
                Tag tag = _tagManager.FirstOrDefaultTag(input.TagId, input.Name);
                if( tag != null )
                    variable.TagId = tag.Id;

                variable = _iowManager.InsertOrUpdateVariable(variable);
            }
            return variable.MapTo<VariableDto>();
        }

        public GetAllLimitsOutput GetAllLimits(GetAllLimitsInput input)
        {
            List<LimitDto> output = new List<LimitDto>();
            int i, j;

            // Look for the variable. This will NOT throw an error if the variable id is not found.
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.VariableId);

            // Get limits for this variable.
            List<IOWLimit> limits = _iowManager.GetAllLimits(input.VariableId);

            // Get all available levels defined at this site.
            List<IOWLevel> levels = _iowManager.GetAllLevels();

            // Now build the output DTO. Do it the hard way, since we have our own logic.
            // "j" is our counter into the list of limits. This list will have between zero items
            // and the number of items in the level list.
            j = limits.Count > 0 ? 0 : levels.Count + 1;

            for ( i = 0; i < levels.Count; i++ )
            {
                // Increment j (index into existing limits) to match the level, if possible
                for (; j < limits.Count && limits[j].Level.Criticality < levels[i].Criticality; j++ );
                bool haveLimit = (j < limits.Count) && (limits[j].Level.Criticality == levels[i].Criticality);

                // If j (the index into the list of limits for this variable) <= i (index into the list of all levels)
                // then the limit matches the level, and this level matches an active limit.
                // Otherwise, this level is not currently active for this variable.
                output.Add(new LimitDto
                {
                    Id = haveLimit ? limits[j].Id : 0,
                    IsActive = haveLimit ? true : false,
                    IOWVariableId = variable != null ? variable.Id : 0,
                    IOWLevelId = levels[i].Id,
                    Name = levels[i].Name,
                    Description = levels[i].Description,
                    Criticality = levels[i].Criticality,
                    ResponseGoal = levels[i].ResponseGoal,
                    MetricGoal = levels[i].MetricGoal,
                    StartDate = limits[j].StartDate,
                    EndDate = limits[j].EndDate,
                    Direction = limits[j].Direction,
                    Value = limits[j].Value,
                    Cause = haveLimit ? limits[j].Cause : "",
                    Consequences = haveLimit ? limits[j].Consequences : "",
                    Action = haveLimit ? limits[j].Action : ""
                });
            }

            return new GetAllLimitsOutput
            {
                Limits = new List<LimitDto>(output)
            };
        }

        public LimitDto UpdateLimit(UpdateLimitInput input)
        {
            DateTime oldestDate = new DateTime(2014, 1, 1);

            // This method accepts one limit for one variable.
            // If the limit does not exist, it is added.
            // If the limit exists, it is changed or deleted.

            LimitDto output = null;
            bool inputIsValid = false;
            bool isActive = input.IsActive.HasValue ? input.IsActive.Value : true;

            // Look for the variable.
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.IOWVariableId, input.VariableName);

            if( variable != null )
            {

                // Does the specified limit exist? If so, get it. This will return null if nothing is found.
                IOWLimit limit = _iowManager.FirstOrDefaultLimit(variable.Id, input.Id, input.LevelName);

                // Does the specified level exist? It should.
                IOWLevel level = _iowManager.FirstOrDefaultLevel(input.IOWLevelId, input.LevelName);

                // There are five possibilities at this point:
                //   1) We did not find a level ==> bad input, do nothing
                //   2) We found a level, found a limit, isActive flag is true ==> update an existing limit
                //   3) We found a level, found a limit, IsActive flag is false ==> delete an existing limit
                //   4) We found a level, did not find a limit, IsActive flag is true ==> insert a new limit
                //   5) We found a level, did not find a limit, IsActive flag is false ==> do nothing

                if ( level == null )
                {
                    // Case 1: Bad input
                }
                else if ( isActive )
                {
                    // Case 2: IsActive is true AND limit exists ==> update an existing limit
                    // Case 4: IsActive is true AND limit does not exist ==> insert a new limit

                    // For case 4 (limit does not exist), need to create a limit record to continue
                    if ( limit == null )
                        limit = new IOWLimit
                        {
                            IOWVariableId = variable.Id,
                            IOWLevelId = level.Id,
                            LastCheckDate = null,
                            LastStatus = IOWStatus.Normal,
                            LastDeviationStartTimestamp = null,
                            LastDeviationEndTimestamp = null,
                            TenantId = variable.TenantId
                        };

                    limit.StartDate = new DateTime(Math.Max(input.StartDate.Ticks, oldestDate.Ticks));
                    limit.EndDate = input.EndDate;
                    limit.Direction = input.Direction;
                    limit.Value = input.Value;
                    limit.Cause = input.Cause;
                    limit.Consequences = input.Consequences;
                    limit.Action = input.Action;

                    limit.Id = _iowManager.InsertOrUpdateLimitAndGetId(limit);

                    inputIsValid = true;
                }
                else if ( limit != null && !isActive )
                {
                    // Case 3: Limit exists and should be deleted
                    _iowManager.DeleteLimit(limit.Id);

                    inputIsValid = true;
                }

                if( inputIsValid )
                    output = new LimitDto
                    {
                        Id = limit.Id,
                        IOWVariableId = variable.Id,
                        IsActive = (limit != null && isActive ) ? true : false,
                        IOWLevelId = limit.IOWLevelId,
                        Name = level.Name,
                        Criticality = level.Criticality,
                        StartDate = limit.StartDate,
                        EndDate = limit.EndDate,
                        Direction = limit.Direction,
                        Value = limit.Value,
                        Cause = limit.Cause,
                        Consequences = limit.Consequences,
                        Action = limit.Action
                    };
            }
            return output;
        }

        public GetIowChartCanvasJSOutput GetIowChartCanvasJS(GetIowChartCanvasJSInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");

            GetIowChartCanvasJSOutput output = new GetIowChartCanvasJSOutput
            {
                Name = input.Name,
                Description = "",
                TagId = 0,
                TagName = "",
                UOM = "",
                CanvasJS = new CanvasJS
                {
                    exportEnabled = true,
                    zoomEnabled = true,
                    zoomType = "xy",
                    title = new CanvasJSTitle { },
                    axisX = new CanvasJSAxisX { },
                    axisY = new CanvasJSAxisY { },
                    data = new List<CanvasJSData>()
                }
            };

            // Look for the variable
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.Id, input.Name);

            if( variable != null )
            {
                output.Name = variable.Name;
                output.Description = variable.Description;
                output.TagId = variable.TagId;
                output.TagName = variable.Tag.Name;
                output.UOM = !string.IsNullOrEmpty(variable.UOM) ? variable.UOM : variable.Tag.UOM;
                

                TagType tagType = variable.Tag.Type.HasValue ? variable.Tag.Type.Value : TagType.Continuous;
                string hi = ChartColors.CriticalForeground(0);

                output.CanvasJS.title.text = output.Name;
                output.CanvasJS.axisX.gridThickness = 0;
                output.CanvasJS.axisY.gridThickness = 0;
                output.CanvasJS.axisY.title = output.UOM;
                output.CanvasJS.axisY.includeZero = false;
                output.CanvasJS.data.Add(new CanvasJSData
                {
                    name = variable.Tag.Name,
                    type = tagType == TagType.Continuous ? "line" : "scatter",
                    lineDashType = "solid",
                    lineThickness = tagType == TagType.Continuous ? 2 : 0,
                    markerColor = ChartColors.CriticalForeground(0),
                    markerType = tagType == TagType.Continuous ? "none" : "circle",
                    xValueType = "dateTime",
                    color = ChartColors.CriticalForeground(0),
                    showInLegend = true,
                    legendText = variable.Tag.Name,
                    dataPoints = new List<CanvasJSDataPoints>()
                });

                // Get data in the desired range
                List<TagDataRaw> data = _tagManager.GetAllListData(output.TagId, input.StartTimestamp, input.EndTimestamp);

                // Convert the entity data into the necessary charting format
                foreach (TagDataRaw d in data)
                {
                    // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                    // C# datetime is in ticks (milliseconds) since January 1, 0000
                    output.CanvasJS.data[0].dataPoints.Add(new CanvasJSDataPoints { x = d.Timestamp.ToJavaScriptMilliseconds(), y = d.Value });
                }

                // Calculate the minimum and maximum times that cover our data
                DateTime minTimestamp = DateTime.Now.AddDays(-1);
                DateTime maxTimestamp = DateTime.Now;
                if (data != null && data.Count > 0)
                {
                    minTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : data[0].Timestamp;
                    maxTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : data[data.Count - 1].Timestamp;
                }
                else
                {
                    minTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : minTimestamp;
                    maxTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : maxTimestamp;
                }

                string valueFormatString = null;
                TimeSpan tsRange = maxTimestamp - minTimestamp;
                if (tsRange.TotalDays > 1)
                {
                    // Round the start back to the start of day
                    minTimestamp = new DateTime(minTimestamp.Year, minTimestamp.Month, minTimestamp.Day, 0, 0, 0);
                    valueFormatString = "DD-MMM HH:mm";
                }
                else if (tsRange.TotalHours > 1)
                {
                    // Round the start back to the start of the hour
                    minTimestamp = new DateTime(minTimestamp.Year, minTimestamp.Month, minTimestamp.Day, minTimestamp.Hour, 0, 0);
                    valueFormatString = "HH:mm";
                }
                output.CanvasJS.axisX.minimum = minTimestamp.ToJavaScriptMilliseconds();
                output.CanvasJS.axisX.maximum = maxTimestamp.ToJavaScriptMilliseconds();
                output.CanvasJS.axisX.valueFormatString = valueFormatString;

                // Add some limit information to the chart. Sort the limits from bottom to top, to help with shading. Most critical lower limit should be first, most critical higher limit should be last.
                List<IOWLimit> allLimits = variable.IOWLimits.OrderBy(p => ((int)p.Direction-2) * (1000-p.Level.Criticality)).ToList();

                // Get the smallest and largest values in the data, including both raw data and limits
                double maxValue = Math.Max(data.Max(p => p.Value), allLimits.Max(p => p.Value));
                double minValue = Math.Min(data.Min(p => p.Value), allLimits.Min(p => p.Value));

                // The "rangeArea" chart type needs an upper value for the highest limit. Set that to 10% (based on the range) above the maximum value in the data.
                double maxLimit = maxValue + 0.10 * (maxValue - minValue);

                bool isFirst = true;
                IOWLimit lastLimit = null;
                int lastHighIndex = -1;
                foreach( IOWLimit limit in allLimits)
                {
                    // Add this limit to the series.
                    // The first low limit (if there are any low limits) will be configured as an "area" series, which fills the area under the curve.
                    // All remaining limits will be configured as "rangeArea" series, which fills the area between two lines.
                    // For low limits that are configured as "rangeArea", the bottom line (y) is the previous limit and the top line (z) is the current limit.
                    // For high limits, the bottom line (y) is the current limit and the top line (z) is the next limit (if there is one) or 10% more than the current limit.
                    output.CanvasJS.data.Add(new CanvasJSData
                    {
                        name = limit.Level.Name,
                        type = (isFirst && limit.Direction == Direction.Low) ?  "area" : "rangeArea",
                        lineDashType = "solid",
                        lineThickness = 0,
                        markerColor = ChartColors.CriticalBackground(limit.Level.Criticality),
                        markerType = "none",
                        xValueType = "dateTime",
                        color = ChartColors.CriticalBackground(limit.Level.Criticality),
                        showInLegend = true,
                        legendText = string.Format("{0}-", limit.Level.Criticality) + limit.Level.Name + "-" 
                            + localize.GetString(limit.Direction == Direction.Low ? "DirectionLow" : "DirectionHigh"),
                        dataPoints = new List<CanvasJSDataPoints>()
                    });

                    // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                    // C# datetime is in ticks (milliseconds) since January 1, 0000
                    int index = output.CanvasJS.data.Count - 1;

                    if( isFirst && limit.Direction == Direction.Low )
                    {
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = minTimestamp.ToJavaScriptMilliseconds(), y = limit.Value });
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = maxTimestamp.ToJavaScriptMilliseconds(), y = limit.Value });
                    }
                    else if( limit.Direction == Direction.Low )
                    {
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                        { x = minTimestamp.ToJavaScriptMilliseconds(), y = lastLimit.Value, z = limit.Value });
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                        { x = maxTimestamp.ToJavaScriptMilliseconds(), y = lastLimit.Value, z = limit.Value });
                    }
                    else // limit.Direction == Direction.High
                    {
                        if( lastHighIndex >= 0 )
                        {
                            output.CanvasJS.data[lastHighIndex].dataPoints[0].z = limit.Value;
                            output.CanvasJS.data[lastHighIndex].dataPoints[1].z = limit.Value;
                        }

                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                        { x = minTimestamp.ToJavaScriptMilliseconds(), y = limit.Value, z = maxLimit });
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                        { x = maxTimestamp.ToJavaScriptMilliseconds(), y = limit.Value, z = maxLimit });

                        lastHighIndex = index;
                    }

                    isFirst = false;
                    lastLimit = limit;
                }
            }
            return output;
        }

        public GetVariableLimitStatusOutput GetVariableLimitStatus(GetVariableLimitStatusInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");
            string[] localizedDirectionNames = new string[4] 
            {
                localize.GetString("DirectionNone"),
                localize.GetString("DirectionLow"),
                localize.GetString("DirectionFocus"),
                localize.GetString("DirectionHigh")
            };

            // Get all the variable/limit combinations that match the input
            List<IOWLimit> limits = _iowManager.GetAllLimits(input.VariableName, input.LevelName);

            // Transform into our output format
            List<VariableLimitStatusDto> output = new List<VariableLimitStatusDto>();
            foreach( IOWLimit limit in limits )
            {
                double hoursSinceLastDeviation = -1;
                string severityMessage1 = "";
                string severityMessage2 = "";
                string severityClass = "";
                if (limit.LastStatus == IOWStatus.OpenDeviation)
                {
                    hoursSinceLastDeviation = 0;
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgActive");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                    else if (limit.Level.Criticality == 3)
                        severityClass = "label label-default";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue && (DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalHours <= 24)
                {
                    hoursSinceLastDeviation = (DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalHours;
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgLast24Hours");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue)
                {
                    hoursSinceLastDeviation = (DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalHours;
                    double days = Math.Round((DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalDays,0);
                    severityMessage1 = "";
                    severityMessage2 = String.Format(localize.GetString("IowMsgNotRecent"), days);
                    severityClass = "";
                }

                output.Add(new VariableLimitStatusDto
                {
                    VariableId = limit.Variable.Id,
                    VariableName = limit.Variable.Name,
                    VariableDescription = limit.Variable.Description,
                    TagId = limit.Variable.TagId,
                    TagName = limit.Variable.Tag.Name,
                    UOM = limit.Variable.UOM,
                    LastTimestamp = limit.Variable.Tag.LastTimestamp,
                    LastValue = limit.Variable.Tag.LastValue,
                    LastQuality = limit.Variable.Tag.LastQuality,

                    IOWLevelId = limit.IOWLevelId,
                    LevelName = limit.Level.Name,
                    LevelDescription = limit.Level.Description,
                    Criticality = limit.Level.Criticality,
                    ResponseGoal = limit.Level.ResponseGoal,
                    MetricGoal = limit.Level.MetricGoal,

                    LimitName = string.Format("{0}-", limit.Level.Criticality) + limit.Level.Name + "-" + localizedDirectionNames[Convert.ToInt32(limit.Direction)],
                    Direction = limit.Direction,
                    LimitValue = limit.Value,
                    Cause = limit.Cause,
                    Consequences = limit.Consequences,
                    Action = limit.Action,

                    LastStatus = limit.LastStatus,
                    LastDeviationStartTimestamp = limit.LastDeviationStartTimestamp,
                    LastDeviationEndTimestamp = limit.LastDeviationEndTimestamp,
                    HoursinceLastDeviation = hoursSinceLastDeviation, 

                    SeverityMessage1 = severityMessage1,
                    SeverityMessage2 = severityMessage2,
                    SeverityClass = severityClass
                });
            }
            return new GetVariableLimitStatusOutput { variablelimits = output };
        }

        public GetRecentlyDeviatingLimitsOutput GetRecentlyDeviatingLimits(GetRecentlyDeviatingLimitsInput input)
        {
            var localize = _localizationManager.GetSource("AssetManager");
            string[] localizedDirectionNames = new string[4]
            {
                localize.GetString("DirectionNone"),
                localize.GetString("DirectionLow"),
                localize.GetString("DirectionFocus"),
                localize.GetString("DirectionHigh")
            };

            List<IOWLimit> limits = _iowManager.GetProblematicLimits(input.MaxCriticality, input.HoursBack);
            // Transform into our output format
            List<VariableLimitStatusDto> output = new List<VariableLimitStatusDto>();
            foreach (IOWLimit limit in limits)
            {
                string severityMessage1 = "";
                string severityMessage2 = "";
                string severityClass = "";
                if (limit.LastStatus == IOWStatus.OpenDeviation)
                {
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgActive");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                    else if (limit.Level.Criticality == 3)
                        severityClass = "label label-default";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue && (DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalHours <= 24)
                {
                    severityMessage1 = limit.Level.Name;
                    severityMessage2 = localize.GetString("IowMsgLast24Hours");
                    if (limit.Level.Criticality == 1)
                        severityClass = "label label-danger";
                    else if (limit.Level.Criticality == 2)
                        severityClass = "label label-warning";
                }
                else if (limit.LastDeviationEndTimestamp.HasValue)
                {
                    double days = Math.Round((DateTime.Now - limit.LastDeviationEndTimestamp.Value).TotalDays, 0);
                    severityMessage1 = "";
                    severityMessage2 = String.Format(localize.GetString("IowMsgNotRecent"), days);
                    severityClass = "";
                }

                output.Add(new VariableLimitStatusDto
                {
                    VariableId = limit.Variable.Id,
                    VariableName = limit.Variable.Name,
                    VariableDescription = limit.Variable.Description,
                    TagId = limit.Variable.TagId,
                    TagName = limit.Variable.Tag.Name,
                    UOM = limit.Variable.UOM,
                    LastTimestamp = limit.Variable.Tag.LastTimestamp,
                    LastValue = limit.Variable.Tag.LastValue,
                    LastQuality = limit.Variable.Tag.LastQuality,

                    IOWLevelId = limit.IOWLevelId,
                    LevelName = limit.Level.Name,
                    LevelDescription = limit.Level.Description,
                    Criticality = limit.Level.Criticality,
                    ResponseGoal = limit.Level.ResponseGoal,
                    MetricGoal = limit.Level.MetricGoal,

                    LimitName = string.Format("{0}-", limit.Level.Criticality) + limit.Level.Name + "-" + localizedDirectionNames[Convert.ToInt32(limit.Direction)],
                    Direction = limit.Direction,
                    LimitValue = limit.Value,
                    Cause = limit.Cause,
                    Consequences = limit.Consequences,
                    Action = limit.Action,

                    LastStatus = limit.LastStatus,
                    LastDeviationStartTimestamp = limit.LastDeviationStartTimestamp,
                    LastDeviationEndTimestamp = limit.LastDeviationEndTimestamp,

                    SeverityMessage1 = severityMessage1,
                    SeverityMessage2 = severityMessage2,
                    SeverityClass = severityClass
                });
            }

            return new GetRecentlyDeviatingLimitsOutput
            {
                variablelimits = output
            };

        }
    }
}
