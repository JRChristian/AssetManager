using Abp.Application.Services;
using AssetManager.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Assets
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
    public interface IAssetAppService : IApplicationService
    {
        GetAssetOutput GetAssets(GetAssetInput input);
        GetOneAssetOutput GetOneAsset(GetOneAssetInput input);
        void UpdateAsset(UpdateAssetInput input);
        void CreateAsset(CreateAssetInput input);

        GetAllAssetTypeOutput GetAssetTypes();
        Task<GetAllAssetTypeOutput> GetAssetTypesAsync();
    }
}
