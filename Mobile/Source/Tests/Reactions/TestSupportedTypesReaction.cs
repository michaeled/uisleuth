using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Widgets;

namespace UISleuth.Tests.Reactions
{
    [TestFixture]
    public class TestSupportedTypesReaction
    {
        private TypeRegistrar Tr => TypeRegistrar.Instance;


        [Test]
        public void Register_supported_types_when_message_is_received()
        {
            var ctx = new UIMessageContext();
            ctx.SetRequest<SupportedTypesRequest>(r =>
            {
                r.Types = new[]
                {
                    new UIType
                    {
                        Descriptor = UIPropertyDescriptors.None,
                        FullName = "String"
                    },
                    new UIType
                    {
                        Descriptor = UIPropertyDescriptors.None,
                        FullName = "Integer"
                    }
                };
            });

            Reaction.Register<SupportedTypesRequest, SupportedTypesReaction>();
            Reaction.Execute(ctx);
            Assert.IsTrue(Tr.Types.Count == 2);
        }
    }
}
