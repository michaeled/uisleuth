using System;
using System.Collections.Generic;
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
    public class TestGetNumericProperties : TestBaseReaction
    {
        [Test]
        public void Should_return_property_types_and_names()
        {
            const string requestedType = "System.Guid";

            Tr.Types = new HashSet<UIType>(new[]
            {
                new UIType
                {
                    FullName = requestedType
                }
            });

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

            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("Id"));

            Assert.IsNotNull(property, "Id property should have been found.");
            Assert.AreEqual(property.UIType.FullName, typeof(Guid).FullName);
            Assert.IsNotNull(property.Value, "Value should have been set.");
        }
    }
}
