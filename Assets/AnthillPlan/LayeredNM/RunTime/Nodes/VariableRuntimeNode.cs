using System;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    
    
    /// <summary>
    /// get the val from the blackboard variable
    /// pass it to the all the linked nodes
    /// </summary>
    [Serializable]
    public class VariableRuntimeNode : LayeredNmRuntimeNode {
        public string variableName;
        public string dataType;
        
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            //get the val from the blackboard variable
            // pass it to the all the linked 
            var rtv = ctx.blackboardVariables.Find(rtv => rtv.name == variableName);

            foreach (var port in ports) { 
                foreach (var linkedPort in port.linkedPorts) {
                   
                    // the out port may be linked to port that dead ends and not needed in graph
                    // this should not stop the graph from executing
                    if (dataType == typeof(int).ToString()) {
                        if (ctx.dictIdToRtNode.TryGetValue(linkedPort.nodeId, out var value)) {
                            value.ports.Find(p => p.name == linkedPort.name)
                                .intVal = rtv.intVal;
                        }
                    }
                    if (dataType == typeof(float).ToString()) {
                        if (ctx.dictIdToRtNode.TryGetValue(linkedPort.nodeId, out var value)) {
                                value.ports.Find(p => p.name == linkedPort.name)
                                    .floatVal = rtv.floatVal;
                        }
                    }
                    if (dataType == typeof(float[,]).ToString()) {
                        if (ctx.dictIdToRtNode.TryGetValue(linkedPort.nodeId, out var value)) {
                            value.ports.Find(p => p.name == linkedPort.name)
                                .map = rtv.map;
                        }
                    }
                }
            }
        }
    }
}