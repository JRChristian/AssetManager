using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetLimitStatsByDayInput : IInputDto
    {
        public long? VariableId { get; set; }
        public string VariableName { get; set; }
        public long? LimitId { get; set; }
        public DateTime? startTimestamp { get; set; }
        public DateTime? endTimestamp { get; set; }
    }
}
