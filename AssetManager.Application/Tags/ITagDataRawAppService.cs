using Abp.Application.Services;
using AssetManager.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public interface ITagDataRawAppService : IApplicationService
    {
        GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input);
        //Task<GetTagDataRawOutput> GetTagDataRawListAsync(GetTagDataRawInput input);
        TagDataRawDto AddTagDataRaw(AddTagDataRawInput input);
    }
}
