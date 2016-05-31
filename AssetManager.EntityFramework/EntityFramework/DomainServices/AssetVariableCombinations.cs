using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.DomainServices
{
    public class AssetVariableCombinations
    {
        public long? AssetId { get; set; }
        public string AssetName { get; set; }
        public long? VariableId { get; set; }
        public string VariableName { get; set; }
        public IsVariableAssignedToAsset IsAssigned { get; set; }
    }
}
