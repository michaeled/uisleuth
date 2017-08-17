using System;

namespace UISleuth.Networking
{
    internal class SocketClosedEventArgs : EventArgs
    {
        public string UniqueId { get; set; }
        public string Address { get; set; }
    }
}
