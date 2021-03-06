﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class CreateAssetInput : IInputDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public long? AssetTypeId { get; set; }

        public string AssetTypeName { get; set; }
        public string Materials { get; set; }
    }
}
