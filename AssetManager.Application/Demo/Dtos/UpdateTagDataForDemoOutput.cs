using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Demo.Dtos
{
    public class UpdateTagDataForDemoOutput : IOutputDto
    {
        public int NumberUpdates { get; set; }
    }
}
