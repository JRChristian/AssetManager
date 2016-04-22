using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetIowLimitOutput : IOutputDto
    {
        public List<IowLimitDto> Limits { get; set; }
    }
}
