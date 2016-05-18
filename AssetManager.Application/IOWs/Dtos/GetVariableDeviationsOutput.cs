using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetVariableDeviationsOutput : IOutputDto
    {
        public long VariableId { get; set; }
        public string VariableName { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public long LimitId { get; set; }

        public List<DeviationsDto> Deviations { get; set; }
    }
}
