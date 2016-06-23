using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetRecentlyDeviatingLimitsInput : IInputDto
    {
        public int MaxCriticality { get; set; }
        public double HoursBack { get; set; }
    }
}
