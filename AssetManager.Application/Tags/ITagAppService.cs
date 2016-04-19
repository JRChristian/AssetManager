using Abp.Application.Services;
using AssetManager.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public interface ITagAppService : IApplicationService
    {
        TagDto GetOneTag(GetOneTagInput input);
        GetTagOutput GetTagList(GetTagInput input);
        Task<GetTagOutput> GetTagListAsync(GetTagInput input);
        TagDto CreateTag(CreateTagInput input);
        void DeleteTag(DeleteTagInput input);
        TagDto UpdateTag(UpdateTagInput input);
    }
}
