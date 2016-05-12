using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class DeleteVariableOutput : IOutputDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
    }
}
