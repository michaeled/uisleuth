using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Tests.Messages
{
    [TestFixture]
    public class TestUIMessageContext
    {
        [Test]
        public void Should_create_null_objects_when_default_ctor_called()
        {
            // Code relies on this behavior.
            var ctx = new UIMessageContext();
            Assert.IsNull(ctx.Request, "Request expected to be null.");
            Assert.IsNull(ctx.Response, "Response expected to be null.");
        }


        [Test]
        public void Get_methods()
        {
            var ctx = new UIMessageContext
            {
                Request = new ServerStoppedOk(),
                Response = new OkResponse()
            };

            var req = ctx.Get<ServerStoppedOk>();
            Assert.AreEqual(ctx.Request, req, "Request check failed.");

            var res = ctx.Get<OkResponse>();
            Assert.AreEqual(ctx.Response, res, "Response check failed.");
        }


        [Test]
        public void Suggest()
        {
            var ctx = new UIMessageContext();

            var res = ctx.SetResponse<OkResponse>(r =>
            {
                r.Suggest<GetVisualTreeRequest>();
            });

            Assert.AreEqual(res.NextSuggestedMessage, nameof(GetVisualTreeRequest));
        }
    }
}
