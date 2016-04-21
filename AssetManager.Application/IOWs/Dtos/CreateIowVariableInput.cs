using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class CreateIowVariableInput : IInputDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public long? TagId { get; set; }
        public string TagName { get; set; }
        public string UOM { get; set; }
    }
}
