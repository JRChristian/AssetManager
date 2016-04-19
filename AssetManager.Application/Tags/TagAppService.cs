using Abp.Application.Services;
using Abp.AutoMapper;
using AssetManager.Entities;
using AssetManager.Tags.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public class TagAppService : AssetManagerAppServiceBase, ITagAppService
    {
        //These members set in constructor using constructor injection.
        private readonly ITagRepository _tagRepository;

        public TagAppService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public TagDto GetOneTag(GetOneTagInput input)
        {
            TagDto output = new TagDto { Name = input.Name, Description = "", UOM = "" };
            Tag tag;

            // If the input includes a tag name, treat it as a wild card match. Otherwise, get all tags.
            if (input.Id.HasValue)
                tag = _tagRepository.FirstOrDefault(t => t.Id == input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(t => t.Name == input.Name);

            output = tag.MapTo<TagDto>();
            return output;
        }

        public GetTagOutput GetTagList(GetTagInput input)
        {
            List<Tag> tags;

            // If the input includes a tag name, treat it as a wild card match. Otherwise, get all tags.
            if(input.Name != "")
                tags = _tagRepository.GetAll().Where(p => p.Name.Contains(input.Name)).OrderBy(p => p.Name).ToList();
            else
                tags = _tagRepository.GetAll().OrderBy(p => p.Name).ToList();

            return new GetTagOutput
            {
                Tags = tags.MapTo<List<TagDto>>()
            };
        }

        public async Task<GetTagOutput> GetTagListAsync(GetTagInput input)
        {
            List<Tag> tags;

            // If the input includes a tag name, treat it as a wild card match. Otherwise, get all tags.
            if (input.Name != "")
                tags = await _tagRepository.GetAllListAsync(p => p.Name.Contains(input.Name));
            else
                tags = await _tagRepository.GetAllListAsync();

            var sorted = tags.OrderBy(p => p.Name);
            return new GetTagOutput
            {
                Tags = sorted.MapTo<List<TagDto>>()
            };
        }

        public TagDto CreateTag(CreateTagInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a tag for input: " + input.Name);

            TagDto output = new TagDto { Name = input.Name, Description = "", UOM = "" };

            //Check to see if this name already exists
            var tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);
            if (tag == null)
            {
                // All tags belong to a tenant. If not specified, put them in the default tenant.
                int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

                // Create a new tag object from the input
                var New = new Tag
                {
                    Name = input.Name,
                    Description = input.Description,
                    UOM = input.UOM,
                    TenantId = tenantid
                };

                // Add the new tag to the repository
                _tagRepository.Insert(New);

                // Map the new tag to the return format, and return the new information
                output = New.MapTo<TagDto>();
            }
            return output;
        }

        public void DeleteTag(DeleteTagInput input)
        {
            Tag tag;

            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Deleting a tag for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            //Get the Tag entity using either the Id or the name--whatever is passed through
            if (input.Id.HasValue)
                tag = _tagRepository.Get(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            if (tag != null)
                _tagRepository.Delete(tag);
        }

        public TagDto UpdateTag(UpdateTagInput input)
        {
            Logger.Info("Updating a tag for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            TagDto output = new TagDto { Name = "", Description = "", UOM = "" };

            //Retrieving an Tag entity with given id (if specified) or name (if id is not specified).
            //FirstOrDefault() returns null if nothing is found.
            Tag tag;
            if (input.Id.HasValue)
                tag = _tagRepository.FirstOrDefault(input.Id.Value);
            else
                tag = _tagRepository.FirstOrDefault(p => p.Name == input.Name);

            if (tag != null)
            {
                if (input.Name != "")
                    tag.Name = input.Name;

                if (input.Description != "")
                    tag.Description = input.Description;

                if (input.UOM != "")
                    tag.UOM = input.UOM;

                // Map the new tag to the return format, and return the new information
                output = tag.MapTo<TagDto>();
            }

            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).

            return output;
        }
    }
}
