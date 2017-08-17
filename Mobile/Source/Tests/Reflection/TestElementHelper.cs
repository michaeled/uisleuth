using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;

namespace UISleuth.Tests.Reflection
{
    [TestFixture]
    public class TestElementHelper
    {
        [Test]
        public void Should_return_Grids_attached_properties()
        {
            var grid = new Grid();
            var attachedProps = ElementHelper.GetAttachedProperties(grid).ToArray();

            var rowProp = attachedProps.Any(p => p.Field.Name == "RowProperty");
            var colProp = attachedProps.Any(p => p.Field.Name == "ColumnProperty");

            Assert.IsNotNull(rowProp, "Row");
            Assert.IsNotNull(colProp, "Col");
            Assert.AreEqual(4, attachedProps.Length);
        }


        [Test]
        public void Should_return_ContentPage_as_parent()
        {
            var label = new Label();

            var page = new ContentPage
            {
                Content = label
            };

            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[label.Id];
            var cp = ElementHelper.GetParents(pair.UIWidget);

            Assert.AreEqual(root.Id, cp[0].Id);
        }


        [Test]
        public void Should_return_all_parents_for_nested_stacks()
        {
            var stack = new StackLayout();
            StackLayout last = null;

            for (var i = 0; i < 5; i++)
            {
                var tmp = new StackLayout();

                if (last == null)
                {
                    stack.Children.Add(tmp);
                }
                else
                {
                    last.Children.Add(tmp);
                }

                last = tmp;
            }

            var page = new ContentPage
            {
                Content = stack
            };

            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[last.Id];
            var cp = ElementHelper.GetParents(pair.UIWidget);

            Assert.AreEqual(6, cp.Length);
        }
    }
}
