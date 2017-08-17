using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class GetBindingContextReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var req = ctx.Get<GetBindingContextRequest>();
            if (req == null) return;

            var pair = Surface[req.WidgetId];
            if (pair == null) return;
            if (pair.UIWidget.IsDatabound == false) return;

            var data = BindingContextSerializer.SerializeObject(pair.VisualElement.BindingContext, 2);

            ctx.SetResponse<GetBindingContextResponse>(res =>
            {
                res.WidgetId = req.WidgetId;
                res.Data = data;
            });
        }
    }
}