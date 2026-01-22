using System;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class ResultMapRuntimeNode : LayeredNmRuntimeNode {
        public override void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            ctx.resultMap = ports[0].map;
        }
    }
    
}