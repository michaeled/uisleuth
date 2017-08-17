using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class GestureReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<GestureRequest>();
            if (request == null) return;

            var toucher = InspectorContainer.Current.Resolve<ITouchEvent>();

            Thread.Invoke(() =>
            {
                toucher.Gesture(request.Path, request.Duration);
            });
        }
    }
}
