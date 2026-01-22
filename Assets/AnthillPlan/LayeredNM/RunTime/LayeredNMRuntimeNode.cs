using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnthillPlan.LayeredNM {
    [Serializable]
    public class LayeredNmRuntimeNode {
        // const placed in Runtime namespace so the both runtime and Editor can access
        
        public const string PortMapIn = "MapIn";
        public const string PortMapOut = "MapOut";
        
        public const string PortNoiseScaleIn = "NoiseScale";
        public const string PortOctavesIn = "Octaves";
        public const string PortLacunarityIn = "Lacunarity";
        public const string PortPersistenceIn = "Persistence";

        public const string DynamicPortCount = "DynamicPortCount";
        public const string DynamicPortIn = "DynamicPortIn_";

        public const string FloatPort0 = "FloatPort0";

        /// <summary>
        /// Stores runtime port values
        /// </summary>
        [Serializable]
        public class RuntimePort {
            public bool inPort;
            public string name;
            public List<LinkedPort> linkedPorts = new();
            public float[,] map;
            public int intVal;
            public float floatVal;

            public override string ToString() {
                return $"RuntimePort: inPort:{inPort} name:{name} linkedPort:{linkedPorts.Count} ";
            }
        }
        
        [Serializable]
        public class LinkedPort {
            public string name;
            public int nodeId;
        }
        
        /// <summary>
        /// Stores runtime option values
        /// </summary>
        [Serializable]
        public struct RuntimeOption {
            public string name;
            public int intVal;

        }

        public List<RuntimePort> ports = new();
        public int id;
        public bool isDeadEnd;
        public HashSet<int> outNodeIds;
        public List<RuntimeOption> options = new();

        /// <summary>
        /// Graph Toolkit has not implemented changing node name yet
        /// so a debug name is useful for differentiating two node of the same type
        /// </summary>
        /// <returns></returns>
        public virtual string DebugName() {
            return $" {this.GetType()} {id}";
        }
        
        public RuntimePort FindPort(string portName) {
            return ports.Find(p => p.name == portName);
        }

        public virtual void Execute(LayeredNmRuntimeGraph.GraphContext ctx) {
            Debug.LogWarning($"Execute not implemented for {this}");
        }
        
        public void MakeOutNodeIds() {
            outNodeIds = new();
            foreach (var outPort in ports.FindAll(p => !p.inPort)){
                foreach (var linkedPort in outPort.linkedPorts) {
                    outNodeIds.Add(linkedPort.nodeId);
                }
                
            }
        }
        
    }
    
    

    
    
}