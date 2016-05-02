using Abp.Application.Services.Dto;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class UpdateTagInput : IInputDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        [Range(-10, 10)]
        public int? Precision { get; set; }
        public TagType? Type { get; set; }
    }
}
