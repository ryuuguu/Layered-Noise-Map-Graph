using System;
using Unity.GraphToolkit.Editor;


namespace AnthillPlan.LayeredNM.Editor {   
    [Serializable]
    internal abstract class LayeredNmNode : Node {
        public abstract LayeredNmRuntimeNode NewRuntimeNode();
    }
  
}
