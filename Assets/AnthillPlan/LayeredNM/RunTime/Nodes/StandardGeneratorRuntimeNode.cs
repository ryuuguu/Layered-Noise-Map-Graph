using System;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class StandardGeneratorRuntimeNode : LayeredNmRuntimeNode {
        
        public override string DebugName() {
            return $" {this.GetType()}: {(HeightmapGenerator.HeightmapType) options[0].intVal} {id} ";
        }
        
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            var hmg = new HeightmapGenerator() {
                width = ctx.size.x,
                height = ctx.size.y,
                seed = ctx.randomSeed,

                heightmapType = (HeightmapGenerator.HeightmapType) options[0].intVal,
                noiseScale = FindPort(PortNoiseScaleIn).floatVal,
                octaves = FindPort(PortOctavesIn).intVal,
                lacunarity = FindPort(PortLacunarityIn).floatVal,
                persistence = FindPort(PortPersistenceIn).floatVal,
                heightMultiplier = 1,
            };
            var mapOut = FindPort(PortMapOut);
            mapOut.map = HeightmapUtility.GenerateNoiseMap(hmg);
            foreach (var port in mapOut.linkedPorts) {
                 ctx.dictIdToRtNode[port.nodeId].ports.
                         Find(p=> p.name== port.name).map = mapOut.map;
            }
        }
    }
}
