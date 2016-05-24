using Abp.Domain.Services;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public interface IImageManager : IDomainService
    {
        Image FirstOrDefaultImage(long id);
        List<Image> GetAllListImage();
        List<Image> GetAllListImage(Expression<Func<Image, bool>> predicate);
        bool UpdateImage(long? id, string name, string description, string url);
    }
}
