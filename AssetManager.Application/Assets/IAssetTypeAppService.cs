using Abp.Application.Services;
using AssetManager.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets
{
    public interface IAssetTypeAppService : IApplicationService
    {
        /// <summary>
        /// Defines an application service for <see cref="AssetType"/> operations.
        /// 
        /// It extends <see cref="IApplicationService"/>.
        /// Thus, ABP enables automatic dependency injection, validation and other benefits for it.
        /// 
        /// Application services works with DTOs (Data Transfer Objects).
        /// Service methods gets and returns DTOs.
        /// </summary>
        Task<GetAssetTypesOutput> GetAllAssetTypes();
    }
}
