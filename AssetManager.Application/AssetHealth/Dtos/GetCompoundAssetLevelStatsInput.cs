﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetCompoundAssetLevelStatsInput : IInputDto
    {
        public long? AssetId { get; set; }
        public string AssetName { get; set; }
        public int IncludeAssetChildren { get; set; }
        public int IncludeAssetTypesAsChildren { get; set; }
        public long? AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime? StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
    }
}
