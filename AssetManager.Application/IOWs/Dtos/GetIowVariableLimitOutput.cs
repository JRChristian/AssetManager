using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetIowVariableLimitOutput : IOutputDto
    {
        public List<IowVariableLimitDto> IowVariables { get; set; }
    }
}
