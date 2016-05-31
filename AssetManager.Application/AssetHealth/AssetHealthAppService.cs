using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.AssetHealth.Dtos;
using AssetManager.DomainServices;
using AssetManager.Entities;
using AssetManager.EntityFramework.DomainServices;
using AssetManager.Utilities;
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

        // This routine is similar to GetAssetHierarchyAsList() in the Asset Manager, with the addition of variables associated with each asset.
        public GetAssetHierarchyWithVariablesAsListOutput GetAssetHierarchyWithVariablesAsList(GetAssetHierarchyWithVariablesAsListInput input)
        {
            // Output array
            List<AssetVariableArrayDto> flatHierarchy = new List<AssetVariableArrayDto>();

            // Get all nodes
            List<AssetHierarchy> rawHierarchy = _assetManager.GetAssetHierarchy();
            HierarchyBuilder(rawHierarchy, ref flatHierarchy, 0, null, "");

            return new GetAssetHierarchyWithVariablesAsListOutput
            {
                AssetHierarchy = flatHierarchy
            };
        }

        private void HierarchyBuilder(List<AssetHierarchy> rawHierarchy, ref List<AssetVariableArrayDto> flatHierarchy, int level, long? parentAssetHierarchyId, string parentAssetName)
        {
            var oneLevel = from assets in rawHierarchy
                           where assets.ParentAssetHierarchyId == parentAssetHierarchyId
                           orderby assets.Asset.Name ascending
                           select assets;

            if (oneLevel != null && oneLevel.Count() > 0)
            {
                foreach (var oneItem in oneLevel)
                {
                    AssetVariableArrayDto hierarchy = new AssetVariableArrayDto
                    {
                        Id = oneItem.AssetId,
                        Name = oneItem.Asset.Name,
                        Description = oneItem.Asset.Description,
                        AssetTypeName = oneItem.Asset.AssetType.Name,
                        ParentAssetName = parentAssetName,
                        Level = level,
                        Variables = new List<VariableArrayDto>()
                    };
                    // Now add the child variables, if any
                    if( oneItem.Asset != null && oneItem.Asset.Variables != null )
                    {
                        foreach(var v in oneItem.Asset.Variables)
                        {
                            VariableArrayDto variable = new VariableArrayDto
                            {
                                Id = v.IOWVariableId,
                                Name = v.IOWVariable.Name,
                                Description = v.IOWVariable.Description,
                                TagName = v.IOWVariable.Tag.Name,
                                UOM = v.IOWVariable.UOM,
                                IsAssigned = IsVariableAssignedToAsset.Yes
                            };
                            hierarchy.Variables.Add(variable);
                        }
                        hierarchy.Variables = hierarchy.Variables.OrderBy(p => p.Name).ToList();
                    }

                    flatHierarchy.Add(hierarchy);

                    // And add any children
                    HierarchyBuilder(rawHierarchy, ref flatHierarchy, level + 1, oneItem.Id, oneItem.Asset.Name);
                }
            }
        }

        public UpdateAssetVariableListOutput UpdateAssetVariableList(UpdateAssetVariableListInput input)
        {
            UpdateAssetVariableListOutput output = new UpdateAssetVariableListOutput { NumberUpdates = 0 };
            if( input.AssetVariables != null )
            {
                List<AssetVariableCombinations> thingsToUpdate = new List<AssetVariableCombinations>();
                foreach(AssetVariableDto oneInputItem in input.AssetVariables)
                {
                    AssetVariableCombinations oneOutputItem = new AssetVariableCombinations
                    {
                        AssetName = oneInputItem.AssetName,
                        VariableName = oneInputItem.VariableName,
                        IsAssigned = IsVariableAssignedToAsset.NoAndAdd
                    };
                    thingsToUpdate.Add(oneOutputItem);
                }

                if( thingsToUpdate.Count > 0 )
                {
                    List<AssetVariable> result = _assetHealthManager.UpdateAssetVariableList(thingsToUpdate);
                    if (result != null)
                        output.NumberUpdates = result.Count;
                }
            }
            return output;
        }

        public DeleteAssetVariableListOutput DeleteAssetVariableList(DeleteAssetVariableListInput input)
        {
            DeleteAssetVariableListOutput output = new DeleteAssetVariableListOutput { NumberDeletes = 0 };
            if (input.AssetVariables != null)
            {
                foreach (AssetVariableDto oneInputItem in input.AssetVariables)
                {
                    if( _assetHealthManager.DeleteAssetVariable(null, oneInputItem.AssetName, null, oneInputItem.VariableName) )
                        output.NumberDeletes++;
                }
            }
            return output;
        }
    }
}
