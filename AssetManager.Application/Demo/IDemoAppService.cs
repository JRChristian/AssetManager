using Abp.Application.Services;
using AssetManager.Demo.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Demo
{
    public interface IDemoAppService : IApplicationService
    {
        GetDemoStatusOutput GetDemoStatus(GetDemoStatusInput input);
        UpdateTagDataForDemoOutput UpdateTagDataForDemo(UpdateTagDataForDemoInput input);
    }
}
