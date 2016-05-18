using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    class DeviationSummaryDto : EntityDto<long>
    {
        public long VariableId { get; set; }
        public string VariableName { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public long LimitId { get; set; }
        public long DeviationCount30Day { get; set; }
        public long DeviationHours30Day { get; set; }
    }
}
