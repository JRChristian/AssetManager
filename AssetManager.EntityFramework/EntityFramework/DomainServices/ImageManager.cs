using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class ImageManager : DomainService, IImageManager
    {
        private readonly ImageRepository _imageRepository;

        public ImageManager( ImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public Image FirstOrDefaultImage(long id)
        {
            return _imageRepository.FirstOrDefault(id);
        }

        public List<Image> GetAllListImage()
        {
            return _imageRepository.GetAll().OrderBy(t => t.Name).ToList();
        }

        public List<Image> GetAllListImage(Expression<Func<Image, bool>> predicate)
        {
            return _imageRepository.GetAll().Where(predicate).OrderBy(t => t.Name).ToList();
        }

        public bool UpdateImage(long? id, string name, string description, string url)
        {
            Image image = null;

            // First look to see if this image already exists. If so, this is an update. If not, insert a new record.
            if (id.HasValue && id.Value > 0)
                image = _imageRepository.FirstOrDefault(id.Value);
            else if( !string.IsNullOrEmpty(name) )
                image = _imageRepository.FirstOrDefault(p => p.Name == name);

            if (image == null)
            {
                image = new Image { Name=name, Description=description, Url=url };
                _imageRepository.Insert(image);
            }
            else
            {
                image.Description = description;
                image.Url = url;
                // No need to explicitly call the update() method -- ABP does this automatically
            }
            return true;
        }
    }
}
