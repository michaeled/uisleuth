using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Reflection;
using UISleuth.Tests.EmptyFakes;
using UISleuth.Workflow;

namespace UISleuth.Tests.Workflow
{
    [TestFixture]
    public class TestDefaultDesignWorkflow
    {
        private UIMessageFinder _finder;
        private Mock<InspectorSocket> _socket;


        [SetUp]
        public void BeforeTest()
        {
            _finder = new UIMessageFinder();
            _socket = new Mock<InspectorSocket>();

            Reaction.Reset();
        }


        [Test]
        public void Starting_server_creates_consumer_thread()
        {
            var workflow = new DefaultInspectorWorkflow();

            Assert.AreEqual(0, workflow.ConsumerThreads, "Consumer threads should be 0.");

            // todo
            workflow.Start(null, null);

            SpinWait.SpinUntil(() => workflow.ConsumerThreads > 1, TimeSpan.FromSeconds(3));

            Assert.AreEqual(1, workflow.ConsumerThreads, "Workflow isn't running.");
        }


        [Test]
        public void Queing_one_message_without_handler_registered()
        {
            var msg = UIMessage.Create<FakeQueueRequest>();

            var json = msg.ToJson();
            var workflow = new DefaultInspectorWorkflow();

            /* todo */
            workflow.Start(null, null);
            workflow.Queue(json);
            SpinWait.SpinUntil(() => workflow.QueuedMessages > 1, TimeSpan.FromSeconds(3));
            Assert.AreEqual(1, workflow.QueuedMessages);
        }


        [Test]
        public void Queing_message_with_handler_registered()
        {
            var msg = UIMessage.Create<FakeQueueRequest>();
            var json = msg.ToJson();
            var workflow = new DefaultInspectorWorkflow();
            
            var reaction = new FakeQueueReaction();

            /* todo */
            Reaction.Register<FakeQueueRequest, FakeQueueReaction>(() => reaction);
            workflow.Start(null, null);
            workflow.Queue(json);

            SpinWait.SpinUntil(() => reaction.Context != null, TimeSpan.FromSeconds(3));
            Assert.IsNotNull(reaction.Context);

            _socket.Verify(r => r.Send(It.IsAny<string>()), Times.AtMostOnce);
        }


        [Test]
        public void OkResponse_set_when_reaction_didnt_set_a_response()
        {
            var msg = UIMessage.Create<FakeQueueRequest>();
            var json = msg.ToJson();
            var workflow = new DefaultInspectorWorkflow();

            var reaction = new FakeNoResponseReaction();
            Reaction.Register<FakeQueueRequest, FakeNoResponseReaction>(() => reaction);

            // todo
            workflow.Start(null, null);
            workflow.Queue(json);

            SpinWait.SpinUntil(() => reaction.Completed, TimeSpan.FromSeconds(3));
            Assert.IsTrue(reaction.Completed);
            Assert.IsInstanceOf(typeof(OkResponse), reaction.Context.Response);
        }
    }
}
