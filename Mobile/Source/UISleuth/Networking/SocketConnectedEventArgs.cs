using System;

namespace UISleuth.Networking
{
    internal class SocketConnectedEventArgs : EventArgs
    {
        public string UniqueId { get; set; }
        public string Address { get; set; }
    }
}
