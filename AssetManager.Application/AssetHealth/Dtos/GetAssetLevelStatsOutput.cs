using Abp.Application.Services.Dto;
using AssetManager.EntityFramework.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLevelStatsOutput : IOutputDto
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public double DurationHours { get; set; }
        public List<AssetLevelStats> AssetLevelStats { get; set; }
    }
}
