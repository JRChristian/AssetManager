using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
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
        private readonly IIOWVariableRepository _iowVariableRepository;
        private readonly IIOWLimitRepository _iowLimitRepository;
        private readonly IIOWLevelRepository _iowLevelRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagDataRawRepository _tagDataRawRepository;

        public IowVariableAppService(
            IIOWVariableRepository iowVariableRepository, 
            IIOWLimitRepository iowLimitRepository, 
            IIOWLevelRepository iowLevelRepository, 
            ITagRepository tagRepository, 
            ITagDataRawRepository tagDataRawRepository)
        {
            _iowVariableRepository = iowVariableRepository;
            _iowLimitRepository = iowLimitRepository;
            _iowLevelRepository = iowLevelRepository;
            _tagRepository = tagRepository;
            _tagDataRawRepository = tagDataRawRepository;
        }


        public IowVariableDto GetOneIowVariable(GetOneIowVariableInput input)
        {
            IOWVariable variable = null;

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if (!string.IsNullOrEmpty(input.Name))
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            return variable.MapTo<IowVariableDto>();
        }

        public GetIowVariableOutput GetIowVariables(GetIowVariableInput input)
        {
            List<IOWVariable> variables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if( !string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if( !string.IsNullOrEmpty(input.Name) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if( !string.IsNullOrEmpty(input.TagName) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else
            {
                variables = _iowVariableRepository.GetAll()
                    .OrderBy(v => v.Name)
                    .ToList();
            }

            return new GetIowVariableOutput
            {
                IowVariables = variables.MapTo<List<IowVariableDto>>()
            };
        }

        public async Task<GetIowVariableOutput> GetIowVariablesAsync(GetIowVariableInput input)
        {
            List<IOWVariable> variables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if (!string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName))
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName));
            }
            else if( !string.IsNullOrEmpty(input.Name) )
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Name.Contains(input.Name));
            }
            else if( !string.IsNullOrEmpty(input.TagName) )
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Tag.Name.Contains(input.TagName));
            }
            else
            {
                variables = await _iowVariableRepository.GetAllListAsync();
            }

            var sorted = variables.OrderBy(p => p.Name);

            return new GetIowVariableOutput
            {
                IowVariables = sorted.MapTo<List<IowVariableDto>>()
            };
        }

        public GetIowVariableLimitOutput GetIowVariableLimits(GetIowVariableInput input)
        {
            List<IOWVariable> variables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if (!string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName))
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(input.Name))
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(input.TagName))
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else
            {
                variables = _iowVariableRepository.GetAll()
                    .OrderBy(v => v.Name)
                    .ToList();
            }

            return new GetIowVariableLimitOutput
            {
                IowVariables = variables.MapTo<List<IowVariableLimitDto>>()
            };
        }


        public IowVariableDto CreateIowVariable(CreateIowVariableInput input)
        {
            Logger.Info("Creating a variable for input: " + input.Name);

            Tag tag = null;
            IOWVariable variable = null;

            // Check to see if a variable by this name already exists
            variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);
            if( variable == null )
            {
                // All variables belong to a tenant. If not specified, put them in the default tenant.
                int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

                // Create a new variable object from the input
                variable = new IOWVariable
                {
                    Name = input.Name,
                    Description = input.Description,
                    UOM = input.UOM,
                    TenantId = tenantid
                };

                // Having a tag is optional. The caller may have specified a tag id or name, or neither.
                // If a tag is specified but not found, ignore it.
                if (input.TagId.HasValue)
                    tag = _tagRepository.FirstOrDefault(p => p.Id == input.TagId);
                else if ( !string.IsNullOrEmpty(input.TagName) )
                    tag = _tagRepository.FirstOrDefault(p => p.Name == input.TagName);

                if (tag != null)
                {
                    variable.TagId = tag.Id;
                    // If UOM was not specified for the variable AND the tag has a UOM, then use the tag's UOM
                    if (string.IsNullOrEmpty(variable.UOM) && !string.IsNullOrEmpty(tag.UOM))
                        variable.UOM = tag.UOM;
                }

                // Add the new variable to the repository
                variable.Id = _iowVariableRepository.InsertAndGetId(variable);
            }
            return variable.MapTo<IowVariableDto>();
        }

        public IowVariableDto DeleteIowVariable(GetOneIowVariableInput input)
        {
            IOWVariable variable = null;

            Logger.Info("Delete a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if ( !string.IsNullOrEmpty(input.Name) )
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            if (variable != null)
                _iowVariableRepository.Delete(variable);

            return variable.MapTo<IowVariableDto>();
        }

        public IowVariableDto UpdateIowVariable(UpdateIowVariableInput input)
        {
            IOWVariable variable = null;
            Tag tag = null;

            Logger.Info("Update a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if (input.Name != "")
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            // If we find a variable, update its properties
            if (variable != null)
            {
                if (!string.IsNullOrEmpty(input.Name))
                    variable.Name = input.Name;

                if (!string.IsNullOrEmpty(input.Description))
                    variable.Description = input.Description;
                
                // If tag information (id or name) was specified, then look for this tag
                if (input.TagId.HasValue && input.TagId.Value != variable.TagId)
                    variable.TagId = input.TagId.Value;
                else if (!string.IsNullOrEmpty(input.TagName) && input.TagName != variable.Tag.Name)
                {
                    tag = _tagRepository.FirstOrDefault(p => p.Name == input.TagName);
                    if (tag != null)
                        variable.TagId = tag.Id;
                }

                // If a not-null UOM was passed in, use it, even if the empty string was passed in. Ignore null inputs.
                if ( input.UOM != null)
                    variable.UOM = input.UOM;
                // If UOM does not exist for the variable AND the tag has a UOM, then use the tag's UOM
                if (string.IsNullOrEmpty(variable.UOM))
                {
                    if (tag != null && !string.IsNullOrEmpty(tag.UOM))
                        variable.UOM = tag.UOM;
                    else if (variable.Tag != null)
                        variable.UOM = variable.Tag.UOM;
               }
            }
            // If we did not find a variable, attempt to create one. This will work if all required fields are specified.
            else
            {
                return CreateIowVariable(new CreateIowVariableInput
                {
                    Name = input.Name,
                    Description = input.Description,
                    TagId = input.TagId.HasValue ? input.TagId.Value : 0,
                    TagName = input.TagName,
                    UOM = input.UOM
                });
            }


            //We do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).

            return variable.MapTo<IowVariableDto>();
        }

        public GetIowLimitOutput GetIowLimits(GetIowLimitInput input)
        {
            IOWVariable variable = null;
            List<IOWLimit> limits = null;
            List<IOWLevel> levels = null;
            List<IowLimitDto> output = new List<IowLimitDto>();
            int i, j;

            // Look for the variable. This will NOT throw an error if the variable id is not found.
            variable = _iowVariableRepository.FirstOrDefault(input.VariableId);

            // Get limits for this variable.
            limits = _iowLimitRepository.GetAll().Where(v => v.IOWVariableId == input.VariableId).OrderBy(c => c.Level.Criticality).ToList();

            // Get all available levels defined at this site.
            levels = _iowLevelRepository.GetAll().OrderBy(c => c.Criticality).ToList();

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
                output.Add(new IowLimitDto
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

            return new GetIowLimitOutput
            {
                Limits = new List<IowLimitDto>(output)
            };
        }

        public IowLimitDto ChangeIowLimits(ChangeIowLimitInput input)
        {
            // This method accepts one limit for one variable.
            // If the limit does not exist, it is added.
            // If the limit exists, it is changed or deleted.

            IOWVariable variable = null;
            IOWLimit limit = null;
            IOWLevel level = null;
            IowLimitDto output = null;
            bool inputIsValid = false;
            bool isActive = input.IsActive.HasValue ? input.IsActive.Value : true;

            // Look for the variable.
            if( input.IOWLevelId.HasValue)
                variable = _iowVariableRepository.Get(input.IowVariableId.Value);
            else if ( !string.IsNullOrEmpty(input.IowVariableName) )
                variable = _iowVariableRepository.FirstOrDefault(x => x.Name == input.IowVariableName);

            if( variable != null )
            {

                // Does the specified limit exist? If so, get it. This will return null if nothing is found.
                if ( input.Id.HasValue )
                {
                    limit = _iowLimitRepository.FirstOrDefault( input.Id.Value );
                    if ( limit != null )
                        level = _iowLevelRepository.FirstOrDefault( limit.IOWLevelId );
                }
                else if ( input.IOWLevelId.HasValue )
                {   
                    limit = _iowLimitRepository.FirstOrDefault( x => x.IOWVariableId == variable.Id && x.IOWLevelId == input.IOWLevelId.Value );
                    if (limit != null)
                        level = _iowLevelRepository.FirstOrDefault(limit.IOWLevelId);
                }
                else if ( !string.IsNullOrEmpty(input.Name) )
                {
                    level = _iowLevelRepository.FirstOrDefault( x => x.Name == input.Name );
                    if ( level != null )
                        limit = _iowLimitRepository.FirstOrDefault(x => x.IOWVariableId == variable.Id && x.IOWLevelId == level.Id );
                }
                else if ( input.Criticality.HasValue )
                {
                    level = _iowLevelRepository.FirstOrDefault(x => x.Criticality == input.Criticality.Value);
                    if (level != null)
                        limit = _iowLimitRepository.FirstOrDefault(x => x.IOWVariableId == variable.Id && x.IOWLevelId == level.Id);
                }

                // There are four possibilities at this point:
                //   1) We did not find a limit and the level is not valid ==> bad input, do nothing
                //   2) We did not find a limit and the level is valid ==> insert a new limit if possible
                //   3) We found a limit and the IsActive flag is true ==> update an existing limit
                //   4) We found a limit and the InActive flag is false ==> delete an existing limit

                if ( limit == null && level == null )
                {
                    // Case 1: Bad input
                }
                else if ( limit == null && level != null && isActive )
                {
                    // Case 2: Limit does not exist, so insert a new limit

                    // All assets belong to a tenant. If not specified, put them in the default tenant.
                    int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

                    double? lowLimit = null, highLimit = null;
                    if (input.LowLimit.HasValue)
                        lowLimit = input.LowLimit.Value;
                    if (input.HighLimit.HasValue)
                        highLimit = input.HighLimit.Value;

                    limit = new IOWLimit
                    {
                        IOWVariableId = variable.Id,
                        IOWLevelId = level.Id,
                        Cause = input.Cause,
                        Consequences = input.Consequences,
                        Action = input.Action,
                        LowLimit = lowLimit,
                        HighLimit = highLimit,
                        LastCheckDate = null,
                        LastStatus = IOWStatus.Normal,
                        LastDeviationStartDate = null,
                        LastDeviationEndDate = null,
                        TenantId = tenantid
                    };

                    limit.Id = _iowLimitRepository.InsertAndGetId(limit);

                    inputIsValid = true;
                }
                else if ( isActive )
                {
                    // Case 3: Limit exists and should be updated.
                    if (!string.IsNullOrEmpty(input.Cause))
                        limit.Cause = input.Cause;

                    if (!string.IsNullOrEmpty(input.Consequences))
                        limit.Consequences = input.Consequences;

                    if (!string.IsNullOrEmpty(input.Action))
                        limit.Action = input.Action;

                    // Replace the low and high limits in all situation, even if incoming values are null
                    double? lowLimit = null, highLimit = null;
                    if (input.LowLimit.HasValue)
                        lowLimit = input.LowLimit.Value;
                    if (input.HighLimit.HasValue)
                        highLimit = input.HighLimit.Value;

                    limit.LowLimit = lowLimit;
                    limit.HighLimit = highLimit;

                    inputIsValid = true;
                }
                else if ( !isActive )
                {
                    // Case 4: Limit exists and should be deleted
                    _iowLimitRepository.Delete(limit.Id);

                    inputIsValid = true;
                }

                if( inputIsValid )
                    output = new IowLimitDto
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

            IOWVariable variable = null;
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
            if (input.Id.HasValue)
                variable = _iowVariableRepository.FirstOrDefault(p => p.Id == input.Id.Value);
            else if (!string.IsNullOrEmpty(input.Name))
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            if( variable != null )
            {
                output.Name = variable.Name;
                output.Description = variable.Description;
                output.TagId = variable.TagId;
                output.TagName = variable.Tag.Name;
                output.UOM = !string.IsNullOrEmpty(variable.UOM) ? variable.UOM : variable.Tag.UOM;
                
                List<TagDataRaw> data = null;

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
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == output.TagId && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == output.TagId && t.Timestamp >= input.StartTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == output.TagId && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == output.TagId).OrderBy(t => t.Timestamp).ToList();

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
