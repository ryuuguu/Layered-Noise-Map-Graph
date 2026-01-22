using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    public class LayeredNmRuntimeGraph :ScriptableObject {
        [SerializeReference] public List<LayeredNmRuntimeNode> nodeList;
        [Serialize] public List<RuntimeVariable> blackboardVariables;
        [Serialize] public GraphContext ctx;

        public ErrorMessage errorMessage = new();
        
        [Serializable]
        public class GraphContext {
            // global graph variables
            public Vector2Int size = new Vector2Int(4, 4);
            public float scale = 1;
            public int randomSeed = 0;
 
            // Graph output
            public float[,] resultMap;
          
            public Dictionary<int, LayeredNmRuntimeNode> dictIdToRtNode = new ();
            public List<RuntimeVariable> blackboardVariables = new();
        }
        
        public float[,] ExecuteGraph(GraphContext aCtx) {
            if (errorMessage.isError) {
                return new float[ctx.size.x, ctx.size.y];
            }
            
            //build id to node dictionary
            //this needs to built again since unity serialization won't handle
            //recursive references or dictionaries.
            ctx.dictIdToRtNode = new();
            foreach (var node in nodeList) {
                ctx.dictIdToRtNode[node.id] = node;
            }
            foreach (var node in  nodeList) {
                 node.Execute(aCtx);
            }
            return aCtx.resultMap;
        }
    }
}