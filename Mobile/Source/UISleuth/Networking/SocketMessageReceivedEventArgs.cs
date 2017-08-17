using System;

namespace UISleuth.Networking
{
    internal class SocketMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
