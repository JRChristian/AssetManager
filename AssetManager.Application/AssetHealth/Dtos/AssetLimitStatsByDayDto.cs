using Abp.Application.Services.Dto;
using AssetManager.IOWs.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class AssetLimitStatsByDayDto
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<LimitStatsByDayDto> Limits { get; set; }
    }
}
