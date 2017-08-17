using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UISleuth.Widgets;

namespace UISleuth.Tests.Widgets
{
    [TestFixture]
    public class TestUIWidgetExtensions
    {
        [Test]
        public void Should_return_one_node()
        {
            var w1 = new UIWidget();
            var nodes = w1.GetNodeAndDescendants();
            Assert.IsTrue(nodes.Count() == 1);
        }


        [Test]
        public void Should_return_all_nodes()
        {
            var w1 = new UIWidget {Name = "1"};
            var w2 = new UIWidget {Name = "2"};
            var w3 = new UIWidget {Name = "3"};
            var w4 = new UIWidget {Name = "4"};
            var w5 = new UIWidget {Name = "5"};

            w1.Children = new List<UIWidget>
            {
                w2,
                w3
            };

            w3.Children = new List<UIWidget>
            {
                w4
            };

            w4.Children = new List<UIWidget> {w5};
            var nodes = w1.GetNodeAndDescendants();

            CollectionAssert.AreEquivalent(nodes, new[] {w1,w2,w3,w4,w5});
        }
    }
}
