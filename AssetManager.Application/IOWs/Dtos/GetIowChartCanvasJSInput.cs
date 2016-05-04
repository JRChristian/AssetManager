﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetIowChartCanvasJSInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
    }
}