using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class DesktopReadyReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var req = ctx.Get<DesktopReady>();
            if (req == null) return;

            ctx.SetResponse<MobileReady>(res =>
            {
                // ignored
            });
        }
    }
}