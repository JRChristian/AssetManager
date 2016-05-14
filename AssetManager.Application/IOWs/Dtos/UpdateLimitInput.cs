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
    public class UpdateLimitInput : IInputDto
    {
        public long? Id { get; set; }

        public long? IOWVariableId { get; set; }
        public string VariableName { get; set; }
        public bool? IsActive { get; set; }

        public long? IOWLevelId { get; set; }
        public string LevelName { get; set; }
        public int? Criticality { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Direction Direction { get; set; }
        public double Value { get; set; }
        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }
    }
}
