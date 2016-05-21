using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class DetectDeviationsOutput : IOutputDto
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public int NumberVariables { get; set; }
        public int NumberLimits { get; set; }
        public int NumberDeviations { get; set; }
    }
}
