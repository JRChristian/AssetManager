using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetVariableOutput : IOutputDto
    {
        public VariableDto variable { get; set; }
        public long LimitCount { get; set; }
    }
}
