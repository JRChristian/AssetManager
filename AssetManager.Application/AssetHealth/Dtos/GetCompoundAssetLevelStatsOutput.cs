using Abp.Application.Services.Dto;
using AssetManager.EntityFramework.DomainServices;
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
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public long AssetParentId { get; set; }
        public string AssetParentName { get; set; }
        public int NumberAssets { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public double DurationHours { get; set; }
        public List<LevelStats> OverallStats { get; set; }
        public List<AssetLevelStats> AssetStats { get; set; }
    }

    public class LevelInfo
    {
        public int Criticality { get; set; }
        public string LevelName { get; set; }
    }
}
