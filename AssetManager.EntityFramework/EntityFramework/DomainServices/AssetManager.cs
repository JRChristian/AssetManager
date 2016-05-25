using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFramework;
using Abp.Runtime.Session;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetManager : DomainService, IAssetManager
    {
        private readonly AssetRepository _assetRepository;
        private readonly IRepository<AssetType, long> _assetTypeRepository;

        public AssetManager(AssetRepository assetRepository, IRepository<AssetType, long> assetTypeRepository)
        {
            _assetRepository = assetRepository;
            _assetTypeRepository = assetTypeRepository;
        }

        public Asset GetAsset(long id)
        {
            return _assetRepository.FirstOrDefault(id);
        }

        public Asset GetAsset(string name)
        {
            return _assetRepository.FirstOrDefault(p => p.Name == name);
        }

        public Asset GetAsset(long? id, string name)
        {
            Asset asset = null;
            if (id.HasValue && id.Value > 0)
                asset = GetAsset(id.Value);
            else if (!string.IsNullOrEmpty(name))
                asset = GetAsset(name);
            return asset;
        }

        public List<Asset> GetAssetList()
        {
            return _assetRepository.GetAll().OrderBy(p => p.Name).ToList();
        }

        public List<Asset> GetAssetListForType(long id)
        {
            // Get the list of assets. GetAll() returns IQueryable<TEntity>, so we can do further wrok.
            var query = _assetRepository.GetAll().Where(p => p.AssetTypeId == id);

            return query
                .OrderBy(p => p.Name)
                .Include(p => p.AssetType) //Include assigned asset type in a single query
                .ToList();
        }

        public bool InsertOrUpdateAsset(long? id, string name, string description, long? assetTypeId, string assetTypeName, int tenantId)
        {
            bool output = false;

            // Get the asset. This will return null if it does not exist.
            Asset asset = GetAsset(id, name);

            // Get the asset type, if possible. This will return null if it does not exist.
            AssetType assetType = GetAssetType(assetTypeId, assetTypeName);

            if ( asset != null )
            {
                // Asset exists - update
                asset.Description = description;
                asset.AssetTypeId = assetType != null ? assetType.Id : -1;
                output = true;
            }
            else
            {
                // Asset does not exist - create
                asset = new Asset
                {
                    Name = name,
                    Description = description,
                    AssetTypeId = assetType != null ? assetType.Id : -1,
                    TenantId = tenantId
                };
                _assetRepository.Insert(asset);
                output = true;
            }
            return output;
        }

        public AssetType GetAssetType(long id)
        {
            return _assetTypeRepository.FirstOrDefault(id);
        }

        public AssetType GetAssetType(string name)
        {
            return _assetTypeRepository.FirstOrDefault(p => p.Name == name);
        }

        public AssetType GetAssetType(long? id, string name)
        {
            AssetType assetType = null;
            if (id.HasValue && id.Value > 0)
                assetType = GetAssetType(id.Value);
            else if (!string.IsNullOrEmpty(name))
                assetType = GetAssetType(name);
            return assetType;
        }

        public List<AssetType> GetAssetTypeList()
        {
            return _assetTypeRepository.GetAll().OrderBy(p => p.Name).ToList();
        }

        public async Task<List<AssetType>> GetAssetTypeListAsync()
        {
            List<AssetType> assettypes = await _assetTypeRepository.GetAllListAsync();
            return assettypes;
        }

    }
}
