using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions
{
    public class TestContentView : ContentView
    {
        public bool Exists { get; set; }    
    }

    public class TestGridLayout : Grid
    {
        public bool Exists { get; set; }
    }

    [TestFixture]
    public class TestGetVisualElementsReaction : TestBaseReaction
    {
        [Test]
        public void Should_get_all_view_types_and()
        {
            var ctx = new UIMessageContext();
            ctx.SetRequest<GetVisualElementsRequest>();

            Reaction.Register<GetVisualElementsRequest, GetVisualElementsReaction>();
            Reaction.Execute(ctx);

            var r = ctx.Response as GetVisualElementsResponse;

            Assert.IsNotNull(r);

            CollectionAssert.Contains(r.Views, "Xamarin.Forms.Button");
            CollectionAssert.Contains(r.Views, "Xamarin.Forms.Editor");
            CollectionAssert.DoesNotContain(r.Views, "Xamarin.Forms.OpenGLView");
            CollectionAssert.Contains(r.Layouts, "Xamarin.Forms.Grid");
            CollectionAssert.Contains(r.Others, "UISleuth.Tests.Reactions.TestContentView");
            CollectionAssert.Contains(r.Others, "UISleuth.Tests.Reactions.TestGridLayout");
        }
    }
}
