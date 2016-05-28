using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.AssetHealth.Dtos;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHealth
{
    public class AssetHealthAppService : AssetManagerAppServiceBase, IAssetHealthAppService
    {
        private readonly IAssetManager _assetManager;
        private readonly IAssetHealthManager _assetHealthManager;
        private readonly IIowManager _iowManager;

        public AssetHealthAppService( 
            IAssetManager assetManager,
            IAssetHealthManager assetHealthManager,
            IIowManager iowManager
            )
        {
            _assetManager = assetManager;
            _assetHealthManager = assetHealthManager;
            _iowManager = iowManager;
        }

        // Asset-Variable combinations
        public GetAssetVariableListOutput GetAssetVariableList(GetAssetVariableListInput input)
        {
            GetAssetVariableListOutput output = new GetAssetVariableListOutput { AssetVariables = null };

            List<AssetVariable> assetVariables = _assetHealthManager.GetAssetVariableList(input.AssetId, input.AssetName, input.VariableId, input.VariableName);
            if (assetVariables != null)
            {
                output.AssetVariables = new List<AssetVariableDto>();
                foreach (AssetVariable av in assetVariables)
                {
                    output.AssetVariables.Add(new AssetVariableDto { Id = av.Id, AssetName = av.Asset.Name, VariableName = av.IOWVariable.Name });
                }
            }
            return output;
        }

        public UpdateAssetVariableListOutput UpdateAssetVariableList(UpdateAssetVariableListInput input)
        {
            UpdateAssetVariableListOutput output = new UpdateAssetVariableListOutput { NumberUpdates = 0 };

            List<AssetVariableCombinations> combos = input.AssetVariables.MapTo<List<AssetVariableCombinations>>();
            if (combos != null && combos.Count > 0)
            {
                List<AssetVariable> result = _assetHealthManager.UpdateAssetVariableList(combos);
                if (result != null)
                    output.NumberUpdates = result.Count;
            }
            return output;
        }
    }
}
