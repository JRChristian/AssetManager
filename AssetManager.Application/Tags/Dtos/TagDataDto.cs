using Abp.Application.Services.Dto;
using AssetManager.Entities;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class TagDataDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int? Precision { get; set; }
        public TagType? Type { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public TagDataQuality Quality { get; set; }
    }
}
