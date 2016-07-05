using Abp.Application.Services.Dto;
using AssetManager.EntityFramework.DomainServices;
using AssetManager.IOWs.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetCompoundAssetLevelStatsOutput : IOutputDto
    {
        public long AssetParentId { get; set; }
        public string AssetParentName { get; set; }
        public int NumberAssets { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public double DurationHours { get; set; }
        public AssetLevelStats OverallStats { get; set; }
        public List<AssetLevelStats> ChildStats { get; set; }
        public List<LevelInfo> Levels { get; set; }
        public List<VariableLimitStatusDto> ProblemLimits { get; set; }
    }

    public class LevelInfo
    {
        public int Criticality { get; set; }
        public string LevelName { get; set; }
    }
}
