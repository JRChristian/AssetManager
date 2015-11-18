using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    public class GetAssetOutput : IOutputDto
    {
        public List<AssetDto> Assets { get; set; }
    }
}
