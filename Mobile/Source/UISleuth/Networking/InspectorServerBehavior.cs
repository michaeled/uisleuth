using System;
using UISleuth.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UISleuth.Networking
{
    internal sealed class InspectorServerBehavior : WebSocketBehavior
    {
        private readonly InspectorSocket _socket;


        public InspectorServerBehavior(InspectorSocket socket)
        {
            _socket = socket;
            EmitOnPing = false;
            IgnoreExtensions = true;
        }


        protected override void OnMessage(MessageEventArgs e)
        {
            #pragma warning disable 618

            if (e.IsBinary)
            {
                _socket.OnTrace(UILogLevel.Warn, @"Binary messages are unsupported. Ignored.");
                return;
            }


            if (e.IsPing)
            {
                _socket.OnTrace(UILogLevel.Trace, @"Ping/Pong received.");
                return;
            }


            if (e.IsText)
            {
                // The data property contains the text message received from the client.
                // It's not known if the data is valid JSON.

                var msgEvent = new SocketMessageReceivedEventArgs
                {
                    Message = e.Data
                };

                _socket.OnMessage(msgEvent);
            }

            #pragma warning restore 618
        }


        protected override void OnOpen()
        {
            var remoteHost = Context.UserEndPoint.ToString();
            var e = new SocketConnectedEventArgs
            {
                Address = remoteHost,
                UniqueId = ID
            };

            _socket.OnClientConnected(e);
            _socket.OnTrace(UILogLevel.Info, $"Client {remoteHost} connected.");

            if (_socket.ConnectionQueue.Count > 1)
            {
                _socket.OnTrace(UILogLevel.Warn, "Disconnecting client. Multiple clients cannot be connected.");
                Context.WebSocket.Close();
            }
        }


        protected override void OnClose(CloseEventArgs wssArgs)
        {
            var level = UILogLevel.Info;

            if (!wssArgs.WasClean)
            {
                level = UILogLevel.Error;
            }

            var e = new SocketClosedEventArgs
            {
                Address = null,
                UniqueId = ID
            };

            _socket.OnClientDisconnected(e);
            _socket.OnTrace(level, $"Client disconnected.{Environment.NewLine}Code: {wssArgs.Code}. Reason: {wssArgs.Reason}");
        }


        protected override void OnError(ErrorEventArgs wssArgs)
        {
            var e = new SocketErrorEventArgs
            {
                Message = wssArgs.Message
            };

            _socket.OnError(e);
        }
    }
}