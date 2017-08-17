using System.Linq;
using NUnit.Framework;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions
{
    [TestFixture]
    public class TestDesignerReactions : TestBaseReaction
    {
        [Test]
        public void Only_attach_method_can_add_widgets_to_hierarchy()
        {
            var traverser = new DefaultSurfaceManager();

            var layout = new StackLayout
            {
                Children =
                {
                    new Label {Text = "For good measure..."}
                }
            };

            var page = new ContentPage
            {
                Title = "Testing that a newly added UIWidget can be found in the hierarchy",
                Content = layout
            };

            var addLater = new Label { Text = "Added after hierarchy built." };

            // Act
            var root = traverser.SetInspectorSurface(page);
            var firstWidgets = root.GetNodeAndDescendants().ToArray();

            Assert.IsNull(firstWidgets.FirstOrDefault(w => w.Id == addLater.Id.ToString()), "Recurisve search found label.");

            // Act
            layout.Children.Add(addLater);

            // The root element is fixed. Its the page.
            var secondWidgets = root.GetNodeAndDescendants().ToArray();
            Assert.IsNull(secondWidgets.FirstOrDefault(w => w.Id == addLater.Id.ToString()), "Recurisve search found label.");
        }
    }
}
