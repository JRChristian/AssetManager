using Abp.Application.Services;
using Abp.AutoMapper;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
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
        private readonly ITagManager _tagManager;

        public TagAppService(ITagRepository tagRepository, ITagManager tagManager)
        {
            _tagRepository = tagRepository;
            _tagManager = tagManager;
        }

        public TagDto GetOneTag(GetOneTagInput input)
        {
            TagDto output = new TagDto { Name = input.Name, Description = "", UOM = "" };
            Tag tag = null;

            if (input.Id.HasValue)
                tag = _tagManager.FirstOrDefaultTag(input.Id.Value);
            else
                tag = _tagManager.FirstOrDefaultTag(input.Name);

            output = tag.MapTo<TagDto>();
            return output;
        }

        public GetTagOutput GetTagList(GetTagInput input)
        {
            List<Tag> tags = null;

            // If the input includes a tag name, treat it as a wild card match. Otherwise, get all tags.
            if (!string.IsNullOrEmpty(input.Name))
                tags = _tagManager.GetAllListTag(p => p.Name.Contains(input.Name));
            else
                tags = _tagManager.GetAllListTag();

            return new GetTagOutput
            {
                Tags = tags.MapTo<List<TagDto>>()
            };
        }

        public TagDto CreateTag(CreateTagInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a tag for input: " + input.Name);

            // Create a new tag object from the input
            var New = new Tag
            {
                Name = input.Name,
                Description = input.Description,
                UOM = input.UOM,
                Precision = input.Precision,
                Type = input.Type.HasValue ? input.Type.Value : TagType.Continuous,
                TenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1
            };

            // Create (or update) the new tag
            _tagManager.InsertOrUpdateTag(New);

            // Map the new tag to the return format, and return the new information
            TagDto output = New.MapTo<TagDto>();

            return output;
        }

        public bool DeleteTag(DeleteTagInput input)
        {
            bool success = false;

            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Deleting a tag for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            //Get the Tag entity using either the Id or the name--whatever is passed through
            if (input.Id.HasValue)
                success = _tagManager.DeleteTag(input.Id.Value);
            else if ( !string.IsNullOrEmpty(input.Name) )
                success = _tagManager.DeleteTag(input.Name);

            return success;
        }

        public TagDto UpdateTag(UpdateTagInput input)
        {
            Logger.Info("Updating a tag for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            TagDto output = new TagDto { Name = "", Description = "", UOM = "" };

            //Retrieving an Tag entity with given id (if specified) or name (if id is not specified).
            //FirstOrDefault() returns null if nothing is found.
            Tag tag = null;
            if (input.Id.HasValue)
                tag = _tagManager.FirstOrDefaultTag(input.Id.Value);
            else if( !string.IsNullOrEmpty(input.Name) )
                tag = _tagManager.FirstOrDefaultTag(input.Name);

            if (tag != null)
            {
                if (!string.IsNullOrEmpty(input.Name))
                    tag.Name = input.Name;

                if (!string.IsNullOrEmpty(input.Description))
                    tag.Description = input.Description;

                if (!string.IsNullOrEmpty(input.Name))
                    tag.UOM = input.UOM;

                if (input.Precision.HasValue)
                    tag.Precision = input.Precision.Value;

                if (input.Type.HasValue)
                    tag.Type = input.Type.Value;
            }
            else if ( !string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.Description) )
            {
                tag = new Tag
                {
                    Name = input.Name,
                    Description = input.Description,
                    UOM = input.UOM,
                    Precision = input.Precision,
                    Type = input.Type.HasValue ? input.Type.Value : TagType.Continuous,
                    TenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1
                };
            }

            if( tag != null)
            {
                tag = _tagManager.InsertOrUpdateTag(tag);
                output = tag.MapTo<TagDto>();
            }
            return output;
        }
    }
}
