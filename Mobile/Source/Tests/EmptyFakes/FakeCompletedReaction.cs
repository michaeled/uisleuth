using UISleuth.Networking;
using UISleuth.Reactions;

namespace UISleuth.Tests.EmptyFakes
{
    sealed class FakeCompletedReaction : Reaction
    {
        protected override void OnExecute(UIMessageContext context)
        {
            context.Response = new FakeCompletedResponse
            {
                Action = nameof(FakeCompletedResponse),
            };
        }
    }
}