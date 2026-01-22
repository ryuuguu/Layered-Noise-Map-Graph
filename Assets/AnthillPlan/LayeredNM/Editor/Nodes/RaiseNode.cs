using System;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {
    /// <summary>
    /// multiple all Y values in map by FloatPort0  
    /// </summary>
    [Serializable]
    internal class RaiseNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new RaiseRuntimeNode();
        }
        protected override void OnDefinePorts(Node.IPortDefinitionContext context) {
            context.AddInputPort<float[,]>(LayeredNmRuntimeNode.PortMapIn)
                .WithDisplayName("Map")
                .Build();
            context.AddInputPort<float>(LayeredNmRuntimeNode.FloatPort0)
                .WithDisplayName("Raise")
                .Build();
            
            //OutPorts
            context.AddOutputPort<float[,]>(LayeredNmRuntimeNode.PortMapOut)
                .WithDisplayName("Map")
                .Build();
        }
    }

}