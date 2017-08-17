using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Widgets;

namespace UISleuth.Reactions
{
    /// <summary>
    /// An action that will return all <see cref="UIWidget"/>s on the design surface.
    /// </summary>
    internal class GetVisualTreeReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var r = UIMessage.Create<GetVisualTreeResponse>();
            r.Root = Surface.Root;
            ctx.Response = r;
        }
    }
}