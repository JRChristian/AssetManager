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
        Tag FirstOrDefaultTag(long? id, string name);
        Tag FirstOrDefaultTag(Expression<Func<Tag, bool>> predicate);
        List<Tag> GetAllListTag();
        List<Tag> GetAllListTag(Expression<Func<Tag, bool>> predicate);
        bool DeleteTag(long id);
        bool DeleteTag(string name);
        bool DeleteTag(long? id, string name);
        bool DeleteTag(Expression<Func<Tag, bool>> predicate);
        Tag InsertOrUpdateTag(Tag input);
        long InsertOrUpdateAndGetIdTag(Tag tag);

        DateTime GetMinimumTagDataTimestamp();
        DateTime GetMaximumTagDataTimestamp();
        List<TagDataRaw> GetAllListData(long tagId);
        List<TagDataRaw> GetAllListData(string tagName);
        List<TagDataRaw> GetAllListData(Expression<Func<TagDataRaw, bool>> predicate);
        List<TagDataRaw> GetAllListData(long tagId, DateTime? startTimestamp, DateTime? endTimestamp);
        List<TagDataRaw> GetAllListData(string tagName, DateTime? startTimestamp, DateTime? endTimestamp);
        TagDataRaw GetTagDataAtTime(long tagId, DateTime timestamp);
        List<TagDataRaw> GetTagDataForTimeRange(long tagId, DateTime startTimestamp, DateTime endTimestamp);
        List<TagDataRaw> GetTagDataAtTime(List<long> tagIds, DateTime timestamp);
        List<TagDataRaw> GetTagDataForTimeRange(List<long> tagIds, DateTime startTimestamp, DateTime endTimestamp);
        void DeleteData(Expression<Func<TagDataRaw, bool>> predicate);
        TagDataRaw InsertOrUpdateDataByName(TagDataName input);
        long InsertOrUpdateAllDataByName(List<TagDataName> input);

        int UpdateTagDataForDemo(int daysToAdd);
    }
}
