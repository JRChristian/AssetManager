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
        GetTagOutput GetTagList(GetTagInput input);
        Task<GetTagOutput> GetTagListAsync(GetTagInput input);
        void CreateTag(CreateTagInput input);
        void DeleteTag(DeleteTagInput input);
        void UpdateTag(UpdateTagInput input);
    }
}
