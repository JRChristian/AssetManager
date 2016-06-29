using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class UpdateLevelInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, 9, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }
        public MetricType MetricType { get; set; }
        public Direction GoodDirection { get; set; }
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double WarningLevel { get; set; }
        [Range(0,100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double ErrorLevel { get; set; }
    }
}
