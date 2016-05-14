using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.IOWs.Dtos;
using AssetManager.Tags;
using AssetManager.Tags.Dtos;
using AssetManager.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public class IowDeviationAppService : AssetManagerAppServiceBase, IIowDeviationAppService
    {
        private readonly IIowManager _iowManager;
        private readonly ITagManager _tagManager;

        public IowDeviationAppService(
            IIowManager iowManager,
            ITagManager tagManager)
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
        }

        public void DetectDeviations(DetectDeviationsInput input)
        {
            // Set defaults for the time range
            DateTime startTimestamp = input.StartTimestamp.HasValue ? input.StartTimestamp.Value : DateTime.Now.AddHours(-24);
            DateTime endTimestamp = input.EndTimestamp.HasValue ? input.EndTimestamp.Value : DateTime.Now.AddHours(1);

            // Validate the tag
            if( input.TagName == "*")
            {
                List<Tag> tags = _tagManager.GetAllListTag();
                foreach(Tag tag in tags)
                    _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
            else
            {
                Tag tag = _tagManager.FirstOrDefaultTag(input.TagId, input.TagName);
                if( tag != null )
                    _iowManager.DetectDeviations(tag, startTimestamp, endTimestamp);
            }
        }
    }
}
