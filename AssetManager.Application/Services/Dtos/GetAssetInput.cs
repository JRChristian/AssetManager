using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    public class GetAssetInput : IInputDto
    {
        public long? AssetTypeId { get; set; }
    }
}
