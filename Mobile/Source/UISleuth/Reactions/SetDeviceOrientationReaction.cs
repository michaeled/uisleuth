using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class SetDeviceOrientationReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<SetDeviceOrientationRequest>();
            if (request == null) return;

            DisplayDimensions dims = null;
            var orientationSvc = InspectorContainer.Current.Resolve<IDeviceOrientation>();
            var dimSvc = InspectorContainer.Current.Resolve<IDisplayDimensions>();

            Thread.Invoke(() =>
            {
                orientationSvc.SetOrientation(request.Orientation);
                dims = dimSvc.GetDimensions();
            });

            ctx.SetResponse<SetDeviceOrientationResponse>(r =>
            {
                r.Height = dims.Height;
                r.Width = dims.Width;
            });
        }
    }
}