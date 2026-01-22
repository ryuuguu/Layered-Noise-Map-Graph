using System;
using Unity.GraphToolkit.Editor;

namespace AnthillPlan.LayeredNM.Editor {
    /// <summary>
    /// generates maps for generators that use standard input ports
    /// </summary>
    [Serializable]
    internal class StandardGeneratorNode : LayeredNmNode {
        public override LayeredNmRuntimeNode NewRuntimeNode() {
            return new StandardGeneratorRuntimeNode();
        }

        protected override void OnDefineOptions(Node.IOptionDefinitionContext context) {
            context.AddOption("Type", typeof(HeightmapGenerator.HeightmapType))
                .WithDefaultValue(HeightmapGenerator.HeightmapType.Perlin)
                .Delayed().Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort<float>(LayeredNmRuntimeNode.PortNoiseScaleIn)
                .WithDisplayName("Noise Scale")
                .WithDefaultValue(10f)
                .Build();
            context.AddInputPort<int>(LayeredNmRuntimeNode.PortOctavesIn)
                .WithDisplayName("Octaves")
                .WithDefaultValue(1)
                .Build();
            context.AddInputPort<float>(LayeredNmRuntimeNode.PortLacunarityIn)
                .WithDisplayName("Lacunarity")
                .WithDefaultValue(0.5f)
                .Build();
            context.AddInputPort<float>(LayeredNmRuntimeNode.PortPersistenceIn)
                .WithDisplayName("Persistence")
                .WithDefaultValue(0.5f)
                .Build();

            //OutPorts
            context.AddOutputPort<float[,]>(LayeredNmRuntimeNode.PortMapOut)
                .WithDisplayName("Map")
                .Build();
        }
        
    }
}
