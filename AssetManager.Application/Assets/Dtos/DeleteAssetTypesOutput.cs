using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets.Dtos
{
    public class DeleteAssetTypesOutput : IOutputDto
    {
        public int SuccessfulDeletions { get; set; }
        public int FailedDeletions { get; set; }
    }
}
