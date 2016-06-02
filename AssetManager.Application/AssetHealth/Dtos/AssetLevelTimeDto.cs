using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class AssetLevelTimeDto : EntityDto<long>
    {
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public string LevelName { get; set; }
        public int Criticality { get; set; }
        public List<DeviationDetailDto> DeviationDetails { get; set; }
    }

    public class DeviationDetailDto
    {
        public DateTime Timestamp { get; set; }
        public int DeviationCount { get; set; }
        public double DurationHours { get; set; }
    }
}
