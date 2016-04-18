using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetIOWLevelOutput : IOutputDto
    {
        public List<IOWLevelDto> IOWLevels { get; set; }
    }
}
