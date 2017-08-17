using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class DeleteWidgetReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<DeleteWidgetRequest>();
            if (request == null) return;

            var pair = Surface[request.WidgetId];
            if (pair == null) return;

            if (!pair.UIWidget.CanDelete)
            {
                return;
            }

            Thread.Invoke(() =>
            {
                Surface.Remove(request.WidgetId);
            });

            ctx.SetResponse<OkResponse>(r =>
            {
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
