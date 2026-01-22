using System;
using System.Linq;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class SumRuntimeNode : LayeredNmRuntimeNode {
        
        /// <summary>
        /// Sum all linked in ports and send Map to out port
        /// if there are no linked in ports Map will be all 0s
        /// </summary>
        /// <param name="ctx"></param>
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            var mapOut = FindPort(PortMapOut);
            mapOut.map = new float[ctx.size.x, ctx.size.y];  //initialized to default 0
            
            foreach (var port in ports) {
                if (!port.inPort) continue;
                if (port.map == null) continue;
                for (int i = 0; i <mapOut.map.GetLength(0); i++) {
                    for (int j = 0; j < mapOut.map.GetLength(1); j++) {
                        mapOut.map[i, j] += port.map[i, j];
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
