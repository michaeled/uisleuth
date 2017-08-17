using System;
using System.Linq;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.GetProperties
{
    [TestFixture]
    public class TestGetAttachedProperties : TestBaseReaction
    {
        [Test]
        public void Should_return_parents_attached_properties()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(BindableProperty));

            var label1 = new Label();
            var label2 = new Label();
            var grid = new Grid();

            grid.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition {Width = new GridLength(10, GridUnitType.Absolute)},
                new ColumnDefinition {Width = new GridLength(20, GridUnitType.Absolute)},
            };

            grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition {Height = new GridLength(30, GridUnitType.Absolute)},
                new RowDefinition {Height = new GridLength(40, GridUnitType.Absolute)},
            };

            grid.Children.Add(label1);
            grid.Children.Add(label2);

            Grid.SetRow(label1, 5);
            Grid.SetColumn(label1, 6);

            var page = new ContentPage
            {
                Content = grid
            };

            var id = label1.Id.ToString();
            var ctx = new UIMessageContext();
            ctx.SetRequest<GetAttachedPropertiesRequest>(req =>
            {
                req.WidgetId = id;
            });

            InspectorReaction.Register<GetAttachedPropertiesRequest, GetAttachedPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetAttachedPropertiesResponse>();
            var ap1 = res.Widget.AttachedProperties.First(p => p.Path[0] == "RowProperty");
            var ap2 = res.Widget.AttachedProperties.First(p => p.Path[0] == "ColumnProperty");

            Assert.AreEqual(4, res.Widget.AttachedProperties.Count);
            Assert.AreEqual(5, ap1.Value);
            Assert.AreEqual(6, ap2.Value);
        }


        [Test]
        public void Should_not_repeat_attached_properties_when_theyre_nested()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(BindableProperty));

            var label = new Label();
            var first = new Grid();
            var second = new Grid();
            var id = label.Id.ToString();

            first.Children.Add(second);
            second.Children.Add(label);
            Grid.SetRow(label, 5);

            var page = new ContentPage
            {
                Content = first
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetAttachedPropertiesRequest>(req =>
            {
                req.WidgetId = id;
            });

            InspectorReaction.Register<GetAttachedPropertiesRequest, GetAttachedPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetAttachedPropertiesResponse>();
            var row = res.Widget.AttachedProperties.First(f => f.XamlPropertyName == "Grid.Row");

            Assert.AreEqual(5, row.Value);
        }
    }
}
