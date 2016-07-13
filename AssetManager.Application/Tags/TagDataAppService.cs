using Abp.Application.Services;
using Abp.AutoMapper;
using AssetManager.DomainServices;
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
        private readonly ITagManager _tagManager;

        public TagDataAppService(ITagManager tagManager)
        {
            _tagManager = tagManager;
        }

        public GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input)
        {
            GetTagDataRawOutput output = null;
            List<TagDataRaw> data = null;

            //Get the raw data for the tag. This will return null if the tag does not exist OR there aren't any data.
            if (input.Id.HasValue)
                data = _tagManager.GetAllListData(input.Id.Value);
            else if( !string.IsNullOrEmpty(input.Name))
                data = _tagManager.GetAllListData(input.Name);

            if (data != null && data.Count > 0)
            {
                Tag tag = data[0].Tag;
                output = new GetTagDataRawOutput
                {
                    Name = tag.Name,
                    Description = tag.Description,
                    UOM = tag.UOM,
                    Precision = tag.Precision,
                    TagDataRaw = data.MapTo<List<TagDataRawDto>>()
                };
            }
            return output;
        }

        public GetTagDataChartOutput GetTagDataChart(GetTagDataChartInput input)
        {
            GetTagDataChartOutput output = null;

            //Get the raw data for the tag. This will return null if the tag does not exist OR there aren't any data.
            List<TagDataRaw> data = null;
            if (input.Id.HasValue)
                data = _tagManager.GetAllListData(input.Id.Value, input.StartTimestamp, input.EndTimestamp);
            else if (!string.IsNullOrEmpty(input.Name))
                data = _tagManager.GetAllListData(input.Name, input.StartTimestamp, input.EndTimestamp);

            //If we got some data exist, work on the chart format.
            if (data != null && data.Count > 0)
            {
                Tag tag = data[0].Tag;
                output = new GetTagDataChartOutput
                {
                    Name = tag.Name,
                    Description = tag.Description,
                    UOM = tag.UOM,
                    Precision = tag.Precision,
                    TagDataChart = new List<TagDataChartDto>()
                };

                // Convert the entity data into the necessary charting format
                foreach (TagDataRaw d in data )
                {
                    // Charting time is in JavaScript ticks (milliseconds) since January 1, 1970
                    // C# datetime is in ticks (milliseconds) since January 1, 0000
                    output.TagDataChart.Add(new TagDataChartDto { x = d.Timestamp.ToJavaScriptMilliseconds(), y = d.Value });
                }

                // Calculate and store the time span
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
                output.MinTimestampJS = minTimestamp.ToJavaScriptMilliseconds();
                output.MaxTimestampJS = maxTimestamp.ToJavaScriptMilliseconds();
            }
            return output;
        }

        public GetTagDataCanvasJSOutput GetTagDataCanvasJS(GetTagDataCanvasJSInput input)
        {
            GetTagDataCanvasJSOutput output = new GetTagDataCanvasJSOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
                Precision = null,
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

            //Get the tag information and confirm that the tag exists
            Tag tag = null;
            if (input.Id.HasValue)
                tag = _tagManager.FirstOrDefaultTag(input.Id.Value);
            else if (!string.IsNullOrEmpty(input.Name))
                tag = _tagManager.FirstOrDefaultTag(input.Name);

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
                    lineThickness = tagType == TagType.Continuous ? 2 : 0,
                    markerColor = "rgba(0,75,141,0.7)",
                    markerType = tagType == TagType.Continuous ? "none" : "circle",
                    xValueType = "dateTime",
                    color = "rgba(0,75,141,0.7)",
                    dataPoints = new List<CanvasJSDataPoints>()
                });

                // Get data in the desired range
                List<TagDataRaw> data = _tagManager.GetAllListData(tag.Id, input.StartTimestamp, input.EndTimestamp);

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
                    DateTime startTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : data[0].Timestamp;
                    DateTime endTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : data[data.Count - 1].Timestamp;
                    TimeSpan tsRange = endTimestamp - startTimestamp;
                    if (tsRange.TotalDays > 1)
                    {
                        // Round the start back to the start of day
                        startTimestamp = new DateTime(startTimestamp.Year, startTimestamp.Month, startTimestamp.Day, 0, 0, 0);
                        valueFormatString = "DD-MMM HH:mm";
                    }
                    else if (tsRange.TotalHours > 1)
                    {
                        // Round the start back to the start of the hour
                        startTimestamp = new DateTime(startTimestamp.Year, startTimestamp.Month, startTimestamp.Day, startTimestamp.Hour, 0, 0);
                        valueFormatString = "HH:mm";
                    }

                    output.CanvasJS.axisX.minimum = startTimestamp.ToJavaScriptMilliseconds();
                    output.CanvasJS.axisX.maximum = endTimestamp.ToJavaScriptMilliseconds();
                    output.CanvasJS.axisX.valueFormatString = valueFormatString;
                }
            }

            return output;
        }

        public TagDataRawDto AddTagDataRaw(AddTagDataRawInput input)
        {
            TagDataName revised = new TagDataName
            {

                TagId = input.Id,
                TagName = input.Name,
                Timestamp = input.Timestamp,
                Value = input.Value,
                Quality = input.Quality
            };
            TagDataRaw data = _tagManager.InsertOrUpdateDataByName(revised);
            TagDataRawDto output = data.MapTo<TagDataRawDto>();

            return output;
        }

        public PostTagDataBulkOutput PostTagDataBulk(PostTagDataBulkInput input)
        {
            PostTagDataBulkOutput output = new PostTagDataBulkOutput
            {
                Total = input.TagDataName.Count,
                Successes = 0
            };

            output.Successes = (int)_tagManager.InsertOrUpdateAllDataByName(input.TagDataName);
            return output;
        }

        public GetTagDataOutput GetTagData(GetTagDataInput input)
        {
            GetTagDataOutput output = new GetTagDataOutput { TagData = new List<TagDataDto>() };

            // Input timestamp - if not specified, use "now"
            DateTime timestamp = input.Timestamp.HasValue ? input.Timestamp.Value : DateTime.Now;

            // Validate each tag in the input list. Could probably do this in one query....
            /*
            List<long> tagIds = new List<long>();
            Tag tag = null;
            if( input.Tags != null && input.Tags.Count > 0 )
            {
                foreach(TagNameDto t in input.Tags)
                {
                    if (t.Id > 0)
                        tag = _tagManager.FirstOrDefaultTag(t.Id);
                    else if (!string.IsNullOrEmpty(t.Name))
                        tag = _tagManager.FirstOrDefaultTag(t.Name);
                    if (tag != null)
                        tagIds.Add(tag.Id);
                }
            }
             */

            // Alternate approach to validate all tags in the input list in one query.
            var query = (from a in _tagManager.GetAllListTag()
                         join b in input.Tags on a.Id equals b.Id
                         where b.Id > 0
                         select a)
                        .Union
                        (from a in _tagManager.GetAllListTag()
                         join b in input.Tags on a.Name equals b.Name
                         where b.Id <= 0 && !String.IsNullOrEmpty(b.Name)
                         select a);
            var allTags = query.Distinct().OrderBy(t => t.Name).ToList();
            List<long> tagIds = allTags.Select(t => t.Id).ToList();

            // Get the data (at or before the specified time) and build the output
            if( tagIds != null && tagIds.Count > 0 )
            {
                List<TagDataRaw> rawData = _tagManager.GetTagDataAtTime(tagIds, timestamp);
                if( rawData != null && rawData.Count > 0 )
                {
                    foreach(TagDataRaw raw in rawData)
                    {
                        output.TagData.Add(new TagDataDto
                        {
                            Id = raw.TagId,
                            Name = raw.Tag.Name,
                            Description = raw.Tag.Description,
                            UOM = raw.Tag.UOM,
                            Precision = raw.Tag.Precision,
                            Type = raw.Tag.Type,
                            Timestamp = raw.Timestamp,
                            Value = raw.Value,
                            Quality = raw.Quality
                        });
                    }
                }
            }
            return output;
        }
    }
}
