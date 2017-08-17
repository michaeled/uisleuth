using UISleuth.Messages;
using UISleuth.Networking;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    internal class CreateStackLayoutReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<CreateStackLayoutRequest>();
            if (request == null) return;

            var orientation = StackOrientation.Vertical;

            if (string.IsNullOrWhiteSpace(request.Orientation) || request.Orientation.ToLower() == "vertical")
            {
                orientation = StackOrientation.Vertical;
            }
            else if (request.Orientation.ToLower() == "horizontal")
            {
                orientation = StackOrientation.Horizontal;
            }

            var view = new StackLayout
            {
                Orientation = orientation,
                Spacing = request.Spacing
            };

            var target = Surface[request.ParentId];
            if (target == null) return;

            var attached = false;

            Thread.Invoke(() =>
            {
                attached = Surface.SetParent(view, target);
            });

            if (!attached) return;

            var pair = Surface[view.Id];

            ctx.SetResponse<CreateWidgetResponse>(r =>
            {
                r.Widget = pair.UIWidget;
                r.Parent = target.UIWidget;
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
