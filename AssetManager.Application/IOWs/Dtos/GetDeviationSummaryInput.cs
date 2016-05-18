using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetDeviationSummaryInput : IInputDto
    {
        public bool? includeAllVariables { get; set; }
        public int? maxCriticality { get; set; }
        public double? hoursBack { get; set; }
    }
}
