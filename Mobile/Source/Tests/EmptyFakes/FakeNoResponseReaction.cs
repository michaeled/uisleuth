using UISleuth.Networking;
using UISleuth.Reactions;

namespace UISleuth.Tests.EmptyFakes
{
    internal class FakeNoResponseReaction : Reaction
    {
        public bool Completed { get; set; }
        public UIMessageContext Context { get; set; }

        protected override void OnExecute(UIMessageContext ctx)
        {
            Completed = true;
            Context = ctx;
        }
    }
}