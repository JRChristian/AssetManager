using Abp.Application.Services;
using AssetManager.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tags
{
    public interface ITagDataAppService : IApplicationService
    {
        GetTagDataRawOutput GetTagDataRawList(GetTagDataRawInput input);
        GetTagDataChartOutput GetTagDataChart(GetTagDataChartInput input);
        GetTagDataCanvasJSOutput GetTagDataCanvasJS(GetTagDataCanvasJSInput input);
        //Task<GetTagDataRawOutput> GetTagDataRawListAsync(GetTagDataRawInput input);
        TagDataRawDto AddTagDataRaw(AddTagDataRawInput input);
    }
}
