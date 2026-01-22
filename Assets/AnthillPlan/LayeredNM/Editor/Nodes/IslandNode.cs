using System;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {

    /// <summary>
    /// calls HeightmapUtility.IslandMap
    /// which lowers the map so that the part above height 0 forms an island
    /// </summary>
    [Serializable]
    internal class IslandNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new IslandRuntimeNode();
        }

        protected override void OnDefinePorts(Node.IPortDefinitionContext context) {
            context.AddInputPort<float[,]>(LayeredNmRuntimeNode.PortMapIn)
                .WithDisplayName("Map")
                .Build();

            //OutPorts
            context.AddOutputPort<float[,]>(LayeredNmRuntimeNode.PortMapOut)
                .WithDisplayName("Map")
                .Build();
        }
    }
}