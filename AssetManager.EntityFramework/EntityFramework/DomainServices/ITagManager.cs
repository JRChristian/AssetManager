using Abp.Domain.Services;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public interface ITagManager : IDomainService
    {
        Tag FirstOrDefaultTag(long id);
        Tag FirstOrDefaultTag(string name);
        Tag FirstOrDefaultTag(Expression<Func<Tag, bool>> predicate);
        List<Tag> GetAllListTag();
        List<Tag> GetAllListTag(Expression<Func<Tag, bool>> predicate);
        bool DeleteTag(long id);
        bool DeleteTag(string name);
        bool DeleteTag(Expression<Func<Tag, bool>> predicate);
        Tag InsertOrUpdateTag(Tag tag);
        long InsertOrUpdateAndGetIdTag(Tag tag);

        List<TagDataRaw> GetAllListData(long tagId);
        List<TagDataRaw> GetAllListData(string tagName);
        List<TagDataRaw> GetAllListData(Expression<Func<TagDataRaw, bool>> predicate);
        List<TagDataRaw> GetAllListData(long tagId, DateTime? startTimestamp, DateTime? endTimestamp);
        List<TagDataRaw> GetAllListData(string tagName, DateTime? startTimestamp, DateTime? endTimestamp);
        void DeleteData(Expression<Func<TagDataRaw, bool>> predicate);
        TagDataRaw InsertOrUpdateData(TagDataRaw input);
        TagDataRaw InsertOrUpdateDataByName(TagDataName input);
        long InsertOrUpdateAllData(List<TagDataRaw> input);
        long InsertOrUpdateAllDataByName(List<TagDataName> input);
    }
}
