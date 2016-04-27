using Abp.Application.Services;
using Abp.AutoMapper;
using AssetManager.Entities;
using AssetManager.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public class TagDataAppService : AssetManagerAppServiceBase, ITagDataAppService
    {
        //These members set in constructor using constructor injection.
        private readonly ITagRepository _tagRepository;
        private readonly ITagDataRawRepository _tagDataRawRepository;

        public TagDataAppService(ITagRepository tagRepository, ITagDataRawRepository tagDataRawRepository)
        {
            _tagRepository = tagRepository;
            _tagDataRawRepository = tagDataRawRepository;
        }

        public GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input)
        {
            Tag tag = null;
            List<TagDataRaw> data = null;
            GetTagDataRawOutput output = new GetTagDataRawOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
                Precision = null,
                TagDataRaw = new List<TagDataRawDto>()
            };

            //Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            //If the tag exists, save the description and get values in the relevant time range
            if (tag != null)
            {
                output.Name = tag.Name;
                output.Description = tag.Description;
                output.UOM = tag.UOM;
                output.Precision = tag.Precision;

                // Get data in the desired range
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderByDescending(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp >= input.StartTimestamp.Value).OrderByDescending(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id && t.Timestamp <= input.EndTimestamp).OrderByDescending(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.TagId == tag.Id).OrderByDescending(t => t.Timestamp).ToList();

                output.TagDataRaw = data.MapTo<List<TagDataRawDto>>();
            }

            return output;
        }

        /*public Task<GetTagDataRawOutput> GetTagDataRawListAsync(GetTagDataRawInput input)
        {

        }*/

        public TagDataRawDto AddTagDataRaw(AddTagDataRawInput input)
        {
            Tag tag = null;
            TagDataRawDto output = new TagDataRawDto
            {
                Id = 0,
                Timestamp = DateTime.Now,
                Value = 0,
                Quality = TagDataQuality.Bad
            };

            // Get the tag information and confirm that the tag exists
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            // If the tag exists, add or update the value
            if (tag != null)
            {
                TagDataRaw data = null;
                DateTime timestamp = input.Timestamp.HasValue ? input.Timestamp.Value : DateTime.Now;

                // Find out if there is already a tag with this value
                data = _tagDataRawRepository.FirstOrDefault(p => p.TagId == tag.Id && p.Timestamp == timestamp);

                if( data == null)
                {
                    // Did not found a match, so add all entries
                    data = new TagDataRaw
                        {
                            TenantId = tag.TenantId,
                            TagId = tag.Id,
                            Timestamp = timestamp,
                            Value = input.Value,
                            Quality = input.Quality.HasValue ? input.Quality.Value : TagDataQuality.Good
                        };
                }
                else
                {
                    // Found the record, so update only what has changed
                    data.Value = input.Value;
                    if( input.Quality.HasValue )
                        data.Quality = input.Quality.Value;
                }

                data.Id = _tagDataRawRepository.InsertOrUpdateAndGetId(data);

                output = data.MapTo<TagDataRawDto>();
            }

            return output;
        }
    }
}
