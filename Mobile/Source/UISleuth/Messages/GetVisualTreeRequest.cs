using UISleuth.Reactions;
using UISleuth.Widgets;

namespace UISleuth.Messages
{
    /// <summary>
    /// Signal's that the client has requested a tree structure of the UI elements on the surface.
    /// This "document outline" is created by a <see cref="SurfaceManager"/>.
    /// </summary>
    internal class GetVisualTreeRequest : Request {}

    /// <summary>
    /// Sent in response to a <see cref="GetVisualTreeRequest"/>.
    /// </summary>
    internal class GetVisualTreeResponse : Response
    {
        /// <summary>
        /// The visual tree's top-most widget.
        /// </summary>
        public UIWidget Root { get; set; }
    }
}
