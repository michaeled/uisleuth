using System;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.SetProperties
{
    public class TestEditCollectionReaction : TestBaseReaction
    {
        [Test]
        public void Add_nongeneric_collection()
        {
            var grid = new Grid();
            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<EditCollectionRequest>(r =>
            {
                r.Type = EditCollectionType.Add;
                r.Path = new[] {"ColumnDefinitions"};
                r.WidgetId = grid.Id.ToString();
            });

            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());

            Assert.IsEmpty(grid.ColumnDefinitions);

            InspectorReaction.Register<EditCollectionRequest, EditCollectionReaction>(page);
            Reaction.Execute(ctx);

            Assert.IsNotEmpty(grid.ColumnDefinitions);

            var response = ctx.Get<EditCollectionResponse>();
            Assert.IsTrue(response.Successful);
            Assert.AreEqual(EditCollectionType.Add, response.Type);
        }


        [Test]
        public void Delete_nongeneric_collection()
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition()
                }
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<EditCollectionRequest>(r =>
            {
                r.Type = EditCollectionType.Delete;
                r.Path = new [] {"ColumnDefinitions[0]"};
                r.WidgetId = grid.Id.ToString();
            });

            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());

            Assert.IsNotEmpty(grid.ColumnDefinitions);

            InspectorReaction.Register<EditCollectionRequest, EditCollectionReaction>(page);
            Reaction.Execute(ctx);

            Assert.IsEmpty(grid.ColumnDefinitions);

            var response = ctx.Get<EditCollectionResponse>();
            Assert.IsTrue(response.Successful);
            Assert.AreEqual(EditCollectionType.Delete, response.Type);
        }
    }
}
