using System;
using System.Linq;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.GetProperties
{
    [TestFixture]
    public class TestGetEnumProperties : TestBaseReaction
    {
        [Test]
        public void Should_return_property_types_and_names()
        {
            Tr.SetTypes(typeof (Enum));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            Dr.Add(typeof (Enum), new EnumGenerator());
            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("HorizontalTextAlignment"));
            Assert.IsNotNull(property, "Property not found.");

            Assert.AreEqual(property.Value, TextAlignment.Start.ToString());
            Assert.AreEqual(property.UIType.Descriptor, UIPropertyDescriptors.Literals);

            // if this occurs, types will not be selected correctly by the client
            Assert.IsFalse(property.UIType.Descriptor.HasFlag(UIPropertyDescriptors.ValueType));
            CollectionAssert.IsNotEmpty(property.UIType.PossibleValues);
        }


        [Test]
        public void Should_return_multiple_values()
        {
            Tr.SetTypes(typeof(Enum));

            var label = new Label
            {
                FontAttributes = FontAttributes.Bold | FontAttributes.Italic
            };

            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            Dr.Add(typeof(Enum), new EnumGenerator());
            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("FontAttributes"));
            Assert.IsNotNull(property, "Property not found.");

            Assert.AreEqual(property.Value, TextAlignment.Start.ToString());
            Assert.AreEqual(property.UIType.Descriptor, UIPropertyDescriptors.Literals);

            // if this occurs, types will not be selected correctly by the client
            Assert.IsFalse(property.UIType.Descriptor.HasFlag(UIPropertyDescriptors.ValueType));
            CollectionAssert.IsNotEmpty(property.UIType.PossibleValues);
        }
    }
}