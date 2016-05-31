using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFramework;
using Abp.Runtime.Session;
using AssetManager.DomainServices;
using AssetManager.EntityFramework.Repositories;
using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.EntityFramework.DomainServices
{
    public class AssetHealthManager : DomainService, IAssetHealthManager
    {
        private readonly IAssetVariableRepository _assetVariableRepository;
        private readonly IAssetManager _assetManager;
        private readonly IIowManager _iowManager;

        public AssetHealthManager(
            IAssetVariableRepository assetVariableRepository,
            IAssetManager assetManager,
            IIowManager iowManager
            )
        {
            _assetVariableRepository = assetVariableRepository;
            _assetManager = assetManager;
            _iowManager = iowManager;
        }

        // Variable-Asset assignments
        public List<AssetVariable> GetAssetVariableList()
        {
            return _assetVariableRepository.GetAll().OrderBy(p => p.Asset.Name).ThenBy(p => p.IOWVariable.Name).ToList();
        }

        public List<AssetVariable> GetAssetVariableList(long? assetId, string assetName, long? variableId, string variableName)
        {
            List<AssetVariable> output = null;

            // Get the asset and/or variable specified in the input
            Asset asset = _assetManager.GetAsset(assetId, assetName);
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(variableId, variableName);

            // If the input included a valid asset and/or variable, use them to subset the list
            if (asset != null && variable != null) // Should be just 0 or 1 items found
                output = _assetVariableRepository.GetAllList(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
            else if (asset != null) // All variables for a specified asset
                output = _assetVariableRepository.GetAll().Where(p => p.AssetId == asset.Id).OrderBy(p => p.IOWVariable.Name).ToList();
            else if (variable != null) // All assets for a specified variable
                output = _assetVariableRepository.GetAll().Where(p => p.IOWVariableId == variable.Id).OrderBy(p => p.Asset.Name).ToList();
            else // Everything
                output = _assetVariableRepository.GetAll().OrderBy(p => p.Asset.Name).ThenBy(p => p.IOWVariable.Name).ToList();

            return output;
        }

        public List<AssetVariable> UpdateAssetVariableList(List<AssetVariableCombinations> input)
        {
            List<AssetVariable> output = new List<AssetVariable>();
            Asset asset = null;
            IOWVariable variable = null;
            AssetVariable av = null;

            // Loop through the input, validate the inputs, and then insert or update the cross-combination record
            foreach (AssetVariableCombinations one in input)
            {
                asset = _assetManager.GetAsset(one.AssetId, one.AssetName);
                variable = _iowManager.FirstOrDefaultVariable(one.VariableId, one.VariableName);
                if (asset != null && variable != null)
                {
                    av = _assetVariableRepository.FirstOrDefault(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
                    if (av == null)
                    {
                        av = new AssetVariable { AssetId = asset.Id, IOWVariableId = variable.Id, TenantId = asset.TenantId };
                        av = _assetVariableRepository.InsertOrUpdate(av);
                    }

                    output.Add(av);
                }
            }
            return output;
        }

        public bool DeleteAssetVariable(long? assetId, string assetName, long? variableId, string variableName)
        {
            bool success = false;

            Asset asset = _assetManager.GetAsset(assetId, assetName);
            IOWVariable variable = _iowManager.FirstOrDefaultVariable(variableId, variableName);
            if (asset != null && variable != null)
            {
                AssetVariable input = _assetVariableRepository.FirstOrDefault(p => p.AssetId == asset.Id && p.IOWVariableId == variable.Id);
                if (input != null)
                {
                    _assetVariableRepository.Delete(input);
                    success = true;
                }
            }
            return success;
        }
    }
}
