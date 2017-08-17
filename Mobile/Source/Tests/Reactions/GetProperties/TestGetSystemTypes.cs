using System;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Tests.Reactions.SetProperties;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.GetProperties
{
    public class TestGetSystemTypes : TestBaseReaction
    {
        [Test]
        public void Should_return_Guid_as_string()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof (Guid));
        
            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var prop = res.Properties[0];

            Assert.IsFalse(prop.UIType.IsNullable);
            Assert.IsNotNull(prop.Value);
        }


        [Test]
        public void Should_get_nint()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(Guid), typeof(int?));

            var page = new PageWithPrimitives();
            var ctx = new UIMessageContext();

            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
            });

            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var type = res.Properties[0].UIType;

            Assert.IsTrue(type.IsNullable);
            Assert.AreEqual("Int32?", type.ShortName);
        }


        [Test]
        public void Should_get_nchar()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(Guid), typeof(char?));

            var page = new PageWithPrimitives();
            var ctx = new UIMessageContext();

            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
            });

            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var type = res.Properties[0].UIType;

            Assert.IsTrue(type.IsNullable);
            Assert.AreEqual("Char?", type.ShortName);
        }
    }
}
