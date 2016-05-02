using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class CanvasJSDto : EntityDto<long>
    {
        public CanvasJSTitle title { get; set; }
        public CanvasJSAxisX axisX { get; set; }
        public CanvasJSAxisY axisY { get; set; }
        public List<CanvasJSData> data { get; set; }
    }

    public class CanvasJSTitle
    {
        public string text { get; set; }
    }

    public class CanvasJSAxisX
    {
        public int? gridThickness { get; set; }
        public long? maximum { get; set; }
        public long? minimum { get; set; }
        public string title { get; set; }
        public string valueFormatString { get; set; }
    }

    public class CanvasJSAxisY
    {
        public int? gridThickness { get; set; }
        public long? maximum { get; set; }
        public long? minimum { get; set; }
        public string title { get; set; }
    }
    public class CanvasJSData
    {
        public string name { get; set; }
        public string type { get; set; }
        public string markerType { get; set; }
        public string xValueType { get; set; }
        public string color { get; set; }
        public List<CanvasJSDataPoints> dataPoints { get; set; }
    }

    public class CanvasJSDataPoints
    {
        public double x { get; set; }
        public double? y { get; set; }
    }
}
