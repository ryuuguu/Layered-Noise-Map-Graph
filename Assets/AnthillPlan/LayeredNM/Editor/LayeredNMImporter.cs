using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace AnthillPlan.LayeredNM.Editor {
    [ScriptedImporter(1, LayeredNMGraph.AssetExtension)]
    internal class LayeredNMImporter : ScriptedImporter {

        //this is shown in inspector as an example of where it shows in the inspector
#region Runtime Asset Variables
        [ShowInInspector]
        private List<LayeredNmRuntimeNode> _orderedNodes = new();
        private List<RuntimeVariable> _blackboardVariables = new();
        private ResultMapRuntimeNode _rootNode = null;
#endregion

#region mediation data. Runtime variable access to Editor namespace node links 
        private Dictionary<INode, int> _dictNodeToId = new ();
        private Dictionary<int, LayeredNmRuntimeNode> _dictIdToRtNode = new ();
#endregion

#region Const for variable types
        private string _intString; 
        private string _floatString;
        private string _mapString;
#endregion   

#region OnImportAsset Overview of asset creation
        public override void OnImportAsset(AssetImportContext ctx) {
            _intString = typeof(int).ToString();
            _floatString = typeof(float).ToString();
            _mapString = typeof(float[,]).ToString();
            
            var graph = GraphDatabase.LoadGraphForImporter<LayeredNMGraph>(ctx.assetPath);
            if (graph == null) {
                Debug.LogError($"Failed to load Layered Noise Map graph asset: {ctx.assetPath}");
                return;
            }

#region Error Handling
            if (graph.errorMessage.isError) {
                var runtimeErrorAsset = ScriptableObject.CreateInstance<LayeredNmRuntimeGraph>();
                runtimeErrorAsset.errorMessage.isError = graph.errorMessage.isError;
                runtimeErrorAsset.errorMessage.messages = graph.errorMessage.messages;
                foreach (var msg in runtimeErrorAsset.errorMessage.messages) {
                    runtimeErrorAsset.errorMessage.message += msg + "\r\n";
                }
                ctx.AddObjectToAsset("RuntimeAsset", runtimeErrorAsset);
                ctx.SetMainObject(runtimeErrorAsset);
                Debug.Log($"OnImportAsset Error found  ");
                return;
            }
#endregion
            
            MakeBlackboardVariables(graph);
            // runtime Nodes can not have editor references so int Ids are used
            // for port references when making runtime nodes.
            var id = 0;
            foreach (var node in graph.GetNodes()) {
                id++;
                _dictNodeToId[node] = id;
            }
            _rootNode = MakeRuntimeNodes(graph);
            if (_rootNode == null) {
                // It should not be possible to get here because the graph error checker should have set isError
                Debug.LogError($"Failed to compile. No ResultMapNode found in: {ctx.assetPath}");
                return;
            }
            var allNodes = _dictIdToRtNode.Values.ToList();
            foreach (var node in allNodes) {
                node.MakeOutNodeIds();
            }
            
            var prunedNodes =
                TopologicalSort(allNodes, false, false);
            TopologicalSort(prunedNodes, true, true);
            _orderedNodes.Reverse();
            
#region save runtime graph to asset
            var runtimeAsset = ScriptableObject.CreateInstance<LayeredNmRuntimeGraph>();
            runtimeAsset.nodeList = _orderedNodes;
            runtimeAsset.blackboardVariables = _blackboardVariables.ToList();
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
#endregion
        }
#endregion

#region MakeBlackboardVariables
        private void MakeBlackboardVariables(LayeredNMGraph graph) {
            foreach (var variable in graph.GetVariables()) {
                _blackboardVariables.Add( new RuntimeVariable() {
                    name = variable.name,
                    variableKind = variable.variableKind.ToString(), // not implemented yet
                    dataType = variable.dataType.ToString(), // Unity does not serialize Type
                });
            }
        }
#endregion 

#region Make Runtime Node
private ResultMapRuntimeNode MakeRuntimeNodes(LayeredNMGraph graph) {
    ResultMapRuntimeNode result = null;
    foreach (var node in graph.GetNodes()) {
        var id = _dictNodeToId[node];
        if (node is IVariableNode variableNode  ) {
            var rtNode = TranslateNodeModelToRuntimeNode(variableNode, id);
            rtNode.variableName = variableNode.variable.name;
            _dictIdToRtNode[id] = rtNode; 
            continue;
        }
        if (node is LayeredNmNode nmNode) {
            var rtNode = TranslateNodeModelToRuntimeNode(nmNode, id);
            if (rtNode is ResultMapRuntimeNode runtimeNode) {
                result= runtimeNode;
            }
            _dictIdToRtNode[id] = rtNode; 
            continue;
        }
    }
    return result;
}
public VariableRuntimeNode TranslateNodeModelToRuntimeNode(IVariableNode node, int id) {
    var runtimeNode = new VariableRuntimeNode() {
        id = id,
        dataType = node.variable.dataType.ToString(),
    };
    SetPorts(node, runtimeNode);
    return runtimeNode;
}
       
public LayeredNmRuntimeNode TranslateNodeModelToRuntimeNode(LayeredNmNode node, int id) {
    var runtimeNode = node.NewRuntimeNode();
    SetPorts(node, runtimeNode);
    SetOptions(node, runtimeNode);
    runtimeNode.id = id;
    return runtimeNode;
}
#endregion

#region Set option for runtime node
        public void SetOptions(LayeredNmNode node, LayeredNmRuntimeNode runtimeNode) {
            foreach (var option in node.nodeOptions) {
                var runtimeOption = new LayeredNmRuntimeNode.RuntimeOption();
                runtimeOption.name = option.name;
                if (option.dataType == typeof(HeightmapGenerator.HeightmapType)) {
                    option.TryGetValue(out HeightmapGenerator.HeightmapType val);
                    runtimeOption.intVal = (int) val;
                }
                runtimeNode.options.Add(runtimeOption);
            }
        }
#endregion
        
#region Set ports for runtime node
         public void SetPorts(INode node, LayeredNmRuntimeNode runtimeNode) {
            foreach (var inPort in node.GetInputPorts()) {
                var runtimePort = new LayeredNmRuntimeNode.RuntimePort();
                runtimePort.inPort = true;
                runtimePort.name = inPort.name;
                
                var connectedPorts = new List<IPort>();
                inPort.GetConnectedPorts( connectedPorts);
                if (connectedPorts.Count > 0) {
                    foreach (var connectedPort in connectedPorts) {
                        var connectedNode = connectedPort.GetNode();
                        if (connectedNode is IConstantNode) {
                            var constantNode = (IConstantNode) connectedNode;
                            var inPortType = inPort.dataType.ToString();
                            if (inPortType == _intString) {
                                constantNode.TryGetValue(out runtimePort.intVal);
                            } else if(inPortType == _floatString ){
                                constantNode.TryGetValue(out runtimePort.floatVal);
                            } else if (inPortType == _mapString) {
                                constantNode.TryGetValue(out runtimePort.map);
                            }else{
                                Debug.LogError($"inPort name:{inPort.name} datatype not implemented: {inPort.dataType}");
                            }    
                        }
                        else {
                            var linkedPort = new LayeredNmRuntimeNode.LinkedPort();
                            linkedPort.name = connectedPort.name;
                            linkedPort.nodeId = _dictNodeToId[connectedNode];
                            runtimePort.linkedPorts.Add(linkedPort);
                        }
                    }
                }
                else {
                    //Get the default value assigned in the graph editor
                    // chained if else is ugly but switch statements need hard coded strings
                    var inPortType = inPort.dataType.ToString();
                    if (inPortType == _intString) {
                        inPort.TryGetValue(out runtimePort.intVal);
                    }
                    else if (inPortType == _floatString) {
                        inPort.TryGetValue(out runtimePort.floatVal);
                    }
                    else if (inPortType == _mapString) {
                        inPort.TryGetValue(out runtimePort.map);
                    }
                    else {
                        Debug.LogError($"inPort name:{inPort.name} datatype not implemented: {inPort.dataType}");
                    }
                }
                runtimeNode.ports.Add(runtimePort);
            }

            foreach (var outPort in node.GetOutputPorts()) {
                var runtimePort = new LayeredNmRuntimeNode.RuntimePort();
                runtimePort.inPort = false;
                runtimePort.name = outPort.name;
                var connectedPorts = new List<IPort>();
                outPort.GetConnectedPorts( connectedPorts);
                foreach(var connectedPort in connectedPorts){ 
                    var linkedPort = new LayeredNmRuntimeNode.LinkedPort();
                    linkedPort.name = connectedPort.name;
                    linkedPort.nodeId = _dictNodeToId[connectedPort.GetNode()];
                    runtimePort.linkedPorts.Add(linkedPort);
                }
                runtimeNode.ports.Add(runtimePort);
            }
        }
#endregion  

#region MakeOrderedNodes

public List<LayeredNmRuntimeNode> TopologicalSort(List<LayeredNmRuntimeNode> digraph,
    bool includeRoot, bool saveOrder) {
    _orderedNodes.Clear();
    var result = digraph.ToList();
    var zeros = 
        result.FindAll(node => !node.outNodeIds.Any());
    if (!includeRoot) {
        zeros.Remove(_rootNode);
    }
    while (zeros.Any() ) {
        if (saveOrder) {
            _orderedNodes.AddRange(zeros);
        }
        result.RemoveAll(node =>
            zeros.Any(z => z.id == node.id));
        foreach (var node in result) {
            node.outNodeIds.RemoveWhere(id =>
                zeros.Any(z => z.id == id));
        }
        zeros = result.FindAll(node => !node.outNodeIds.Any());
        if (!includeRoot) {
            zeros.Remove(_rootNode);
        }
    }
    return result;
}
#endregion

    }
}
