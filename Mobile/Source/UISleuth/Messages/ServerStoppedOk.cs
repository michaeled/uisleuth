namespace UISleuth.Messages
{
    /// <summary>
    /// Sent before the design server stops responding to requests.
    /// When this is sent, the client is informed that the design server executed it's shutdown
    /// routine without error.
    /// </summary>
    internal class ServerStoppedOk : UIMessage {}
}
