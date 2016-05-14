using Abp.Application.Services;
using AssetManager.IOWs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public interface IIowDeviationAppService : IApplicationService
    {
        void DetectDeviations(DetectDeviationsInput input);
    }
}
