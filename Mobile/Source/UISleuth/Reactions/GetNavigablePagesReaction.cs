using System.Collections.Generic;
using UISleuth.Messages;
using UISleuth.Networking;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    internal class GetNavigablePagesReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<GetNavigablePagesRequest>();
            if (request == null) return;

            var mainPage = Application.Current.MainPage as MasterDetailPage;
            if (mainPage == null) return;

            var pages = new List<NavigablePage>();

            if (mainPage.Master != null)
            {
                pages.Add(new NavigablePage
                {
                    Id = mainPage.Master.Id.ToString(),
                    Type = NavigablePageTypes.MasterPage
                });
            }

            if (mainPage.Detail != null)
            {
                pages.Add(new NavigablePage
                {
                    Id = mainPage.Detail.Id.ToString(),
                    Type = NavigablePageTypes.DetailPage
                });
            }

            ctx.SetResponse<GetNavigablePagesResponse>(r =>
            {
                r.Pages = pages.ToArray();
            });
        }
    }
}
