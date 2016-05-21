using Abp.Application.Services.Dto;
using AssetManager.Entities;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class VariableLimitStatusDto : EntityDto<long>
    {
        // Variable and tag
        public long VariableId { get; set; }
        public string VariableName { get; set; }
        public string VariableDescription { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public DateTime? LastTimestamp { get; set; }
        public double? LastValue { get; set; }
        public TagDataQuality? LastQuality { get; set; }

        // Level
        public long IOWLevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }

        // Limit
        public string LimitName { get; set; } // Formated as: Criticality-LevelName-Direction
        public Direction Direction { get; set; }
        public double LimitValue { get; set; }
        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        // Latest deviation
        public IOWStatus LastStatus { get; set; }
        public DateTime? LastDeviationStartTimestamp { get; set; }
        public DateTime? LastDeviationEndTimestamp { get; set; }

        // Help the UI
        // SeverityMessage: "Active", "within 24 hours", "none", etc.
        // SeverityLabel: Bootstrap class. See http://www.w3schools.com/bootstrap/bootstrap_badges_labels.asp
        public string SeverityMessage1 { get; set; }
        public string SeverityMessage2 { get; set; }
        public string SeverityClass { get; set; }
    }
}
