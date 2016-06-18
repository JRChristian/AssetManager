using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Utilities
{
    public static class ChartColors
    {
        public static string Criticality(int Criticality)
        {
            string[] colors =
            {
                "rgba(0,75,141,0.9)",   // 0 - blue
                "rgba(178,34,34,0.7)",  // 1 - "firebrick" xB22222
                "rgba(255,102,0,0.7)",  // 2 - orange
                "rgba(0,100,0,0.5)",    // 3 - "darkgreen" #006400
                "rgba(51,51,51,0.5)",   // 4 - gray
                "rgba(100,100,100,0.5)" // 5 - lighter gray
            };

            int colorIndex = Math.Max( 0, Criticality);
            colorIndex = Math.Min(colors.Count(), Criticality);

            return colors[colorIndex];
        }
    }
}
