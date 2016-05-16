using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetVariableLimitStatusInput : IInputDto
    {
        public string VariableName { get; set; }
        public string LevelName { get; set; }
    }
}
