using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class GetTagDataCanvasJSOutput : IOutputDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int? Precision { get; set; }
        public CanvasJSDto CanvasJS { get; set; }
    }
}
