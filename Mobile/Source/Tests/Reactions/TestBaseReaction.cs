using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Reflection;
using UISleuth.Tests.Reactions.EmptyFakes;
using UISleuth.Tests.Reactions.SetProperties;
using Xamarin.Forms;

namespace UISleuth.Tests.Reactions
{
    public class TestBaseReaction
    {
        internal static TypeRegistrar Tr => TypeRegistrar.Instance;
        internal DescriptorRegistrar Dr { get; set; }

        [SetUp]
        public void BeforeTest()
        {
            var tf = new TypeFinder();

            Dr = DescriptorRegistrar.Create(tf);
            Dr.Reset();

            InspectorContainer.Current.Register<UISleuth.Reactions.SurfaceManager, DefaultSurfaceManager>();
            InspectorContainer.Current.Register<ITypeFinder, TypeFinder>();
            InspectorContainer.Current.Register<IInspectorThread, FakeInspectorThread>();
            InspectorContainer.Current.Register<ICodeLoader, FakeCodeLoader>();
            InspectorContainer.Current.Register<IScreenShot, FakeScreenShot>();
        }

        [TearDown]
        public void AfterTest()
        {
            InspectorContainer.Reset();
            Reaction.Reset();
        }

        public PageWithPrimitives SetPrimitiveProperty(string path, string value)
        {
            var page = new PageWithPrimitives
            {
                Content = new Label()
            };

            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            var ctx = new UIMessageContext();

            ctx.SetRequest<SetPropertyRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
                req.Path = new[] { path };
                req.Value = value;
            });

            Reaction.Execute(ctx);

            return page;
        }
    }
}
