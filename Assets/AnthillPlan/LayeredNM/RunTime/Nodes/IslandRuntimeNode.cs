using System;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    
    [Serializable]
    public class IslandRuntimeNode : LayeredNmRuntimeNode {
        
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            var mapIn = FindPort(PortMapIn);
            var mapOut = FindPort(PortMapOut);
            mapOut.map = HeightmapUtility.IslandMap(mapIn.map);
            foreach (var port in mapOut.linkedPorts) {
                if (ctx.dictIdToRtNode.TryGetValue(port.nodeId, out var node)) {
                    node.ports.Find(p => p.name == port.name).map = mapOut.map;
                }
            }
        }
    }
}
