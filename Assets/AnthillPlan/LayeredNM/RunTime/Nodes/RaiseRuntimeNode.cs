using System;
using System.Linq;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class RaiseRuntimeNode : LayeredNmRuntimeNode {
        
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            var mapIn = FindPort(PortMapIn);
            var mapOut = FindPort(PortMapOut);
            var float0 = FindPort(FloatPort0);
            mapOut.map = new float[ctx.size.x, ctx.size.y]; //initialized to default 0
             if (mapIn.map != null) {
                for (int i = 0; i < mapOut.map.GetLength(0); i++) {
                    for (int j = 0; j < mapOut.map.GetLength(1); j++) {
                        mapOut.map[i, j] = mapIn.map[i, j] * float0.floatVal;
                    }
                }
            }

            foreach (var port in mapOut.linkedPorts) {
                // the out port may be linked to port that dead ends and not needed in graph
                // this should not stop the graph from executing
                if (ctx.dictIdToRtNode.TryGetValue(port.nodeId, out var value)) {
                    value.ports.Find(p => p.name == port.name).map = mapOut.map;

                }
            }
        }
    }
}
