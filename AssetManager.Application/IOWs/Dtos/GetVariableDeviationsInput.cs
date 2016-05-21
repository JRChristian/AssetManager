using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetVariableDeviationsInput : IInputDto
    {
        public long? Id { get; set; }
        public string VariableName { get; set; }
        public int? hoursBack { get; set; }
    }
}
