using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    // For CanvasJS Bar Charts, axisX is Vertical and axisY is Horizontal
    public class CanvasJSHorizontalBarDto
    {
        public bool? exportEnabled { get; set; }
        public CanvasJSBarTitle title { get; set; }
        public CanvasJSBarAxisX axisX { get; set; }
        public CanvasJSBarAxisY axisY { get; set; }
        public List<CanvasJSBarData> data { get; set; }
    }

    public class CanvasJSBarTitle
    {
        public string text { get; set; }
    }

    public class CanvasJSBarAxisX
    {
        public string title { get; set; }
        public int interval { get; set; }
    }

    public class CanvasJSBarAxisY
    {
        public string title { get; set; }
    }
    public class CanvasJSBarData
    {
        public string name { get; set; }
        public string type { get; set; }
        public string legendText { get; set; }
        public bool? showInLegend { get; set; }
        public string color { get; set; }
        public List<CanvasJSBarDataPoints> dataPoints { get; set; }
    }

    public class CanvasJSBarDataPoints
    {
        public double? y { get; set; }
        public string label { get; set; }
    }
}
