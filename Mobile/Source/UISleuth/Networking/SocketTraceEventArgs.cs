using System;
using UISleuth.Diagnostics;

namespace UISleuth.Networking
{
    internal class SocketTraceEventArgs : EventArgs
    {
        public string Description { get; set; }
        public UILogLevel Level { get; set; }


        public SocketTraceEventArgs(string description, UILogLevel level = UILogLevel.Info)
        {
            Description = description;
            Level = level;
        }
    }
}
