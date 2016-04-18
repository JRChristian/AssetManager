﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class GetOneAssetInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}