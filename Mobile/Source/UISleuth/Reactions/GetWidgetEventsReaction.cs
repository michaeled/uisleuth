using System.Linq;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Widgets;

namespace UISleuth.Reactions
{
    internal class GetWidgetEventsReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<GetWidgetEventsRequest>();
            if (request == null) return;

            var pair = Surface[request.WidgetId];

            var events = pair?
                .VisualElement?
                .GetType()
                .GetPublicEvents()
                .Select(UIEvent.Create)
                .ToArray();

            if (events == null) return;

            ctx.SetResponse<GetWidgetEventsResponse>(r =>
            {
                r.Events = events;
                r.WidgetId = request.WidgetId;
            });
        }
    }
}