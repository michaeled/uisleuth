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
    public class TestGetXamarinStructs : TestBaseReaction
    {
        [Test]
        public void Should_return_LayoutOption_property_and_possible_values()
        {
            Tr.SetTypes(typeof(LayoutOptions));
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());

            var label = new Label();
            var page = new ContentPage { Content = label };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new [] {"HorizontalOptions"};
                req.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<ObjectResponse>();
            var p = res.Property;

            Assert.AreEqual(p.PropertyName, "HorizontalOptions");
            Assert.IsTrue(p.UIType.Descriptor.HasFlag(UIPropertyDescriptors.ValueType | UIPropertyDescriptors.Literals));
            Assert.AreEqual(8, p.UIType.PossibleValues.Length);
            Assert.IsAssignableFrom<UIProperty[]>(p.Value);

            var alignmentProp = (p.Value as UIProperty[])?[0];
            Assert.AreEqual(alignmentProp?.Value, "Fill");
            CollectionAssert.IsNotEmpty(alignmentProp?.UIType.PossibleValues);
        }


        [Test]
        public void Should_return_public_properties_of_Bounds_type()
        {
            Tr.SetTypes(typeof(Rectangle));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new []{"Bounds"};
                req.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<ObjectResponse>();

            var prop = res.Property;
            Assert.AreEqual(prop.PropertyName, "Bounds");

            Assert.IsAssignableFrom<UIProperty[]>(prop.Value);

            var boundProps = (UIProperty[]) prop.Value;
            var xProp = boundProps.FirstOrDefault(b => b.PropertyName == "X");

            Assert.IsNotNull(xProp);
            Assert.AreEqual(xProp.UIType.FullName, typeof (double).FullName);
        }


        [Test]
        public void Private_setters_should_return_struct_as_readonly()
        {
            Tr.SetTypes(typeof(Rectangle));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new []{"Bounds"};
                req.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);
            var res = ctx.Get<ObjectResponse>();

            var prop = res.Property;
            Assert.AreEqual(prop.PropertyName, "Bounds");

            Assert.IsFalse(prop.CanWrite);
            Assert.IsTrue(prop.UIType.Descriptor.HasFlag(UIPropertyDescriptors.ValueType));
        }


        [Test]
        public void Color_test()
        {
            Tr.SetTypes(typeof(Color));

            var label = new Label
            {
                BackgroundColor = Color.Red
            };

            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = label.Id.ToString();
                req.IncludeValues = true;
            });

            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var p = res.Properties.First(f => f.PropertyName == "BackgroundColor");
        }
    }
}
