using Abp.Application.Services;
using AssetManager.Images.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Images
{
    public interface IImageAppService : IApplicationService
    {
        GetImageOutput GetImage(GetImageInput input);
        GetImageListOutput GetImageList(GetImageListInput input);
        UpdateImageListOutput UpdateImageList(UpdateImageListInput input);
    }
}
