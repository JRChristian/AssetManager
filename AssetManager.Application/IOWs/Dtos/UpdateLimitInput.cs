using Abp.Application.Services.Dto;
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

        public long? IowVariableId { get; set; }
        public string IowVariableName { get; set; }
        public bool? IsActive { get; set; }

        public long? IOWLevelId { get; set; }
        public string Name { get; set; }
        public int? Criticality { get; set; }

        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public double? LowLimit { get; set; }
        public double? HighLimit { get; set; }
    }
}
