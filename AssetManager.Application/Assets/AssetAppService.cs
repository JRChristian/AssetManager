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

        private readonly IAssetRepository _assetRepository;
        private readonly IRepository<AssetType,long> _assetTypeRepository;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public AssetAppService(IAssetRepository assetRepository, IRepository<AssetType,long> assetTypeRepository)
        {
            _assetRepository = assetRepository;
            _assetTypeRepository = assetTypeRepository;
        }

        public GetOneAssetOutput GetOneAsset(GetOneAssetInput input)
        {
            Asset asset;

            //Get one asset, using either Id (if specified) or name.
            if( input.Id.HasValue)
            {
                asset = _assetRepository.Get(input.Id.Value);
            }
            else
            {
                asset = _assetRepository.FirstOrDefault(x => x.Name == input.Name);
            }

            //Used AutoMapper to automatically convert Asset to AssetDto.
            return new GetOneAssetOutput
            {
                Asset = Mapper.Map<AssetDto>(asset)
            };
        }

        public GetAssetOutput GetAssets(GetAssetInput input)
        {
            //Called specific GetAllWithType method of Asset repository.
            var assets = _assetRepository.GetAllWithType(input.AssetTypeId);

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

            //Retrieving a Asset entity with given id using standard Get method of repositories.
            var Asset = _assetRepository.Get(input.Id);

            //Updating changed properties of the retrieved Asset entity.
            if (!string.IsNullOrEmpty(input.Name))
                Asset.Name = input.Name;

            if (!string.IsNullOrEmpty(input.Description))
                Asset.Description = input.Description;

            if (input.AssetTypeId.HasValue)
            {
                Asset.AssetType = _assetTypeRepository.Load(input.AssetTypeId.Value);
            }

            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }

        public void CreateAsset(CreateAssetInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating an asset for input: " + input);

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            //Creating a new Asset entity with given input's properties
            var Asset = new Asset { Name = input.Name, Description = input.Description, AssetTypeId = -1, TenantId = tenantid };

            // The DTO includes both AssetTypeId and AssetTypeName. If specified, use ID.
            if (input.AssetTypeId.HasValue)
            {
                Asset.AssetTypeId = input.AssetTypeId.Value;
            }
            else if (!string.IsNullOrEmpty(input.AssetTypeName))
            {
                var assettype = _assetTypeRepository.FirstOrDefault(x => x.Name == input.AssetTypeName);
                if (assettype != null) 
                    Asset.AssetTypeId = assettype.Id;
            }

            //Saving entity with standard Insert method of repositories.
            _assetRepository.Insert(Asset);
        }
    }
}
