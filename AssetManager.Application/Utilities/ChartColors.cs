using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Utilities
{
    public class ChartColors
    {
        public static string CriticalForeground(int Criticality)
        {
            /*
            string[] colors =
            {
                "rgba(0,75,141,0.9)",   // 0 - blue
                "rgba(178,34,34,0.7)",  // 1 - "firebrick" xB22222
                "rgba(255,102,0,0.7)",  // 2 - orange
                "rgba(0,100,0,0.5)",    // 3 - "darkgreen" #006400
                "rgba(51,51,51,0.5)",   // 4 - gray
                "rgba(100,100,100,0.5)" // 5 - lighter gray
            };
             */
            // Scheme is {red, green, blue, intensity}. Intensity is a decimal between 0 and 1, represented here as an integer between 0 and 10.
            int[,] colors =
            {
                {   0,  75, 141, 9 },  // 0 - blue
                { 178,  34,  34, 7 },  // 1 - "firebrick" xB22222
                { 255, 102,   0, 7 },  // 2 - orange
                {  51,  51,  51, 5 },  // 3 - gray
                {   0, 100,   0, 5 },  // 4 - "darkgreen" #006400
                { 100, 100, 100, 5 },  // 5 - lighter gray
                { 255, 255, 255, 5 }   // 6 - white
            };

            // Validate the input
            int colorIndex = Math.Max( 0, Criticality);
            colorIndex = Math.Min(colors.GetLength(0), Criticality);
            double intensity = (double)colors[colorIndex, 3] / 10.0;

            string output = String.Format("rgba({0},{1},{2},{3:0.0})", colors[colorIndex, 0], colors[colorIndex, 1], colors[colorIndex, 2], intensity);
            return output;
        }
        public static string CriticalBackground(int Criticality)
        {
            int[,] colors =
            {
                {   0,  75, 141, 9 },  // 0 - blue
                { 178,  34,  34, 3 },  // 1 - "firebrick" xB22222
                { 255, 102,   0, 3 },  // 2 - orange
                {  51,  51,  51, 3 },  // 3 - gray
                {   0, 100,   0, 3 },  // 4 - "darkgreen" #006400
                { 100, 100, 100, 3 },  // 5 - lighter gray
                { 255, 255, 255, 1 }   // 6 - white
            };

            // Validate the input
            int colorIndex = Math.Max(0, Criticality);
            colorIndex = Math.Min(colors.GetLength(0), Criticality);
            double intensity = (double)colors[colorIndex, 3] / 10.0;

            string output = String.Format("rgba({0},{1},{2},{3:0.0})", colors[colorIndex, 0], colors[colorIndex, 1], colors[colorIndex, 2], intensity);
            return output;
        }
    }
}
