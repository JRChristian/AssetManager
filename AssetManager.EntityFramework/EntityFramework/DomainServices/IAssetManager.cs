using Abp.Domain.Services;
using AssetManager.DomainServices;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public interface IAssetManager : IDomainService
    {
        Asset GetAsset(long id);
        Asset GetAsset(string name);
        Asset GetAsset(long? id, string name);
        List<Asset> GetAssetList();
        List<Asset> GetAssetListForType(long id);
        Asset InsertOrUpdateAsset(long? id, string name, string description, long? assetTypeId, string assetTypeName, int tenantId);

        AssetType GetAssetType(long id);
        AssetType GetAssetType(string name);
        AssetType GetAssetType(long? id, string name);
        List<AssetType> GetAssetTypeList();
        Task<List<AssetType>> GetAssetTypeListAsync();
        bool InsertOrUpdateAssetType(long? id, string name, int tenantId);
        bool DeleteAssetType(long? id, string name);

        List<AssetHierarchy> GetAssetHierarchy();
        bool InsertOrUpdateAssetHierarchy(long childAssetId, string parentAssetName);
        bool InsertOrUpdateAssetHierarchy(string childAssetName, string parentAssetName);
        bool InsertOrUpdateAssetHierarchy(Asset childAsset, Asset parentAsset);
    }
}
