using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Visual Novel Director Base Node model.
    /// </summary>
    /// <remarks> It is best practice to group all the nodes of a tool under a base node. It improves organization,
    /// scalability, maintenance and debugging.</remarks>
    [Serializable]
    internal abstract class VisualNovelNode : Node
    {
        public const string EXECUTION_PORT_DEFAULT_NAME = "ExecutionPort";
        public float test;
        /// <summary>
        /// Defines common input and output execution ports for all nodes in the Visual Novel Director tool.
        /// </summary>
        /// <param name="scope">The scope to define the node.</param>
        protected void AddInputOutputExecutionPorts(IPortDefinitionContext context) {
            test = 9f;
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
