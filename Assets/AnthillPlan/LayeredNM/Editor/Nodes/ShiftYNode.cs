using System;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {
    /// <summary>
    /// Add FloatPort0 to all Y values in map
    /// </summary>
    [Serializable]
    internal class ShiftYNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new ShiftYRuntimeNode();
        }
        protected override void OnDefinePorts(Node.IPortDefinitionContext context) {
            context.AddInputPort<float[,]>(LayeredNmRuntimeNode.PortMapIn)
                .WithDisplayName("Map")
                .Build();
            context.AddInputPort<float>(LayeredNmRuntimeNode.FloatPort0)
                .WithDisplayName("Shift By")
                .Build();
            
            //OutPorts
            context.AddOutputPort<float[,]>(LayeredNmRuntimeNode.PortMapOut)
                .WithDisplayName("Map")
                .Build();
        }
    }
}