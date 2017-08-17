using System;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions.SetProperties
{
    [TestFixture]
    public class TestSetPropertyReaction : TestBaseReaction
    {
        [Test]
        public void Should_set_widget_string_property()
        {
            var stack = new StackLayout();
            var label = new Label {Text = "text1"};
            stack.Children.Add(label);

            var page = new ContentPage
            {
                Content = stack
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new [] {"Text"};
                r.WidgetId = label.Id.ToString();
                r.Value = "text1 set";
            });

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual("text1 set", label.Text);
        }


        [Test]
        public void Should_set_widget_double_property()
        {
            var stack = new StackLayout();
            var label = new Label { HeightRequest = 50};
            stack.Children.Add(label);

            var page = new ContentPage
            {
                Content = stack
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new [] { "HeightRequest"};
                r.WidgetId = label.Id.ToString();
                r.Value = 100;
            });

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual(100, label.HeightRequest);
        }


        [Test]
        public void Should_set_single_enum_property()
        {
            var stack = new StackLayout();
            var label = new Label { HorizontalTextAlignment = TextAlignment.Center };
            stack.Children.Add(label);

            var page = new ContentPage
            {
                Content = stack
            };

            // change from Center to End
            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new[] {"HorizontalTextAlignment"};
                r.WidgetId = label.Id.ToString();
                r.Value = "End";
            });

            Assert.AreEqual(TextAlignment.Center, label.HorizontalTextAlignment);

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual(TextAlignment.End, label.HorizontalTextAlignment);
        }


        [Test]
        public void Should_set_flag_enum_property()
        {
            var stack = new StackLayout();
            var label = new Label { FontAttributes = FontAttributes.None };
            stack.Children.Add(label);

            var page = new ContentPage
            {
                Content = stack
            };

            // change from None to Bold | Italic
            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new []{ "FontAttributes"};
                r.WidgetId = label.Id.ToString();
                r.Value = "Bold | Italic";
            });

            Assert.AreEqual(FontAttributes.None, label.FontAttributes);

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual(FontAttributes.Bold | FontAttributes.Italic, label.FontAttributes);
        }


        [Test]
        public void Should_set_color_property()
        {
            var stack = new StackLayout();
            var label = new Label { TextColor = Color.Black };
            stack.Children.Add(label);

            var page = new ContentPage
            {
                Content = stack
            };

            var json = JsonConvert.SerializeObject(Color.Red);

            // change from None to Bold | Italic
            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new [] { "TextColor"};
                r.WidgetId = label.Id.ToString();
                r.Value = json;
            });

            Assert.AreEqual(Color.Black, label.TextColor);

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual(Color.Red, label.TextColor);
        }


        [Test]
        public void Should_set_Image_Source_property()
        {
            var image = new Image {Source = null};
            var page = new ContentPage
            {
                Content = image
            };

            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                ImageSourceTest.applogo.Save(ms, ImageFormat.Png);
                buffer = ms.ToArray();
            }

            var b64 = Convert.ToBase64String(buffer);

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.Path = new [] { "Source"};
                r.WidgetId = image.Id.ToString();
                r.Value = b64;
                r.IsBase64 = true;
            });

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.IsNotNull(image.Source);
        }


        [Test]
        public void Should_set_subobj_property_subobj_is_class_and_readwrite()
        {
            var fake = new FakeClass {Name = "Empty"};
            var view = new ViewHasSubProps
            {
                FakeRef = fake
            };

            var page = new ContentPage
            {
                Content = view
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.WidgetId = view.Id.ToString();
                r.Path = new [] { "FakeRef", "Name" };
                r.Value = "New Value";
            });

            Assert.AreEqual("Empty", fake.Name);
            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual("New Value", fake.Name);
        }


        [Test]
        public void Should_set_subobj_property_subobj_is_struct_and_readwrite()
        {
            var fake = new FakeStruct { Name = "Old Value" };
            var view = new ViewHasSubProps
            {
                FakeVal = fake
            };

            var page = new ContentPage
            {
                Content = view
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.WidgetId = view.Id.ToString();
                r.Path = new[] {"FakeVal", "Name"};
                r.Value = "A New Value";
            });

            Assert.AreEqual("Old Value", fake.Name);
            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual("A New Value", view.FakeVal.Name);
        }


        [Test]
        public void Should_set_nested_struct_values()
        {
            var inner = new InnerStruct {Name = "Inner Struct"};

            var fake = new FakeStruct
            {
                Inner = inner,
                Name = "Old Value"
            };

            var view = new ViewHasSubProps
            {
                FakeVal = fake
            };

            var page = new ContentPage
            {
                Content = view
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.WidgetId = view.Id.ToString();
                r.Path = new[] { "FakeVal", "Inner", "Name" };
                r.Value = "Newly Set";
            });

            Assert.AreEqual("Inner Struct", view.FakeVal.Inner.Name);
            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual("Newly Set", view.FakeVal.Inner.Name);
        }


        [Test]
        public void Should_set_struct_value_by_using_staticfield_name()
        {
            // Ex: LayoutOptions.End to LayoutOptions.StartAndExpand

            var label = new Label
            {
                VerticalOptions = LayoutOptions.End
            };

            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new UIMessageContext();
            ctx.SetRequest<SetPropertyRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
                r.Path = new [] { "VerticalOptions"};
                r.Value = "StartAndExpand";
            });

            Assert.IsTrue(label.VerticalOptions.Expands == false &&
                          label.VerticalOptions.Alignment == LayoutAlignment.End);

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            Reaction.Execute(ctx);

            Assert.AreEqual(label.VerticalOptions, LayoutOptions.StartAndExpand);
        }


        [Test]
        public void Should_set_float_type()
        {
            var page = SetPrimitiveProperty("Float", "11.1");
            Assert.AreEqual(11.1f, page.Float);
        }


        [Test]
        public void Should_set_byte_type()
        {
            var page = SetPrimitiveProperty("Byte", "55");
            Assert.AreEqual(55, page.Byte);
        }


        [Test]
        public void Should_set_short_type()
        {
            var page = SetPrimitiveProperty("Short", "95");
            Assert.AreEqual(95, page.Short);
        }


        [Test]
        public void Should_set_ushort_type()
        {
            var page = SetPrimitiveProperty("UShort", "195");
            Assert.AreEqual(195, page.UShort);
        }


        [Test]
        public void Should_set_int_type()
        {
            var page = SetPrimitiveProperty("Int", "2195");
            Assert.AreEqual(2195, page.Int);
        }


        [Test]
        public void Should_set_uint_type()
        {
            var page = SetPrimitiveProperty("UInt", "21951");
            Assert.AreEqual(21951, page.UInt);
        }


        [Test]
        public void Should_set_long_type()
        {
            var page = SetPrimitiveProperty("Long", "219512");
            Assert.AreEqual(219512, page.Long);
        }


        [Test]
        public void Should_set_ulong_type()
        {
            var page = SetPrimitiveProperty("ULong", "2195125");
            Assert.AreEqual(2195125, page.ULong);
        }


        [Test]
        public void Should_set_char_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.Char);

            var page = SetPrimitiveProperty("Char", "Z");
            Assert.AreEqual('Z', page.Char);
        }


        [Test]
        public void Should_set_nchar_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NChar);

            var page = SetPrimitiveProperty("NChar", "NULL");
            Assert.AreEqual(null, page.NChar);
        }


        [Test]
        public void Should_set_nfloat_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NFloat);

            var page = SetPrimitiveProperty("NFloat", "null");
            Assert.AreEqual(null, page.NFloat);
        }


        [Test]
        public void Should_set_nbyte_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NByte);

            var page = SetPrimitiveProperty("NByte", null);
            Assert.AreEqual(null, page.NByte);
        }


        [Test]
        public void Should_set_nshort_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NShort);

            var page = SetPrimitiveProperty("NShort", "NULL");
            Assert.AreEqual(null, page.NShort);
        }


        [Test]
        public void Should_set_nushort_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NUShort);

            var page = SetPrimitiveProperty("NUShort", "NULL");
            Assert.AreEqual(null, page.NUShort);
        }


        [Test]
        public void Should_set_nint_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NInt);

            var page = SetPrimitiveProperty("NInt", "NULL");
            Assert.AreEqual(null, page.NInt);
        }


        [Test]
        public void Should_set_nuint_type()
        {
            var safety = new PageWithPrimitives();
            Assert.IsNotNull(safety.NUInt);

            var page = SetPrimitiveProperty("NUInt", "NULL");
            Assert.AreEqual(null, page.NUInt);
        }


        [Test]
        public void Should_set_nulong_type()
        {
            var page = SetPrimitiveProperty("NULong", "NULL");
            Assert.AreEqual(null, page.NULong);
        }
    }
}
