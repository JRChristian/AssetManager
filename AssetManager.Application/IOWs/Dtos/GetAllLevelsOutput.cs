using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetAllLevelsOutput : IOutputDto
    {
        public List<LevelDto> IOWLevels { get; set; }
    }
}
