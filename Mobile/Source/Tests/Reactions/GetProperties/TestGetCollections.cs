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
    public class TestGetCollectionItems : TestBaseReaction
    {
        [Test]
        public void Should_return_collection_values()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());

            Tr.SetTypes(typeof (GridLength),
                typeof (RowDefinitionCollection),
                typeof (ColumnDefinitionCollection));

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = new GridLength (2)}  
                },

                RowDefinitions = new RowDefinitionCollection()
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(r =>
            {
                r.Path = new[] { "ColumnDefinitions[0]"};
                r.WidgetId = grid.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);

            var resp = ctx.Get<ObjectResponse>();
            var val = resp.Property.Value as ICollection<UIProperty>;

            Assert.AreEqual(0, resp.Property.ItemIndex);
            CollectionAssert.IsNotEmpty(val);

            var v = val?.ElementAt(0);

            // ReSharper disable once PossibleNullReferenceException
            Assert.IsNull(v.Value);
            Assert.AreEqual("Width", v.PropertyName);
            Assert.AreEqual("ColumnDefinitions[0]", v.Path[0]);
            Assert.AreEqual("Width", v.Path[1]);
        }


        [Test]
        public void Test_accessing_out_of_bound_element()
        {
            Tr.SetTypes(typeof(RowDefinitionCollection),
                typeof(ColumnDefinitionCollection));

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = new GridLength (2)}
                },

                RowDefinitions = new RowDefinitionCollection()
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(r =>
            {
                r.Path = new [] {"ColumnDefinitions[1]"};
                r.WidgetId = grid.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);
            Assert.IsNull(ctx.Response);
        }


        [Test]
        public void Return_object_properties_from_a_collection_property_of_a_view()
        {
            var path = new[] { "ColumnDefinitions[0]", "Width" };
            Tr.SetTypes(typeof (GridLength), typeof (ColumnDefinitionCollection));

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = new GridLength (2)}
                },

                RowDefinitions = new RowDefinitionCollection()
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<GetObjectRequest>(r =>
            {
                r.Path = path;
                r.WidgetId = grid.Id.ToString();
            });

            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<ObjectResponse>();
            var v = res.Property.Value as ICollection<UIProperty>;

            Assert.IsNotNull(v);
            CollectionAssert.IsNotEmpty(v);
            Assert.AreEqual(path, res.Property.Path);

            var widthProp = v.First(i => i.PropertyName == "Value");

            Assert.AreEqual("2", widthProp.Value);
            Assert.AreEqual(path.Union(new[] {"Value"}), widthProp.Path);
        }
    }
}