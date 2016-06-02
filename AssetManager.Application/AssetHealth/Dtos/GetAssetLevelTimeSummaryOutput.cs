using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLevelTimeSummaryOutput : IOutputDto
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public int NumberPeriods { get; set; }
        public int HoursInPeriod { get; set; }
        public List<AssetLevelTimeDto> AssetDeviations { get; set; }
    }
}
