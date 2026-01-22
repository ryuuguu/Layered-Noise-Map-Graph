using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Unity.GraphToolkit.Editor;


namespace AnthillPlan.LayeredNM.Editor {
    [Graph(AssetExtension)]
    [Serializable]
    class LayeredNMGraph : Graph {
        private const string GraphName = "Layered Noise Map";
        public const string AssetExtension = "layerednm";
        public ErrorMessage errorMessage = new();

        [MenuItem("Assets/Create/Graph Toolkit/Layered Noise Map Graph")]
        static void CreateAssetFile() {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<LayeredNMGraph>(GraphName);
        }
#region OnGraphChanged
        /// <summary>
        /// Called when the graph changes.
        /// </summary>
        /// <param name="infos">The GraphLogger object to which errors and warnings are added.</param>
        /// <remarks>
        /// This method is triggered whenever the graph is modified. It calls `CheckGraphErrors` to validate the graph
        /// and report any issues.
        /// </remarks>
        public override void OnGraphChanged(GraphLogger infos) {
            base.OnGraphChanged(infos);
            CheckGraphErrors(infos);
        }
#endregion

#region CheckGraphErrors
        /// <summary>
        /// Checks the graph for errors and warnings and adds them to the result object.
        /// </summary>
        /// <param name="infos">Object implementing <see cref="GraphLogger"/> interface and containing
        /// collected errors and warnings</param>
        /// <remarks>Errors and warnings are reported by adding them to the GraphLogger object,
        /// which is the default reporting mechanism for a Graph Toolkit tool. </remarks>
        void CheckGraphErrors(GraphLogger infos) {
            errorMessage.messages = new();
            errorMessage.isError = false;
            ResultMapNodeErrors(infos);
            CycleErrors(infos);
        }
#endregion

#region ResultNodeMap Errors
        private void ResultMapNodeErrors(GraphLogger infos) {
            var resultMapNodes = GetNodes().OfType<ResultMapNode>().ToArray();

            switch (resultMapNodes.Length) {
                case 0:
                    errorMessage.isError = true;
                    var msg = "Add a ResultMapNode in your LayerNoise graph.";
                    errorMessage.messages.Add(msg);
                    infos.LogError(msg, this);
                    break;
                case 1:
                    break;
                case >= 1:
                    errorMessage.isError = true;
                    errorMessage.messages.Add($"There are {resultMapNodes.Length} ResultMapNodes." +
                                              $" Only one per graph is supported.");
                    foreach (var resultMapNode in resultMapNodes) {
                        infos.LogError($"LayeredNM Graph only supports one ResultMapNode per graph." +
                                         $" Delete extra ResultMapNodes .", resultMapNode);
                    }
                    break;
            }
        }
        
#endregion        

#region Cycle Check Error
        HashSet<INode> _visitedNodes = new();
        HashSet<INode> _recursiveCheckedNodes = new();
        HashSet<INode> _cycleNodes = new();
#region Cycle process errors        
        /// <summary>
        /// find cycles and add error messages for them
        /// also log warnings in GraphLogger infos
        /// </summary>
        /// <param name="infos"></param>
        private void CycleErrors(GraphLogger infos) {
            _cycleNodes = new HashSet<INode>();
            FindCycles();
            foreach (var node in _cycleNodes) {
                errorMessage.isError = true;
                var msg = $"{node} is part of a loop. Loops are not supported ";
                errorMessage.messages.Add(msg);
                infos.LogError(msg, node);
            }
        }
#endregion

#region GeeksForGeeks cycle check
        /// <summary>
        /// If there is a cycle find at least one node
        /// continues to check for more nodes and cycles
        /// but is not guaranteed to find all nodes in all cycles
        /// adds nodes to cycleNodes when found
        /// based on geeksforgeeks.org code https://www.geeksforgeeks.org/dsa/detect-cycle-in-a-graph/
        /// </summary>
        private void FindCycles() {
            foreach (var startNode in GetNodes()) {
                _visitedNodes = new HashSet<INode>();
                _recursiveCheckedNodes = new HashSet<INode>();
                IsCyclicUtil(startNode);
            }
        }
#endregion

#region CycleCheck Algo
        private bool IsCyclicUtil( INode startNode) {
            if (_recursiveCheckedNodes.Contains(startNode)) {
                _cycleNodes.Add(startNode);
                return true;
            }
            if (_visitedNodes.Contains(startNode)) {
                return false;
            }
            _visitedNodes.Add(startNode) ;
            _recursiveCheckedNodes.Add(startNode);

            var localOutEdgeChecked = new HashSet<INode>();
            foreach (var outPort in startNode.GetOutputPorts()) {
                var connectedPorts = new List<IPort>();
                outPort.GetConnectedPorts(connectedPorts);
                foreach (var connectedPort in connectedPorts) { 
                    var connectedNode = connectedPort.GetNode();
                    // the graph is not a properly formated digraph 
                    // since it can have multiple edges between two vertices in the same direction
                    // so only process the first such edge 
                    if(localOutEdgeChecked.Contains(connectedNode)) continue ;
                    localOutEdgeChecked.Add(connectedNode);
                    if (IsCyclicUtil(connectedNode))
                        return true;
                }
            }
            _recursiveCheckedNodes.Remove(startNode);
            return false;
        }
#endregion
#endregion Cycle Check
    }
}

