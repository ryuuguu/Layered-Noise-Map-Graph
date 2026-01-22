using UnityEngine;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {
    /// <summary>
    /// Sums the Y values of from 1 to 20 maps
    /// </summary>[Serializable]
    internal class SumNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new SumRuntimeNode();
        }

        protected override void OnDefineOptions(Node.IOptionDefinitionContext context) {
            context.AddOption(LayeredNmRuntimeNode.DynamicPortCount, typeof(int))
                .WithDefaultValue(2)
                .Delayed().Build();
        }
        protected override void OnDefinePorts(Node.IPortDefinitionContext context) {
            GetNodeOptionByName(LayeredNmRuntimeNode.DynamicPortCount).TryGetValue(out int n);
            n = Mathf.Clamp(n, 2, 20);
            for (int i = 0; i < n; i++) {
                context.AddInputPort<float[,]>($"{LayeredNmRuntimeNode.DynamicPortIn}{i}")
                    .WithDisplayName($"Map {i}")
                    .WithDefaultValue(new float[0, 0])
                    .Build();
            }
            //OutPorts
            context.AddOutputPort<float[,]>(LayeredNmRuntimeNode.PortMapOut)
                .WithDisplayName("Map")
                .Build();
        }
    }
}