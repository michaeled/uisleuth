using UISleuth.Networking;
using UISleuth.Reactions;

namespace UISleuth.Tests.EmptyFakes
{
    /// <summary>
    /// Used for tracking the raw data sent through the designer action.
    /// </summary>
    sealed class FakeTrackRequestReaction : Reaction
    {
        private string RawRequest { get; }


        public FakeTrackRequestReaction(string rawRequest)
        {
            RawRequest = rawRequest;
        }


        protected override void OnExecute(UIMessageContext context)
        {
            context.Message = RawRequest;
        }
    }
}