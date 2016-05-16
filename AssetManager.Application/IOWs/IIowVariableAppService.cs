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
        GetVariableOutput GetVariable(GetVariableInput input);
        GetAllVariablesOutput GetAllVariables(GetAllVariablesInput input);
        GetVariableLimitsOutput GetVariableLimits(GetVariableLimitsInput input);
        DeleteVariableOutput DeleteVariable(GetVariableInput input);
        VariableDto UpdateVariable(UpdateVariableInput input);

        GetAllLimitsOutput GetAllLimits(GetAllLimitsInput input);
        LimitDto UpdateLimit(UpdateLimitInput input);

        GetIowChartCanvasJSOutput GetIowChartCanvasJS(GetIowChartCanvasJSInput input);
        GetVariableLimitStatusOutput GetVariableLimitStatus(GetVariableLimitStatusInput input);
    }
}
