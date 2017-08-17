using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class ScreenShotReaction : Reaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<ScreenShotRequest>();
            if (request == null) return;

            var screenshot = InspectorContainer.Current.Resolve<IScreenShot>();
            byte[] capture = null;

            Thread.Invoke(() =>
            {
                capture = screenshot.Capture();
            });

            ctx.SetResponse<ScreenShotResponse>(r =>
            {
                r.Capture = capture;
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
