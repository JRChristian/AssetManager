﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth.Dtos
{
    public class DeleteHealthMetricInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
