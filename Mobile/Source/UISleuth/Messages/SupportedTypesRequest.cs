using UISleuth.Widgets;

namespace UISleuth.Messages
{
    /// <summary>
    /// Sent immediately after a client has connected to the design server.
    /// The <see cref="Types"/> collection contains all known types that can be created, updated, read, or deleted from 
    /// the client.
    /// </summary>
    internal class SupportedTypesRequest : Request
    {
        public UIType[] Types { get; set; }
    }
}
