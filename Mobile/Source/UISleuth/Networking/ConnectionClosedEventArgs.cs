using System;

namespace XenForms.Designer.Communication
{
    public class ConnectionClosedEventArgs : EventArgs
    {
        public bool ServerInitiated { get; set; }


        public ConnectionClosedEventArgs(bool serverInitiated = false)
        {
            ServerInitiated = serverInitiated;
        }
    }
}
