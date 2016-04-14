using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    public class CreateIOWLevelInput : IInputDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1,5)]
        public int Criticality { get; set; }
        [Required]
        public string ResponseGoal { get; set; }
        [Required]
        public string MetricGoal { get; set; }
    }
}
