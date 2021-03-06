﻿using Abp.Application.Services.Dto;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetIowChartCanvasJSOutput : IOutputDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
        public CanvasJS CanvasJS { get; set; }
    }
}
