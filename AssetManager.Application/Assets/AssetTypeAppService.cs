using Abp.Application.Services;
using Abp.AutoMapper;
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

namespace AssetManager.Assets
{
    /// <summary>
    /// Implements <see cref="IAssetTypeAppService"/> to perform asset type related application functionality.
    /// 
    /// Inherits from <see cref="ApplicationService"/>.
    /// <see cref="ApplicationService"/> contains some basic functionality common for application services (such as logging and localization).
    /// </summary>
    public class AssetTypeAppService : AssetManagerAppServiceBase, IAssetTypeAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<AssetType, long> _assetTypeRepository;
        private readonly IAssetManager _assetManager;

        public AssetTypeAppService(IRepository<AssetType, long> assetTypeRepository, IAssetManager assetManager)
        {
            _assetTypeRepository = assetTypeRepository;
            _assetManager = assetManager;
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetAllAssetTypeOutput> GetAllAssetTypes()
        {
            var assettypes = await _assetManager.GetAssetTypeListAsync();
            return new GetAllAssetTypeOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }
   }
}
