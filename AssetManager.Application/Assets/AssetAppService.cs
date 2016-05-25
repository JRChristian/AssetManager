using Abp.Application.Services;
using Abp.Domain.Repositories;
using AssetManager.Entities;
using AssetManager.Assets.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManager.EntityFramework.DomainServices;
using Abp.AutoMapper;

namespace AssetManager.Assets
{
    /// <summary>
    /// Implements <see cref="IAssetAppService"/> to perform Asset related application functionality.
    /// 
    /// Inherits from <see cref="ApplicationService"/>.
    /// <see cref="ApplicationService"/> contains some basic functionality common for application services (such as logging and localization).
    /// </summary>
    public class AssetAppService : AssetManagerAppServiceBase /* ApplicationService */, IAssetAppService
    {
        //These members set in constructor using constructor injection.

        //private readonly IAssetRepository _assetRepository;
        //private readonly IRepository<AssetType,long> _assetTypeRepository;
        private readonly IAssetManager _assetManager;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public AssetAppService(/*IAssetRepository assetRepository, IRepository<AssetType,long> assetTypeRepository, */ IAssetManager assetManager)
        {
            //_assetRepository = assetRepository;
            //_assetTypeRepository = assetTypeRepository;
            _assetManager = assetManager;
        }

        public GetOneAssetOutput GetOneAsset(GetOneAssetInput input)
        {
            //Get one asset, using either Id (if specified) or name.
            Asset asset = _assetManager.GetAsset(input.Id, input.Name);

            //Used AutoMapper to automatically convert Asset to AssetDto.
            return new GetOneAssetOutput
            {
                Asset = Mapper.Map<AssetDto>(asset)
            };
        }

        public GetAssetOutput GetAssets(GetAssetInput input)
        {
            //Called specific GetAllWithType method of Asset repository.
            List<Asset> assets = null;
            if (input.AssetTypeId.HasValue)
                assets = _assetManager.GetAssetListForType(input.AssetTypeId.Value);
            else
                assets = _assetManager.GetAssetList();

            //Used AutoMapper to automatically convert List<Asset> to List<AssetDto>.
            return new GetAssetOutput
            {
                Assets = Mapper.Map<List<AssetDto>>(assets)
            };
        }

        public void UpdateAsset(UpdateAssetInput input)
        {
            //We can use Logger, it is defined in ApplicationService base class.
            Logger.Info("Updating an asset for input: " + input);

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            _assetManager.InsertOrUpdateAsset(input.Id, input.Name, input.Description, input.AssetTypeId, "", tenantId);

            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }

        public void CreateAsset(CreateAssetInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating an asset for input: " + input);

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            //Creating a new Asset entity with given input's properties
            _assetManager.InsertOrUpdateAsset(null, input.Name, input.Description, input.AssetTypeId, input.AssetTypeName, tenantId);
        }


        public GetAllAssetTypeOutput GetAssetTypes()
        {
            List<AssetType> assettypes = _assetManager.GetAssetTypeList();
            return new GetAllAssetTypeOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetAllAssetTypeOutput> GetAssetTypesAsync()
        {
            var assettypes = await _assetManager.GetAssetTypeListAsync();
            return new GetAllAssetTypeOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }
    }
}
