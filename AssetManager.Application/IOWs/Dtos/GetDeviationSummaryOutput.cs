using Abp.Application.Services.Dto;
using AssetManager.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetDeviationSummaryOutput : IOutputDto
    {
        public bool includeAllVariables { get; set; }
        public int maxCriticality { get; set; }
        public double hoursBack { get; set; }
        public List<VariableDeviation> deviations { get; set; }
    }
}
