using Abp.Application.Services.Dto;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetVariableLimitsOutput : IOutputDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public DateTime? LastTimestamp { get; set; }
        public double? LastValue { get; set; }
        public TagDataQuality? LastQuality { get; set; }
        public List<LimitDto> Limits { get; set; }
    }
}
