using UISleuth.Reactions;

namespace UISleuth.Messages
{
    /// <summary>
    /// Client requests require a response. If a <see cref="Reaction"/> did not create a specific
    /// response for the request, an object of this request is sent.
    /// </summary>
    internal class OkResponse : Response
    {
        /// <summary>
        /// The <see cref="UIMessage.MessageId"/> of the parent request.
        /// </summary>
        public string ReplyingTo { get; set; }
    }
}
