using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.Views
{
    [TestFixture]
    public class TestCreateButtonReaction : TestDesignerReactions
    {
        [Test]
        public void Attach_button_to_grid_and_not_set_row_and_col()
        {
            var grid = new Grid();
            var page = new ContentPage
            {
                Title = "Testing create button action",
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<CreateWidgetRequest>(r =>
            {
                r.ParentId = grid.Id.ToString();
                r.TypeName = "Xamarin.Forms.Button";
            });

            InspectorReaction.Register<CreateWidgetRequest, CreateWidgetReaction>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<CreateWidgetResponse>();

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.Parent.Type == nameof(Grid), "Expected type to be grid.");
            Assert.IsTrue(response.Widget.Type == nameof(Button), "Expected type to be button.");
            Assert.IsTrue(response.Parent.Children[0].Type == nameof(Button), "Expected child to be button.");
        }
    }
}