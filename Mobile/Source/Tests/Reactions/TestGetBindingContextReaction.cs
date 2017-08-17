using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions
{
    [TestFixture]
    public class TestGetBindingContextReaction : TestBaseReaction
    {
        public class MyBindingType
        {
            public string Value { get; set; }
            public MyBindingType Child { get; set; }
        }

        public class TypeWithArrayOfObjects
        {
            public MyBindingType[] Value;
        }

        [Test]
        public void Should_serialize_BindingContext()
        {
            var ctx = new UIMessageContext();

            var value = new MyBindingType
            {
                Value = "test123"
            };

            var label = new Label
            {
                BindingContext = value
            };

            var page = new ContentPage
            {
                Content = label
            };

            ctx.SetRequest<GetBindingContextRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetBindingContextRequest, GetBindingContextReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Response as GetBindingContextResponse;

            Assert.IsNotNull(res);
            Assert.AreEqual("{\"Value\":\"test123\",\"Child\":null}", res.Data);
        }

        [Test]
        public void Should_serialize_BindingContext_MaxDepth_2()
        {
            var ctx = new UIMessageContext();

            var value = new MyBindingType
            {
                Value = "Node 1",
                Child = new MyBindingType
                {
                    Value = "Node 2",
                    Child = new MyBindingType
                    {
                        Value = "Node 3",
                        Child = new MyBindingType
                        {
                            Value = "Node 4"
                        }
                    }
                }
            };

            var label = new Label
            {
                BindingContext = value
            };

            var page = new ContentPage
            {
                Content = label
            };

            ctx.SetRequest<GetBindingContextRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetBindingContextRequest, GetBindingContextReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Response as GetBindingContextResponse;
            Assert.IsNotNull(res);
            Assert.AreEqual("{\"Value\":\"Node 1\",\"Child\":{\"Value\":\"Node 2\",\"Child\":{}}}", res.Data);
        }

        [Test]
        public void Should_serialize_array_of_objects_to_MaxDepth_2()
        {
            var ctx = new UIMessageContext();

            var value = new TypeWithArrayOfObjects
            {
                Value = new[]
                {
                    // item 1
                    new MyBindingType
                    {
                        Value = "Node a1",
                        Child = new MyBindingType
                        {
                            Value = "Node a2",
                            Child = new MyBindingType
                            {
                                Value = "Node a3",
                            }
                        }
                    },
                    // item 2
                    new MyBindingType
                    {
                        Value = "Node b1",
                        Child = new MyBindingType
                        {
                            Value = "Node b2",
                            Child = new MyBindingType
                            {
                                Value = "Node b3",
                            }
                        }
                    },
                    // item 3
                    new MyBindingType
                    {
                        Value = "Node c1",
                        Child = new MyBindingType
                        {
                            Value = "Node c2",
                            Child = new MyBindingType
                            {
                                Value = "Node c3",
                            }
                        }
                    }
                }
            };

            var label = new Label
            {
                BindingContext = value
            };

            var page = new ContentPage
            {
                Content = label
            };

            ctx.SetRequest<GetBindingContextRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            InspectorReaction.Register<GetBindingContextRequest, GetBindingContextReaction>(page);
            Reaction.Execute(ctx);

            var res = ctx.Response as GetBindingContextResponse;
            Assert.IsNotNull(res);
        }
    }
}