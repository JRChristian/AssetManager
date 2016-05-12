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
    public class IowVariableAppService : AssetManagerAppServiceBase, IIowVariableAppService
    {
        private readonly IIowManager _iowManager;
        private readonly ITagManager _tagManager;

        public IowVariableAppService(
            IIowManager iowManager,
            ITagManager tagManager)
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
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
                Limits = new List<LimitDto>()
            };

            // Get limits currently in use for this variable, sorted by name
            List<IOWLimit> limits = variable.IOWLimits.OrderBy(p => p.Level.Id).ToList();

            // Get all available levels defined at this site, sorted by name
            List<IOWLevel> levels = _iowManager.GetAllLevels().OrderBy(p => p.Id).ToList();

            // Now build the output the hard way, since we have our own logic.
            // "j" is our counter into the list of limits. This list will have between zero items
            // and the number of items in the level list.
            j = limits.Count > 0 ? 0 : levels.Count + 1;

            for (i = 0; i < levels.Count; i++)
            {
                // Increment j (index into existing limits) to match the level, if possible
                for (; j < limits.Count && limits[j].Level.Id < levels[i].Id; j++) ;

                // If j (the index into the list of limits for this variable) <= i (index into the list of all levels)
                // then the limit matches the level, and this level matches an active limit.
                // Otherwise, this level is not currently active for this variable.
                bool isLimitPresent = (j < limits.Count) && limits[j].Level.Id == levels[i].Id;

                if( isLimitPresent || input.IncludeUnusedLimits )
                {
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
                        Cause = isLimitPresent ? limits[j].Cause : "",
                        Consequences = isLimitPresent ? limits[j].Consequences : "",
                        Action = isLimitPresent ? limits[j].Action : "",
                        LowLimit = isLimitPresent ? limits[j].LowLimit : null,
                        HighLimit = isLimitPresent ? limits[j].HighLimit : null
                    });
                }
            } // for (i = 0; i < levels.Count; i++)
            output.Limits = output.Limits.OrderBy(p => p.Name).ToList();
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
                    Cause = haveLimit ? limits[j].Cause : "",
                    Consequences = haveLimit ? limits[j].Consequences : "",
                    Action = haveLimit ? limits[j].Action : "",
                    LowLimit = haveLimit ? limits[j].LowLimit : null,
                    HighLimit = haveLimit ? limits[j].HighLimit : null
                });
            }

            return new GetAllLimitsOutput
            {
                Limits = new List<LimitDto>(output)
            };
        }

        public LimitDto UpdateLimit(UpdateLimitInput input)
        {
            // This method accepts one limit for one variable.
            // If the limit does not exist, it is added.
            // If the limit exists, it is changed or deleted.

            LimitDto output = null;
            bool inputIsValid = false;
            bool isActive = input.IsActive.HasValue ? input.IsActive.Value : true;

            // Look for the variable.
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(input.IowVariableId, input.IowVariableName);

            if( variable != null )
            {

                // Does the specified limit exist? If so, get it. This will return null if nothing is found.
                IOWLimit limit = _iowManager.FirstOrDefaultLimit(variable.Id, input.Id, input.Name);

                // Does the specified level exist? It should.
                IOWLevel level = _iowManager.FirstOrDefaultLevel(input.IOWLevelId, input.Name);

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
                            LastDeviationStartDate = null,
                            LastDeviationEndDate = null,
                            TenantId = variable.TenantId
                        };

                    limit.Cause = input.Cause;
                    limit.Consequences = input.Consequences;
                    limit.Action = input.Action;

                    double? lowLimit = null, highLimit = null;
                    if (input.LowLimit.HasValue)
                        lowLimit = input.LowLimit.Value;
                    if (input.HighLimit.HasValue)
                        highLimit = input.HighLimit.Value;
                    limit.LowLimit = lowLimit;
                    limit.HighLimit = highLimit;

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
                        Cause = limit.Cause,
                        Consequences = limit.Consequences,
                        Action = limit.Action,
                        LowLimit = limit.LowLimit,
                        HighLimit = limit.HighLimit
                    };
            }
            return output;
        }

        public GetIowChartCanvasJSOutput GetIowChartCanvasJS(GetIowChartCanvasJSInput input)
        {
            string[] colors =
            {
                    "rgba(0,75,141,0.7)",   // 0 - blue
                    "rgba(255,0,0,0.7)",    // 1 - red
                    "rgba(255,102,0,0.7)",  // 2 - orange
                    "darkgreen",            // 3 - green
                    "rgba(51,51,51,0.7)",   // 4 - gray
                    "rgba(100,100,100,0.7)" // 5 - lighter gray
                };

            GetIowChartCanvasJSOutput output = new GetIowChartCanvasJSOutput
            {
                Name = input.Name,
                Description = "",
                TagId = 0,
                TagName = "",
                UOM = "",
                CanvasJS = new CanvasJSDto
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
                    markerColor = colors[0],
                    markerType = tagType == TagType.Continuous ? "none" : "circle",
                    xValueType = "dateTime",
                    color = colors[0],
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

                // Add some limit information to the chart
                foreach( IOWLimit limit in variable.IOWLimits)
                {
                    // Get the color for this limit from the colors array.
                    string color = limit.Level.Criticality > 0 && limit.Level.Criticality < colors.Length
                        ? colors[limit.Level.Criticality] : colors[colors.Length-1];

                    if(limit.HighLimit.HasValue)
                    {
                        output.CanvasJS.data.Add(new CanvasJSData
                        {
                            name = limit.Level.Name,
                            type = "line",
                            lineDashType = "solid",
                            markerColor = color,
                            markerType = "none",
                            xValueType = "dateTime",
                            color = color,
                            showInLegend = true,
                            legendText = limit.Level.Name + "- high",
                            dataPoints = new List<CanvasJSDataPoints>()
                        });

                        // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                        // C# datetime is in ticks (milliseconds) since January 1, 0000
                        int index = output.CanvasJS.data.Count - 1;
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = minTimestamp.ToJavaScriptMilliseconds(), y = limit.HighLimit.Value });
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = maxTimestamp.ToJavaScriptMilliseconds(), y = limit.HighLimit.Value });
                    }

                    if (limit.LowLimit.HasValue)
                    {
                        output.CanvasJS.data.Add(new CanvasJSData
                        {
                            name = limit.Level.Name,
                            type = "line",
                            lineDashType = "dash",
                            markerColor = color,
                            markerType = "none",
                            xValueType = "dateTime",
                            color = color,
                            showInLegend = true,
                            legendText = limit.Level.Name + "- low",
                            dataPoints = new List<CanvasJSDataPoints>()
                        });

                        // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                        // C# datetime is in ticks (milliseconds) since January 1, 0000
                        int index = output.CanvasJS.data.Count - 1;
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = minTimestamp.ToJavaScriptMilliseconds(), y = limit.LowLimit.Value });
                        output.CanvasJS.data[index].dataPoints.Add(new CanvasJSDataPoints
                            { x = maxTimestamp.ToJavaScriptMilliseconds(), y = limit.LowLimit.Value });
                    }
                }
            }
            return output;
        }
    }
}
