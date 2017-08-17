using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Reactions;

namespace UISleuth.Workflow
{
    /// <summary>
    /// Responsible for dispatching client messages to the appropriate <see cref="Reaction"/>.
    /// </summary>
    internal abstract class InspectorWorkflow
    {
        /// <summary>
        /// Queue a request to be executed by the designer. It does not have to be a request to get or set visual element data.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Queue(string message);


        /// <summary>
        /// Start the workflow.
        /// </summary>
        public abstract void Start(IUIMessageFinder messageFinder, InspectorSocket socket);


        /// <summary>
        /// Shutdown the workflow.
        /// </summary>
        public abstract void Shutdown();
    }
}
