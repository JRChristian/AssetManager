using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class GetAssetLimitCurrentStatusInput : IInputDto
    {
        public long? AssetId { get; set; }
        public string AssetName { get; set; }
    }
}
