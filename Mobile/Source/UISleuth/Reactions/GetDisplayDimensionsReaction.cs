using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class GetDisplayDimensionsReaction : InspectorReaction
    {
		protected override void OnExecute(UIMessageContext ctx)
		{
			var request = ctx.Get<GetDisplayDimensionsRequest>();
			if (request == null) return;

			var dimensionService = InspectorContainer.Current.Resolve<IDisplayDimensions>();
			var dimensions = new DisplayDimensions();

			Thread.Invoke(() =>
			{
				dimensions = dimensionService.GetDimensions();
			});
 
            ctx.SetResponse<GetDisplayDimensionsResponse>(r =>
            {
                r.Density = dimensions.Density;
                r.Height = dimensions.Height;
                r.Width = dimensions.Width;
                r.NavigationBarHeight = dimensions.NavigationBarHeight;
                r.StatusBarHeight = dimensions.StatusBarHeight;
            });
        }
    }
}