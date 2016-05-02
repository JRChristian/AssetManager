﻿using Abp.Application.Services.Dto;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class TagDataBulkDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public TagDataQuality? Quality { get; set; }
    }
}