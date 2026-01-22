using System;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {
    
    /// <summary>
    /// PortMapIn value will be the output of the graph
    /// </summary>
    [Serializable]
    internal class ResultMapNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new ResultMapRuntimeNode();
        }
        protected override void OnDefinePorts(Node.IPortDefinitionContext context) {
            context.AddInputPort<float[,]>(LayeredNmRuntimeNode.PortMapIn)
                .WithDisplayName("Map")
                .Build();
        }
    }

}