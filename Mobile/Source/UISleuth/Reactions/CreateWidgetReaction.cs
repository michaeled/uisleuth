using System;
using UISleuth.Messages;
using UISleuth.Networking;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    /// <summary>
    /// Implicitly assumes that the view has a default constructor w/o params.
    /// </summary>
    internal class CreateWidgetReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<CreateWidgetRequest>();
            if (request == null) return;

            var viewType = TypeFinder.Find(request.TypeName);

            if (viewType == null)
            {
                return;
            }

            var view = Activator.CreateInstance(viewType) as View;
            if (view == null)
            {
                return;
            }

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
