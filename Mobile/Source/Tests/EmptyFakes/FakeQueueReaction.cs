using UISleuth.Networking;
using UISleuth.Reactions;

namespace UISleuth.Tests.EmptyFakes
{
    internal class FakeQueueReaction : Reaction
    {
        public UIMessageContext Context { get; set; }


        protected override void OnExecute(UIMessageContext ctx)
        {
            ctx.SetResponse<FakeQueueResponse>(r =>
            {
                r.Completed = true;
            });

            Context = ctx;
        }
    }
}
