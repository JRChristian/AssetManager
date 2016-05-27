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
        private readonly AssetHierarchyRepository _assetHierarchyRepository;
        private readonly IRepository<AssetType, long> _assetTypeRepository;

        public AssetManager(AssetRepository assetRepository, AssetHierarchyRepository assetHierarchyRepository, IRepository<AssetType, long> assetTypeRepository)
        {
            _assetRepository = assetRepository;
            _assetHierarchyRepository = assetHierarchyRepository;
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

        public Asset InsertOrUpdateAsset(long? id, string name, string description, long? assetTypeId, string assetTypeName, int tenantId)
        {
            long assetId = -1;

            // Get the asset. This will return null if it does not exist.
            Asset asset = GetAsset(id, name);

            // Get the asset type, if possible. This will return null if it does not exist.
            AssetType assetType = GetAssetType(assetTypeId, assetTypeName);

            if ( asset != null )
            {
                // Asset exists - update
                asset.Description = description;
                asset.AssetTypeId = assetType != null ? assetType.Id : -1;
                assetId = asset.Id;
            }
            else if( !string.IsNullOrEmpty(name) )
            {
                // Asset does not exist - create
                asset = new Asset
                {
                    Name = name,
                    Description = !string.IsNullOrEmpty(description) ? description : name,
                    AssetTypeId = assetType != null ? assetType.Id : -1,
                    TenantId = tenantId
                };
                assetId = _assetRepository.InsertOrUpdateAndGetId(asset);
                asset.Id = assetId;
            }
            return asset;
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

        public bool InsertOrUpdateAssetType(long? id, string name, int tenantId)
        {
            bool success = false;
            AssetType assetType = null;

            // Get the asset type
            if (id.HasValue && id.Value > 0)
                assetType = _assetTypeRepository.FirstOrDefault(id.Value);
            else if (!string.IsNullOrEmpty(name))
                assetType = _assetTypeRepository.FirstOrDefault(p => p.Name == name);

            // No need to do anything if the asset type exists. Insert if it doesn't exist.
            if (assetType == null)
            {
                assetType = new AssetType { Name = name };
                _assetTypeRepository.Insert(assetType);
                success = true;
            }
            return success;
        }

        public bool DeleteAssetType(long? id, string name)
        {
            bool success = false;
            AssetType assetType = null;

            // Get the asset type
            if (id.HasValue && id.Value > 0)
                assetType = _assetTypeRepository.FirstOrDefault(id.Value);
            else if (!string.IsNullOrEmpty(name))
                assetType = _assetTypeRepository.FirstOrDefault(p => p.Name == name);

            if( assetType != null)
            {
                // Asset type exists. Check to see if it is used and delete only if unused.
                if( 0 == _assetRepository.Count(p => p.AssetTypeId == assetType.Id) )
                {
                    _assetTypeRepository.Delete(assetType);
                    success = true;
                }
            }
            return success;
        }


        public bool InsertOrUpdateAssetHierarchy(long childAssetId, string parentAssetName)
        {
            Asset childAsset = GetAsset(childAssetId);
            Asset parentAsset = GetAsset(parentAssetName);
            return InsertOrUpdateAssetHierarchy(childAsset, parentAsset);
        }

        public bool InsertOrUpdateAssetHierarchy(string childAssetName, string parentAssetName)
        {
            bool success = false;

            Asset childAsset = GetAsset(childAssetName);
            if( childAsset != null )
            {
                Asset parentAsset = GetAsset(parentAssetName);
                success = InsertOrUpdateAssetHierarchy(childAsset, parentAsset);
            }
            return success;
        }

        public bool InsertOrUpdateAssetHierarchy(Asset childAsset, Asset parentAsset)
        {
            /* The input includes a parent and child. The child exists; the parent might not.
             * Any and all records in the hierarchy with the child asset (in the child position) will be updated
             * to point to the new parent, if specified, or to no parent, if a parent isn't specified.
             * If nothing is found with the child asset, a new record will be inserted.
             * 
             * Limitation: This routine looks for the first parent. It won't work properly if the parent appears twice.
             */
            bool success = false;
            AssetHierarchy parentAssetHierarchy = null;

            // If the parent is specified in the argument list, get the first time the parent appears in the hierarchy.
            // If the parent asset is specified but does not appear in the hierarchy, add the parent to the hierarchy. 
            if( parentAsset != null )
            {
                // Look for a record in the hierarchy for the parent.
                // If there isn't one, create a new top level record in the hierarchy for the parent.
                parentAssetHierarchy = _assetHierarchyRepository.FirstOrDefault(p => p.AssetId == parentAsset.Id);
                if ( parentAssetHierarchy == null )
                {
                    parentAssetHierarchy = new AssetHierarchy
                    {
                        TenantId = childAsset.TenantId,
                        AssetId = parentAsset.Id
                        // ParentAssetHierarchyId -- not set, as this is a new top level
                    };
                    long id = _assetHierarchyRepository.InsertAndGetId(parentAssetHierarchy);
                    parentAssetHierarchy.Id = id;
                }
            }

            // Get all records in the AssetHierarchy table with the child asset
            List<AssetHierarchy> assetHierarchies = _assetHierarchyRepository.GetAllList(p => p.AssetId == childAsset.Id);

            if (assetHierarchies != null && assetHierarchies.Count > 0)
            {
                // The child already appears in the hierarchy.
                // Loop through all records in the hierarchy and update them to the new parent (if it exists) or null (if not).
                foreach( AssetHierarchy hierarchy in assetHierarchies )
                {
                    if (parentAssetHierarchy != null)
                    {
                        // A record exists in the hierarchy for the parent: we have a parent
                        hierarchy.ParentAssetHierarchyId = parentAssetHierarchy.Id;
                    }
                    else
                    {
                        // A record does not exist in the hierarchy for the parent: we do not have a parent
                        long? parentAssetHierarchyId = null;
                        hierarchy.ParentAssetHierarchyId = parentAssetHierarchyId;
                    }
                }
            }
            else
            {
                // Nothing exists in the AssetHierarchy table, so insert a new record for the child.
                if (parentAssetHierarchy != null)
                {
                    // A record exists in the hierarchy for the parent: we have a parent
                    AssetHierarchy hierarchy = new AssetHierarchy
                    {
                        TenantId = childAsset.TenantId,
                        AssetId = childAsset.Id,
                        ParentAssetHierarchyId = parentAssetHierarchy.Id
                    };
                    long id = _assetHierarchyRepository.InsertAndGetId(hierarchy);
                }
                else
                {
                    // A record does not exist in the hierarchy for the parent: we do not have a parent
                    AssetHierarchy hierarchy = new AssetHierarchy
                    {
                        TenantId = childAsset.TenantId,
                        AssetId = childAsset.Id
                        // ParentAssetHierarchyId -- not set, as this is a new top level
                    };
                    long id = _assetHierarchyRepository.InsertAndGetId(hierarchy);
                }
            }

            return success;
        }
    }
}
