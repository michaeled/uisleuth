using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UISleuth.Diagnostics;
using UISleuth.Messages;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UISleuth.Networking
{
    internal sealed class DefaultInspectorSocket : InspectorSocket
    {
        private WebSocket _client;
        private WebSocketServer _server;
        private short _port;

        private readonly ManualResetEventSlim _cancellation = new ManualResetEventSlim();
        private readonly object _lock = new object();

        public override bool IsListening => _server?.IsListening ?? false;
        public override string ConnectedClient => ConnectionQueue.Values.FirstOrDefault();
        public override bool IsClientConnected => !string.IsNullOrWhiteSpace(ConnectedClient);

        public const string Localhost = "localhost";


        public override void Listen(short port = ServiceEndpoint.MobilePort)
        {
            lock (_lock)
            {
                _port = port;

                _server = new WebSocketServer(_port, false)
                {
                    KeepClean = false,
                    WaitTime = ServiceEndpoint.PingWaitTime
                };

                _server.AddWebSocketService(ServiceEndpoint.MobilePath, () => new InspectorServerBehavior(this));
                _server.AddWebSocketService(ServiceEndpoint.MobileHeartbeatPath, () => new HeartbeatServerBehavior(_server));
                _cancellation.Reset();
            }

            Task.Run(() =>
            {
                if (_server != null)
                {
                    lock (_lock)
                    {
                        _server?.Start();
                    }

                    OnSocketStarted();
                }

                _cancellation.Wait();
                OnTrace(UILogLevel.Info, "Cancellation event received; stopping server.");
            });
        }


        public override void Connect(string uri)
        {
            lock (_lock)
            {
                _client = new WebSocket(uri);
            }

            Task.Run(() =>
            {
                if (_client != null)
                {
                    lock (_lock)
                    {
                        _client.OnOpen += OnClientOpen;
                        _client.OnClose += OnClientClose;
                        _client.OnError += OnClientError;
                        _client.OnMessage += OnClientMessage;
                        _client.Connect();
                    }

                    IsConnectedToRemoteServer = true;
                    OnSocketStarted();
                }

                _cancellation.Wait();
                OnTrace(UILogLevel.Info, "Cancellation event received; disconnected from remote server.");
            });
        }


        public override void Stop()
        {
            try
            {
                lock (_lock)
                {
                    if (!_cancellation.IsSet)
                    {
                        _cancellation.Set();

                        if (IsListening)
                        {
                            var reason = new ServerStoppedOk().ToJson();
                            _server.Stop(CloseStatusCode.Normal, reason);
                            _server = null;
                        }

                        if (IsConnectedToRemoteServer)
                        {
                            _client.Close(CloseStatusCode.Normal);
                            _client.OnClose -= OnClientClose;
                            _client.OnOpen -= OnClientOpen;
                            _client.OnError -= OnClientError;
                            _client.OnMessage -= OnClientMessage;

                            _client = null;
                            IsConnectedToRemoteServer = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnTrace(UILogLevel.Error, $"An error occurred while stopping the socket:\n{e}");
            }
        }


        public override void Send(string message)
        {
            try
            {
                if (IsListening)
                {
                    _server.WebSocketServices.Broadcast(message);
                }

                if (IsConnectedToRemoteServer)
                {
                    _client.Send(message);
                }
            }
            catch (Exception e)
            {
                OnTrace(UILogLevel.Error, $"An error occurred while sending:\n{message}\n{e}");
            }
        }


        #region Events


        internal override void OnClientDisconnected(SocketClosedEventArgs e)
        {
            if (ConnectionQueue.ContainsKey(e.UniqueId))
            {
                ConnectionQueue.Remove(e.UniqueId);
            }

            base.OnClientDisconnected(e);
        }


        internal override void OnClientConnected(SocketConnectedEventArgs e)
        {
            if (!ConnectionQueue.ContainsKey(e.UniqueId))
            {
                ConnectionQueue.Add(e.UniqueId, e.Address);
            }

            base.OnClientConnected(e);
        }


        private void OnClientError(object sender, ErrorEventArgs e)
        {
            var ea = new SocketErrorEventArgs
            {
                Message = e.Message
            };

            OnError(ea);
        }


        private void OnClientClose(object sender, CloseEventArgs e)
        {
            var level = UILogLevel.Info;

            if (!e.WasClean)
            {
                level = UILogLevel.Error;
            }

            var ea = new SocketClosedEventArgs
            {
                Address = Localhost,
                UniqueId = Localhost
            };

            OnClientDisconnected(ea);
            OnTrace(level, $"Disconnected from remote server.{Environment.NewLine}Code: {e.Code}. Reason: {e.Reason}");

        }


        private void OnClientOpen(object sender, EventArgs e)
        {
            var ea = new SocketConnectedEventArgs
            {
                Address = Localhost,
                UniqueId = Localhost
            };

            OnClientConnected(ea);
            OnTrace(UILogLevel.Info, "Connected to remote server.");
        }


        private bool _sent = false;
        private void OnClientMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var msgEvent = new SocketMessageReceivedEventArgs
                {
                    Message = e.Data
                };

                if (!_sent)
                {
                    OnClientConnected(new SocketConnectedEventArgs { Address = Localhost, UniqueId = Localhost });
                    _sent = true;
                }

                OnMessage(msgEvent);
            }
        }

        #endregion
    }
}
