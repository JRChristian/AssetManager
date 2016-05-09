using Abp.Application.Services;
using AssetManager.IOWs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    /// <summary>
    /// Defines an application service for <see cref="Asset"/> operations.
    /// 
    /// It extends <see cref="IApplicationService"/>.
    /// Thus, ABP enables automatic dependency injection, validation and other benefits for it.
    /// 
    /// Application services works with DTOs (Data Transfer Objects).
    /// Service methods gets and returns DTOs.
    /// </summary>
    public interface IIowLevelAppService : IApplicationService
    {
        IowLevelDto GetLevel(GetLevelInput input);
        GetAllLevelsOutput GetAllLevels();
        IowLevelDto UpdateLevel(UpdateLevelInput input);
        DeleteLevelOutput DeleteLevel(GetLevelInput input);
        void CreateDefaultLevels();
    }
}
