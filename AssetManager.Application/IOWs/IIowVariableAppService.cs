using Abp.Application.Services;
using AssetManager.IOWs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public interface IIowVariableAppService : IApplicationService
    {
        IowVariableDto GetOneIowVariable(GetOneIowVariableInput input);
        GetIowVariableOutput GetIowVariables(GetIowVariableInput input);
        Task<GetIowVariableOutput> GetIowVariablesAsync(GetIowVariableInput input);
        IowVariableDto CreateIowVariable(CreateIowVariableInput input);
        IowVariableDto DeleteIowVariable(GetOneIowVariableInput input);
        IowVariableDto UpdateIowVariable(UpdateIowVariableInput input);
    }
}
