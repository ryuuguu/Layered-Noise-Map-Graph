using System;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class RuntimeVariable {
        public string name;
        public string variableKind; //not implemented yet, used for subgraphs
        public string dataType; // type does not serialize in inspector so using string
        public int intVal;
        public float floatVal;
        public float[,] map;

        public override string ToString() {
            return $"RuntimeVariable {name} {dataType} {intVal} {floatVal}";
        }
    }
}