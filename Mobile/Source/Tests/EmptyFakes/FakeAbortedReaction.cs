using System;
using UISleuth.Networking;
using UISleuth.Reactions;

namespace UISleuth.Tests.EmptyFakes
{
    sealed class FakeAbortedReaction : Reaction
    {
        protected override void OnExecute(UIMessageContext context)
        {
            throw new InvalidOperationException();
        }
    }
}