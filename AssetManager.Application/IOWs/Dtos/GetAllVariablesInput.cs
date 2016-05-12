using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetAllVariablesInput : IInputDto
    {
        public string Name { get; set; }
        public string TagName { get; set; }
    }
}
