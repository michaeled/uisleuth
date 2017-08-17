using System;
using System.Collections.Generic;
using UISleuth.Diagnostics;

namespace UISleuth.Networking
{
    /// <summary>
    /// Network Communication API.
    /// </summary>
    internal abstract class InspectorSocket
    {
        internal readonly Dictionary<string, string> ConnectionQueue = new Dictionary<string, string>();


        /// <summary>
        /// Returns true if the server is accepting connections; otherwise, false.
        /// </summary>
        public virtual bool IsListening { get; protected set; }


        /// <summary>
        /// Return true if connected to a remote server; otherwise, false.
        /// </summary>
        public virtual bool IsConnectedToRemoteServer { get; protected set; }


        /// <summary>
        /// Return address of connected client.
        /// </summary>
        public virtual string ConnectedClient { get; protected set; }


        /// <summary>
        /// Returns true if the server has connected clients; otherwise, false.
        /// </summary>
        public virtual bool IsClientConnected { get; protected set; }


        /// <summary>
        /// Start listening and accepting client connections.
        /// </summary>
        public abstract void Listen(short port = ServiceEndpoint.MobilePort);


        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="uri"></param>
        public abstract void Connect(string uri);


        /// <summary>
        /// Stop listening and accepting client connections.
        /// Any connected client connections will be closed.
        /// </summary>
        public abstract void Stop();


        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Send(string message);

        public event EventHandler<EventArgs> SocketStarted; 
        public event EventHandler<SocketConnectedEventArgs> ClientConnected;
        public event EventHandler<SocketClosedEventArgs> ClientDisconnected;
        public event EventHandler<SocketTraceEventArgs> Trace;
        public event EventHandler<SocketErrorEventArgs> Error;
        public event EventHandler<SocketMessageReceivedEventArgs> Message;  


        internal virtual void OnError(SocketErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }


        internal virtual void OnTrace(UILogLevel level, string description)
        {
            var e = new SocketTraceEventArgs(description, level);
            Trace?.Invoke(this, e);
        }


        internal virtual void OnTrace(SocketTraceEventArgs e)
        {
            Trace?.Invoke(this, e);
        }


        internal virtual void OnClientDisconnected(SocketClosedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }


        internal virtual void OnClientConnected(SocketConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }


        internal virtual void OnMessage(SocketMessageReceivedEventArgs e)
        {
            Message?.Invoke(this, e);
        }


        protected virtual void OnSocketStarted()
        {
            SocketStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}