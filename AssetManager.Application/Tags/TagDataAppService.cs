using Abp.Application.Services;
using Abp.AutoMapper;
using AssetManager.Entities;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public class TagDataAppService : AssetManagerAppServiceBase, ITagDataAppService
    {
        //These members set in constructor using constructor injection.
        private readonly ITagRepository _tagRepository;
        private readonly ITagDataRawRepository _tagDataRawRepository;

        public TagDataAppService(ITagRepository tagRepository, ITagDataRawRepository tagDataRawRepository)
        {
            _tagRepository = tagRepository;
            _tagDataRawRepository = tagDataRawRepository;
        }

        public GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input)
        {
            Tag tag = null;
            List<TagDataRaw> data = null;
            GetTagDataRawOutput output = new GetTagDataRawOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
                Precision = null,
                TagDataRaw = new List<TagDataRawDto>()
            };

            //Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            //If the tag exists, save the description and get values in the relevant time range
            if (tag != null)
            {
                output.Name = tag.Name;
                output.Description = tag.Description;
                output.UOM = tag.UOM;
                output.Precision = tag.Precision;

                // Get data in the desired range
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderByDescending(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value).OrderByDescending(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp <= input.EndTimestamp).OrderByDescending(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id).OrderByDescending(t => t.Timestamp).ToList();

                output.TagDataRaw = data.MapTo<List<TagDataRawDto>>();
            }

            return output;
        }

        public GetTagDataChartOutput GetTagDataChart(GetTagDataChartInput input)
        {
            Tag tag = null;
            GetTagDataChartOutput output = new GetTagDataChartOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
                Precision = null,
                TagDataChart = new List<TagDataChartDto>()
            };
            List<TagDataRaw> data = null;

            //Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else if( !string.IsNullOrEmpty(input.Name) )
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);
            
            //If the tag exists, save the description and get values in the relevant time range
            if (tag != null)
            {
                output.Name = tag.Name;
                output.Description = tag.Description;
                output.UOM = tag.UOM;
                output.Precision = tag.Precision;

                // Get data in the desired range
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id).OrderBy(t => t.Timestamp).ToList();

                // Convert the entity data into the necessary charting format
                foreach (TagDataRaw d in data )
                {
                    // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                    // C# datetime is in ticks (milliseconds) since January 1, 0000
                    //TimeSpan ts = d.Timestamp - new DateTime(1970, 1, 1);
                    //output.TagDataChart.Add(new TagDataChartDto { x = ts.TotalMilliseconds, y = d.Value });
                    output.TagDataChart.Add(new TagDataChartDto { x = d.Timestamp.ToJavaScriptMilliseconds(), y = d.Value });
                }

                if( data != null && data.Count > 0 )
                {
                    DateTime minTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : data[0].Timestamp;
                    DateTime maxTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : data[data.Count-1].Timestamp;
                    TimeSpan tsRange = maxTimestamp - minTimestamp;
                    if( tsRange.TotalDays > 1 )
                    {
                        // Round the start back to the start of day
                        minTimestamp = new DateTime(minTimestamp.Year, minTimestamp.Month, minTimestamp.Day, 0, 0, 0);
                    }
                    else if( tsRange.TotalHours > 1 )
                    {
                        // Round the start back to the start of the hour
                        minTimestamp = new DateTime(minTimestamp.Year, minTimestamp.Month, minTimestamp.Day, minTimestamp.Hour, 0, 0);
                    }

                    //TimeSpan ts = minTimestamp - new DateTime(1970, 1, 1);
                    //output.MinTimestampJS = ts.TotalMilliseconds;
                    output.MinTimestampJS = minTimestamp.ToJavaScriptMilliseconds();

                    //ts = maxTimestamp - new DateTime(1970, 1, 1);
                    //output.MaxTimestampJS = ts.TotalMilliseconds;
                    output.MaxTimestampJS = maxTimestamp.ToJavaScriptMilliseconds();
                }
            }

            return output;
        }

        public GetTagDataCanvasJSOutput GetTagDataCanvasJS(GetTagDataCanvasJSInput input)
        {
            Tag tag = null;
            GetTagDataCanvasJSOutput output = new GetTagDataCanvasJSOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
                Precision = null,
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
            List<TagDataRaw> data = null;

            //Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else if (!string.IsNullOrEmpty(input.Name))
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            //If the tag exists, save the description and get values in the relevant time range
            if (tag != null)
            {
                output.Name = tag.Name;
                output.Description = tag.Description;
                output.UOM = tag.UOM;
                output.Precision = tag.Precision;
                TagType tagType = tag.Type.HasValue ? tag.Type.Value : TagType.Continuous;

                output.CanvasJS.title.text = tag.Name;
                output.CanvasJS.axisX.gridThickness = 0;
                output.CanvasJS.axisY.gridThickness = 1;
                output.CanvasJS.axisY.title = tag.UOM;
                output.CanvasJS.axisY.includeZero = false;
                output.CanvasJS.data.Add(new CanvasJSData
                {
                    name = tag.Name,
                    type = tagType == TagType.Continuous ? "line" : "scatter",
                    lineDashType = "solid",
                    markerColor = "rgba(0,75,141,0.7)",
                    markerType = tagType == TagType.Continuous ? "none" : "circle",
                    xValueType = "dateTime",
                    color = "rgba(0,75,141,0.7)",
                    dataPoints = new List<CanvasJSDataPoints>()
                });

                // Get data in the desired range
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id).OrderBy(t => t.Timestamp).ToList();

                // Convert the entity data into the necessary charting format
                foreach (TagDataRaw d in data)
                {
                    // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                    // C# datetime is in ticks (milliseconds) since January 1, 0000
                    output.CanvasJS.data[0].dataPoints.Add(new CanvasJSDataPoints { x = d.Timestamp.ToJavaScriptMilliseconds(), y = d.Value });
                }

                if (data != null && data.Count > 0)
                {
                    string valueFormatString = null;
                    DateTime minTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : data[0].Timestamp;
                    DateTime maxTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : data[data.Count - 1].Timestamp;
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
                }
            }

            return output;
        }


        /*public Task<GetTagDataRawOutput> GetTagDataRawListAsync(GetTagDataRawInput input)
        {

        }*/

        public TagDataRawDto AddTagDataRaw(AddTagDataRawInput input)
        {
            Tag tag = null;
            TagDataRawDto output = new TagDataRawDto
            {
                Id = 0,
                Timestamp = DateTime.Now,
                Value = 0,
                Quality = TagDataQuality.Bad
            };

            // Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            // If the tag exists, add or update the value
            if (tag != null)
            {
                TagDataRaw data = null;
                DateTime timestamp = input.Timestamp.HasValue ? input.Timestamp.Value : DateTime.Now;

                // Find out if there is already a tag with this value
                data = _tagDataRawRepository.FirstOrDefault(p => p.TagId == tag.Id && p.Timestamp == timestamp);

                if( data == null)
                {
                    // Did not found a match, so add all entries
                    data = new TagDataRaw
                        {
                            TenantId = tag.TenantId,
                            TagId = tag.Id,
                            Timestamp = timestamp,
                            Value = input.Value,
                            Quality = input.Quality.HasValue ? input.Quality.Value : TagDataQuality.Good
                        };
                }
                else
                {
                    // Found the record, so update only what has changed
                    data.Value = input.Value;
                    if( input.Quality.HasValue )
                        data.Quality = input.Quality.Value;
                }

                data.Id = _tagDataRawRepository.InsertOrUpdateAndGetId(data);

                output = data.MapTo<TagDataRawDto>();
            }

            return output;
        }

        public PostTagDataBulkOutput PostTagDataBulk(PostTagDataBulkInput input)
        {
            PostTagDataBulkOutput output = new PostTagDataBulkOutput { Total = 0, Successes = 0 };
            Tag tag = null;

            foreach( TagDataBulkDto one in input.TagDataRaw )
            {
                output.Total++;

                // Get a tag id for the tag on this record.
                // If the tag is the same as the previous record, no need to get it again.
                if ( tag == null || one.Name != tag.Name )
                    tag = _tagRepository.FirstOrDefault(p => p.Name == one.Name);

                if ( tag != null )
                {
                    TagDataRaw data = new TagDataRaw
                    {
                        TenantId = tag.TenantId,
                        TagId = tag.Id,
                        Timestamp = one.Timestamp,
                        Value = one.Value,
                        Quality = one.Quality.HasValue ? one.Quality.Value : TagDataQuality.Good
                    };

                    data.Id = _tagDataRawRepository.InsertOrUpdateAndGetId(data);

                    if (data.Id > 0)
                        output.Successes++;
                }
            }

            return output;
        }

    }
}
