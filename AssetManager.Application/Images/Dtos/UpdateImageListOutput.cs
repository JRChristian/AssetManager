using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Images.Dtos
{
    public class UpdateImageListOutput : IOutputDto
    {
        public List<string> UpdateSucceeded { get; set; }
        public List<string> UpdateFailed { get; set; }
    }
}
