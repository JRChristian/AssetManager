using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.Entities;
using AssetManager.Services.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services
{
    /// <summary>
    /// Implements <see cref="IAssetTypeAppService"/> to perform asset type related application functionality.
    /// 
    /// Inherits from <see cref="ApplicationService"/>.
    /// <see cref="ApplicationService"/> contains some basic functionality common for application services (such as logging and localization).
    /// </summary>
    public class AssetTypeAppService : IAssetTypeAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<AssetType, long> _assetTypeRepository;
 
        public AssetTypeAppService(IRepository<AssetType, long> assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetAllAssetTypeOutput> GetAllAssetTypes()
        {
            var assettypes = await _assetTypeRepository.GetAllListAsync();
            return new GetAllAssetTypeOutput
            {
                AssetTypes = assettypes.MapTo<List<AssetTypeDto>>()
            };
        }
   }
}
