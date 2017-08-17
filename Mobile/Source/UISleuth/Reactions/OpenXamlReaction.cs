using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class OpenXamlReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var r = ctx.Get<OpenXamlRequest>();
            if (string.IsNullOrWhiteSpace(r?.Xaml)) return;

            //App.ConnectionPage?.PushNewPage(r.Xaml);

            ctx.SetResponse<OpenXamlResponse>(res =>
            {
                res.FileName = r.FileName;
                res.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}