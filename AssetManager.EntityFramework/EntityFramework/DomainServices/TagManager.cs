using Abp.Domain.Services;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace AssetManager.DomainServices
{
    public class TagManager : DomainService, ITagManager
    {
        private readonly TagRepository _tagRepository;
        private readonly TagDataRawRepository _tagDataRawRepository;

        // Validation information
        private static int minPrecision = -10;
        private static int maxPrecision = 10;

        public TagManager( TagRepository tagRepository, TagDataRawRepository tagDataRawRepository )
        {
            _tagRepository = tagRepository;
            _tagDataRawRepository = tagDataRawRepository;
        }

        public Tag FirstOrDefaultTag(long id)
        {
            return _tagRepository.FirstOrDefault(id);
        }

        public Tag FirstOrDefaultTag(string name)
        {
            return _tagRepository.FirstOrDefault(p => p.Name == name);
        }

        public Tag FirstOrDefaultTag(long? id, string name)
        {
            Tag tag = null;
            if ( id.HasValue )
                tag = _tagRepository.FirstOrDefault(id.Value);
            else if ( !string.IsNullOrEmpty(name) )
                tag = _tagRepository.FirstOrDefault(p => p.Name == name);
            return tag;
        }

        public Tag FirstOrDefaultTag(Expression<Func<Tag, bool>> predicate)
        {
            return _tagRepository.FirstOrDefault(predicate);
        }

        public List<Tag> GetAllListTag()
        {
            return _tagRepository.GetAll().OrderBy(t => t.Name).ToList();
        }

        public List<Tag> GetAllListTag(Expression<Func<Tag, bool>> predicate)
        {
            return _tagRepository.GetAll().Where(predicate).OrderBy(t => t.Name).ToList();
        }

        public bool DeleteTag(long id)
        {
            // Should check first to see if the tag is in use, and prevent deleting tags in use
            _tagRepository.Delete(id);
            return true;
        }

        public bool DeleteTag(string name)
        {
            // Should check first to see if the tag is in use, and prevent deleting tags in use
            _tagRepository.Delete(p => p.Name == name);
            return true;
        }

        public bool DeleteTag(long? id, string name)
        {
            // Should check first to see if the tag is in use, and prevent deleting tags in use
            if( id.HasValue )
                _tagRepository.Delete(id.Value);

            else if( !string.IsNullOrEmpty(name) )
                _tagRepository.Delete(p => p.Name == name);

            return true;
        }

        public bool DeleteTag(Expression<Func<Tag, bool>> predicate)
        {
            // Should check first to see if the tag is in use, and prevent deleting tags in use
            _tagRepository.Delete(predicate);
            return true;
        }

        public Tag InsertOrUpdateTag(Tag input)
        {
            Tag tag = null;

            // Check to see if a tag already exists. If so, fetch and update it. Otherwise create a new tag.
            if (input.Id > 0)
                tag = _tagRepository.FirstOrDefault(input.Id);
            else if (!string.IsNullOrEmpty(input.Name))
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            // No tag? Then create one
            if (tag == null)
            {
                tag = new Tag { Name = input.Name };
            }
            tag.Description = input.Description;
            tag.Precision = input.Precision;
            tag.UOM = input.UOM;
            tag.Type = input.Type.HasValue ? input.Type.Value : TagType.Continuous;
            tag.TenantId = input.TenantId;

            // Make sure precision is in the proper range
            if(tag.Precision.HasValue)
                tag.Precision = tag.Precision.Value.Clamp(minPrecision, maxPrecision);

            return _tagRepository.InsertOrUpdate(tag);
        }

        public long InsertOrUpdateAndGetIdTag(Tag input)
        {
            Tag tag = null;

            // Check to see if a tag already exists. If so, fetch and update it. Otherwise create a new tag.
            if (input.Id > 0)
                tag = _tagRepository.FirstOrDefault(input.Id);
            else if (!string.IsNullOrEmpty(input.Name))
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            // No tag? Then create one
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = input.Name
                };
            }
            tag.Description = input.Description;
            tag.Precision = input.Precision;
            tag.UOM = input.UOM;
            tag.Type = input.Type.HasValue ? input.Type.Value : TagType.Continuous;
            tag.TenantId = input.TenantId;

            // Make sure precision is in the proper range
            if (tag.Precision.HasValue)
                tag.Precision = tag.Precision.Value.Clamp(minPrecision, maxPrecision);

            return _tagRepository.InsertOrUpdateAndGetId(tag);
        }

        public List<TagDataRaw> GetAllListData(long tagId)
        {
            return _tagDataRawRepository.GetAll().Where(p => p.TagId == tagId).OrderBy(p => p.Timestamp).ToList();
        }

        public List<TagDataRaw> GetAllListData(string tagName)
        {
            return _tagDataRawRepository.GetAll().Where(p => p.Tag.Name == tagName).OrderBy(p => p.Timestamp).ToList();
        }

        public List<TagDataRaw> GetAllListData(Expression<Func<TagDataRaw, bool>> predicate)
        {
            return _tagDataRawRepository.GetAll().Where(predicate).OrderBy(p => p.TagId).ThenBy(p => p.Timestamp).ToList();
        }

        public List<TagDataRaw> GetAllListData(long tagId, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<TagDataRaw> output = null;

            if (startTimestamp.HasValue && endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.TagId == tagId && t.Timestamp >= startTimestamp.Value && t.Timestamp <= endTimestamp).OrderBy(t => t.Timestamp).ToList();
            else if (startTimestamp.HasValue && !endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.TagId == tagId && t.Timestamp >= startTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
            else if (!startTimestamp.HasValue && endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.TagId == tagId && t.Timestamp <= startTimestamp).OrderBy(t => t.Timestamp).ToList();
            else
                output = _tagDataRawRepository.GetAll().Where(t => t.TagId == tagId).OrderBy(t => t.Timestamp).ToList();

            return output;
        }

        public List<TagDataRaw> GetAllListData(string tagName, DateTime? startTimestamp, DateTime? endTimestamp)
        {
            List<TagDataRaw> output = null;

            if (startTimestamp.HasValue && endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.Tag.Name == tagName && t.Timestamp >= startTimestamp.Value && t.Timestamp <= endTimestamp).OrderBy(t => t.Timestamp).ToList();
            else if (startTimestamp.HasValue && !endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.Tag.Name == tagName && t.Timestamp >= startTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
            else if (!startTimestamp.HasValue && endTimestamp.HasValue)
                output = _tagDataRawRepository.GetAll().Where(t => t.Tag.Name == tagName && t.Timestamp <= startTimestamp).OrderBy(t => t.Timestamp).ToList();
            else
                output = _tagDataRawRepository.GetAll().Where(t => t.Tag.Name == tagName).OrderBy(t => t.Timestamp).ToList();

            return output;
        }

        public void DeleteData(Expression<Func<TagDataRaw, bool>> predicate)
        {
            _tagDataRawRepository.Delete(predicate);
            return;
        }

        public TagDataRaw InsertOrUpdateData(TagDataRaw input)
        {
            TagDataRaw data = null;

            // Look to see if there is already a data record matching the tag and time
            data = _tagDataRawRepository.FirstOrDefault(p => p.TagId == input.TagId && p.Timestamp == input.Timestamp);

            if (data == null)
            {
                data = new TagDataRaw
                {
                    TenantId = input.Tag.TenantId,
                    TagId = input.TagId,
                    Timestamp = input.Timestamp,
                    Value = input.Value,
                    Quality = input.Quality
                };
            }
            else
            {
                data.Value = input.Value;
                data.Quality = input.Quality;
            }

            data = _tagDataRawRepository.InsertOrUpdate(data);
            return data;
        }

        public TagDataRaw InsertOrUpdateDataByName(TagDataName input)
        {
            // Transform the "TagDataName" input into "TagDataRaw" format, then call InsertOrUpdateData() to do the work.
            // The most important point is to get/validate the tag information.
            TagDataRaw data = null;
            Tag tag = null;

            if (input.TagId.HasValue)
                tag = _tagRepository.FirstOrDefault(input.TagId.Value);
            else if( !string.IsNullOrEmpty(input.TagName) )
                tag = _tagRepository.FirstOrDefault(t => t.Name == input.TagName);

            if (tag != null)
            {
                data = new TagDataRaw
                {
                    TenantId = tag.TenantId,
                    TagId = tag.Id,
                    Timestamp = input.Timestamp.HasValue ? input.Timestamp.Value : DateTime.Now,
                    Value = input.Value,
                    Quality = input.Quality.HasValue ? input.Quality.Value : TagDataQuality.Good
                };
                data = InsertOrUpdateData(data);
            }
            return data;
        }

        public long InsertOrUpdateAllData(List<TagDataRaw> input)
        {
            long successes = 0;
            List<TagDataRaw> sorted = input.OrderBy(p => p.TagId).ThenBy(p => p.Timestamp).ToList();

            foreach ( TagDataRaw one in sorted )
            {
                if ( InsertOrUpdateData(one) != null )
                    successes++;
            }
            return successes;
        }

        public long InsertOrUpdateAllDataByName(List<TagDataName> input)
        {
            // Transform the "TagDataName" input array into "TagDataRaw" format, then call InsertOrUpdateData() to do the work.
            // The most important point is to get/validate the tag information.
            long successes = 0;
            Tag tag = null;

            List<TagDataName> sorted = input.OrderBy(p => p.TagId).ThenBy(p => p.TagName).ThenBy(p => p.Timestamp).ToList();

            foreach (TagDataName one in sorted)
            {
                // If we don't have a tag OR the one we have doesn't match the data record, get a new tag
                if( tag == null ||
                    (one.TagId.HasValue && one.TagId.Value != tag.Id) ||
                    (!string.IsNullOrEmpty(one.TagName) && one.TagName != tag.Name))
                {
                    if (one.TagId.HasValue)
                        tag = _tagRepository.FirstOrDefault(one.TagId.Value);
                    else
                        tag = _tagRepository.FirstOrDefault(p => p.Name == one.TagName);
                }

                if (tag != null)
                {
                    TagDataRaw data = new TagDataRaw
                    {
                        TenantId = tag.TenantId,
                        TagId = tag.Id,
                        Timestamp = one.Timestamp.HasValue ? one.Timestamp.Value : DateTime.Now,
                        Value = one.Value,
                        Quality = one.Quality.HasValue ? one.Quality.Value : TagDataQuality.Good
                    };

                    if (InsertOrUpdateData(data) != null)
                        successes++;
                }
            }
            return successes;
        }
    }
}
