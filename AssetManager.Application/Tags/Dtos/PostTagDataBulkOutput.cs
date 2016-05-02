using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags.Dtos
{
    public class PostTagDataBulkOutput : IOutputDto
    {
        public int Total { get; set; }
        public int Successes { get; set; }
    }
}
