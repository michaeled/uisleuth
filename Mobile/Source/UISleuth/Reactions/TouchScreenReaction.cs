using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class TouchScreenReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<TouchScreenRequest>();
            if (request == null) return;

            var toucher = InspectorContainer.Current.Resolve<ITouchEvent>();

            Thread.Invoke(() =>
            {
                toucher.Down(request.X, request.Y, request.Duration);
            });
        }
    }
}