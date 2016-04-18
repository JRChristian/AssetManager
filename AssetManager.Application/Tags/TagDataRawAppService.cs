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
    public class TagDataRawAppService : AssetManagerAppServiceBase, ITagDataRawAppService
    {
        //These members set in constructor using constructor injection.
        private readonly ITagRepository _tagRepository;
        private readonly ITagDataRawRepository _tagDataRawRepository;

        public TagDataRawAppService(ITagRepository tagRepository, ITagDataRawRepository tagDataRawRepository)
        {
            _tagRepository = tagRepository;
            _tagDataRawRepository = tagDataRawRepository;
        }

        public GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input)
        {
            Tag tag;
            List<TagDataRaw> data;
            GetTagDataRawOutput output = new GetTagDataRawOutput
            {
                Name = input.Name,
                Description = "",
                UOM = "",
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

                // Get data in the desired range
                if (input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.Id == tag.Id && t.Timestamp >= input.StartTimestamp.Value && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else if (input.StartTimestamp.HasValue && !input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.Id == tag.Id && t.Timestamp >= input.StartTimestamp.Value).OrderBy(t => t.Timestamp).ToList();
                else if (!input.StartTimestamp.HasValue && input.EndTimestamp.HasValue)
                    data = _tagDataRawRepository.GetAll().Where(t => t.Id == tag.Id && t.Timestamp <= input.EndTimestamp).OrderBy(t => t.Timestamp).ToList();
                else
                    data = _tagDataRawRepository.GetAll().Where(t => t.Id == tag.Id).OrderBy(t => t.Timestamp).ToList();

                output.TagDataRaw = data.MapTo<List<TagDataRawDto>>();
            }

            return output;
        }

        /*public Task<GetTagDataRawOutput> GetTagDataRawListAsync(GetTagDataRawInput input)
        {

        }*/
    }
}
