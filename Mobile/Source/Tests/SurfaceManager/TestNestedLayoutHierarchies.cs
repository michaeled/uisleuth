using System.Linq;
using NUnit.Framework;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Tests.SurfaceManager
{
    [TestFixture]
    public class TestNestedLayoutHierarchies
    {
        [Test]
        public void Page_without_content()
        {
            var mgr = new DefaultSurfaceManager();
            var page = new ContentPage
            {
                Title = "No Content Example",
            };

            var root = mgr.SetInspectorSurface(page);

            Assert.IsNotNull(root, "Page should be in result.");
            Assert.AreEqual(page.Title, root.Name, "Title wrong.");
            Assert.AreEqual(nameof(ContentPage), root.Type, "Type wrong.");
            Assert.IsTrue(root.Children == null || root.Children.Count == 0, "No children should be returnd.");
        }


        [Test]
        public void Page_with_stacklayout()
        {
            var page = new ContentPage
            {
                Title = "Stacklayout Example",
                Content = new StackLayout
                {
                    Children =
                    {
                        new StackLayout
                        {
                            Children =
                            {
                                new Entry { Placeholder = "placeholder "},
                                new BoxView { BackgroundColor = Color.Green}
                            }
                        }
                    }
                }
            };

            var builder = new DefaultSurfaceManager();

            var result = builder.SetInspectorSurface(page);
            var flattened = result.GetNodeAndDescendants().ToArray();

            Assert.IsTrue(flattened.Length == 5, "There should have been 5 widgets created.");

            Assert.AreEqual(result.Type, nameof(ContentPage), "1st widget");
            Assert.AreEqual(result.CanDelete, false, "1st widget");

            Assert.AreEqual(result.Children[0].Type, nameof(StackLayout), "2nd widget");
            Assert.AreEqual(result.Children[0].CanDelete, true, "2nd widget");

            Assert.AreEqual(result.Children[0].Children[0].Type, nameof(StackLayout), "3rd widget");
            Assert.AreEqual(result.Children[0].Children[0].CanDelete, true, "3rd widget");

            Assert.AreEqual(result.Children[0].Children[0].Children[0].Type, nameof(Entry), "4th widget");
            Assert.AreEqual(result.Children[0].Children[0].Children[0].CanDelete, true, "4th widget");

            Assert.AreEqual(result.Children[0].Children[0].Children[1].Type, nameof(BoxView), "5th widget");
            Assert.AreEqual(result.Children[0].Children[0].Children[1].CanDelete, true, "5th widget");
        }


        [Test]
        public void Should_return_view_when_using_indexer()
        {
            var entry = new Entry();
            var button = new Button();

            // start of page
            var page = new ContentPage
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        new StackLayout
                        {
                            Children =
                            {
                                new ContentView
                                {
                                    Content = new StackLayout
                                    {
                                        Children =
                                        {
                                            entry,
                                            button
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var entryFound = mgr[entry.Id.ToString()];
            var buttonFound = mgr[button.Id.ToString()];

            Assert.AreSame(entry, entryFound.VisualElement, "Entry is incorrect.");
            Assert.AreSame(button, buttonFound.VisualElement, "Button is incorrect.");
            Assert.IsNull(mgr["test"], "Should return null when id not found.");
        }
    }
}
