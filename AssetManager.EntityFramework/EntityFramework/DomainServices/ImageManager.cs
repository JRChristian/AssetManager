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

    }
}
