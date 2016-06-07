using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Utilities
{
    // For CanvasJS Bar Charts, axisX is Vertical and axisY is Horizontal
    public class CanvasJSVerticalBar
    {
        public bool? exportEnabled { get; set; }
        public CanvasJSVerticalBarTitle title { get; set; }
        public CanvasJSVerticalBarAxisX axisX { get; set; }
        public CanvasJSVerticalBarAxisY axisY { get; set; }
        public List<CanvasJSVerticalBarData> data { get; set; }
    }

    public class CanvasJSVerticalBarTitle
    {
        public string text { get; set; }
    }

    public class CanvasJSVerticalBarAxisX
    {
        public string title { get; set; }
        public int interval { get; set; }
    }

    public class CanvasJSVerticalBarAxisY
    {
        public string title { get; set; }
        public double minimum { get; set; }
        public double maximum { get; set; }
    }
    public class CanvasJSVerticalBarData
    {
        public string name { get; set; }
        public string type { get; set; }
        public string legendText { get; set; }
        public bool? showInLegend { get; set; }
        public string color { get; set; }
        public List<CanvasJSVerticalBarDataPoints> dataPoints { get; set; }
    }

    public class CanvasJSVerticalBarDataPoints
    {
        public double y { get; set; }
        public string label { get; set; }
    }
}
