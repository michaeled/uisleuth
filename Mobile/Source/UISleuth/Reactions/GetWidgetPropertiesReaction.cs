using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    internal class GetWidgetPropertiesReaction : GetPropertiesReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetWidgetPropertiesRequest>();
            if (string.IsNullOrWhiteSpace(req?.WidgetId)) return;

            var props = GetUIProperties(req.WidgetId, req.IncludeValues) ?? new UIProperty[] { };

            foreach (var prop in props)
            {
                var isource = prop.Value as UIImageSource;
                if (isource != null)
                {
                    prop.Value = isource.FileName;
                    continue;
                }

                var wvsource = prop.Value as UrlWebViewSource;
                if (wvsource != null)
                {
                    prop.Value = wvsource.Url;
                    continue;
                }

                var hvsource = prop.Value as HtmlWebViewSource;
                if (hvsource != null)
                {
                    prop.Value = hvsource.Html;
                    continue;
                }
            }

            var pair = Surface[req.WidgetId];

            ctx.SetResponse<GetWidgetPropertiesResponse>(res =>
            {
                res.Widget = pair.UIWidget;
                res.Properties = props;
            });
        }
    }
}