using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.Assets.Dtos;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
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

        public UpdateAssetsOutput UpdateAssets(UpdateAssetsInput input)
        {
            UpdateAssetsOutput output = new UpdateAssetsOutput { SuccessfulUpdates = 0, FailedUpdates = 0 };

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            if ( input.Assets != null )
            {
                foreach(AssetDto asset in input.Assets)
                {
                    _assetManager.InsertOrUpdateAsset(null, asset.Name, asset.Description, asset.AssetTypeId, asset.AssetTypeName, tenantId);
                    output.SuccessfulUpdates++;
                }
            }
            return output;
        }


        public GetAssetTypesOutput GetAssetTypes()
        {
            List<AssetType> assettypes = _assetManager.GetAssetTypeList();
            return new GetAssetTypesOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetAssetTypesOutput> GetAssetTypesAsync()
        {
            var assettypes = await _assetManager.GetAssetTypeListAsync();
            return new GetAssetTypesOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }

        public UpdateAssetTypesOutput UpdateAssetTypes(UpdateAssetTypesInput input)
        {
            // Attempt to add or change each asset type in the input. Report the number of successful operations.
            UpdateAssetTypesOutput output = new UpdateAssetTypesOutput { SuccessfulUpdates = 0, FailedUpdates = 0 };

            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            if (input.AssetTypes != null)
            {
                foreach (AssetTypeDto assetType in input.AssetTypes)
                {
                    if (_assetManager.InsertOrUpdateAssetType(assetType.Id, assetType.Name, tenantId))
                        output.SuccessfulUpdates++;
                    else
                        output.FailedUpdates++;
                }
            }
            return output;
        }

        public DeleteAssetTypesOutput DeleteAssetTypes(DeleteAssetTypesInput input)
        {
            // Attempt to delete each asset type in the input. Report the number of successfully deleted asset types.
            DeleteAssetTypesOutput output = new DeleteAssetTypesOutput { SuccessfulDeletions = 0, FailedDeletions = 0 };

            if( input.AssetTypes != null )
            {
                foreach( AssetTypeDto assetType in input.AssetTypes )
                {
                    if (_assetManager.DeleteAssetType(assetType.Id, assetType.Name))
                        output.SuccessfulDeletions++;
                    else
                        output.FailedDeletions++;
                }
            }
            return output;
        }

        public GetAssetHierarchyAsListOutput GetAssetHierarchyAsList(GetAssetHierarchyAsListInput input)
        {
            // Output array
            List<AssetHierarchyDto> flatHierarchy = new List<AssetHierarchyDto>();

            // Get all nodes
            List<AssetHierarchy> rawHierarchy = _assetManager.GetAssetHierarchy();
            HierarchyBuilder(rawHierarchy, ref flatHierarchy, 0, null, "");

            return new GetAssetHierarchyAsListOutput
            {
                AssetHierarchy = flatHierarchy
            };
        }

        private void HierarchyBuilder(List<AssetHierarchy> rawHierarchy, ref List<AssetHierarchyDto> flatHierarchy, int level, long? parentAssetHierarchyId, string parentAssetName)
        {
            var oneLevel = from assets in rawHierarchy
                           where assets.ParentAssetHierarchyId == parentAssetHierarchyId
                           orderby assets.Asset.Name ascending
                           select assets;

            if( oneLevel != null && oneLevel.Count() > 0)
            {
                foreach (var oneItem in oneLevel)
                {
                    AssetHierarchyDto hierarchy = new AssetHierarchyDto
                    {
                        Name = oneItem.Asset.Name,
                        Description = oneItem.Asset.Description,
                        AssetTypeName = oneItem.Asset.AssetType.Name,
                        ParentAssetName = parentAssetName,
                        Level = level
                    };
                    flatHierarchy.Add(hierarchy);
                
                    // And add any children
                    HierarchyBuilder(rawHierarchy, ref flatHierarchy, level + 1, oneItem.Id, oneItem.Asset.Name);
                }
            }
        }

        public GetAssetTreeOutput GetAssetTree(GetAssetTreeInput input)
        {
            // Output array
            List<AssetTreeDto> tree = new List<AssetTreeDto>();

            // Get all nodes
            List<AssetHierarchy> assetHierarchy = _assetManager.GetAssetHierarchy();

            return new GetAssetTreeOutput
            {
                AssetTree = AngularUITreeBuilder(assetHierarchy, 0, null, null)
            };
        }

        private List<AssetTreeDto> AngularUITreeBuilder(List<AssetHierarchy> assetHierarchy, int level, long? parentAssetHierarchyId, long? parentAssetTreeDtoId )
        {
            List<AssetTreeDto> output = new List<AssetTreeDto>();

            var oneLevel = from assets in assetHierarchy
                           where assets.ParentAssetHierarchyId == parentAssetHierarchyId
                           orderby assets.Asset.Name ascending
                           select assets;

            if (oneLevel != null && oneLevel.Count() > 0)
            {
                foreach (var oneItem in oneLevel)
                {
                    AssetTreeDto node = new AssetTreeDto
                    {
                        Id = oneItem.AssetId,
                        Title = oneItem.Asset.Name,
                        Description = oneItem.Asset.Description,
                        Level = level,
                        ParentAssetTreeDtoId = parentAssetTreeDtoId,
                        Nodes = AngularUITreeBuilder(assetHierarchy, level + 1, oneItem.Id, oneItem.AssetId)
                    };

                    output.Add(node);
                }
            }
            return output;
        }


        public UpdateAssetHierarchyOutput UpdateAssetHierarchy(UpdateAssetHierarchyInput input)
        {
            UpdateAssetHierarchyOutput output = new UpdateAssetHierarchyOutput { Updates = 0 };
            string parentAssetName = "";
            bool success = false;
            
            // All assets belong to a tenant. If not specified, put them in the default tenant.
            int tenantId = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

            foreach (AssetHierarchyDto asset in input.Assets)
            {
                if (asset.ParentAssetName != "*")
                    parentAssetName = asset.ParentAssetName;
                else
                    parentAssetName = "";
                
                Asset childAsset = _assetManager.InsertOrUpdateAsset(null, asset.Name, asset.Description, null, asset.AssetTypeName, tenantId);
                Asset parentAsset = _assetManager.GetAsset(parentAssetName);
                success = _assetManager.InsertOrUpdateAssetHierarchy(childAsset, parentAsset);

                if( success )
                    output.Updates++;
            }
            return output;
        }

        public GetAssetRelativesOutput GetAssetRelatives(GetAssetRelativesInput input)
        {
            GetAssetRelativesOutput output = new GetAssetRelativesOutput { };

            Asset asset = _assetManager.GetAsset(input.Id, input.Name);
            if (asset != null)
            {
                output.Asset = asset.MapTo<AssetDto>();
                output.Parent = _assetManager.GetAssetParent(asset.Id, asset.Name).MapTo<AssetDto>();
                output.Children = _assetManager.GetAssetChildren(asset.Id, asset.Name, false).MapTo<List<AssetDto>>();
            }
            return output;
        }
    }
}
