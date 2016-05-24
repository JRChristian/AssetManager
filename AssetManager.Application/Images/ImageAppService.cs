using Abp.AutoMapper;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.Images.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Images
{
    public class ImageAppService : AssetManagerAppServiceBase, IImageAppService
    {
        private readonly IImageManager _imageManager;

        public ImageAppService( IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        public GetImageOutput GetImage(GetImageInput input)
        {
            Image image = _imageManager.FirstOrDefaultImage(input.Id);
            return new GetImageOutput { Image = image.MapTo<ImageDto>() };
        }

        public GetImageListOutput GetImageList(GetImageListInput input)
        {
            List<Image> images = _imageManager.GetAllListImage();
            return new GetImageListOutput { Images = images.MapTo<List<ImageDto>>() };
        }

        public UpdateImageListOutput UpdateImageList(UpdateImageListInput input)
        {
            UpdateImageListOutput output = new UpdateImageListOutput
            {
                UpdateSucceeded = new List<string>(),
                UpdateFailed = new List<string>()
            };

            if( input.Images != null )
            {
                foreach (ImageDto image in input.Images)
                {
                    _imageManager.UpdateImage(image.Id, image.Name, image.Description, image.Url);
                    output.UpdateSucceeded.Add(image.Name);
                }
            }

            return output;
        }
    }
}
