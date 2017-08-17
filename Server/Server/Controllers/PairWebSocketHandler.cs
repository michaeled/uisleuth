using System;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using StackExchange.Redis;
using Microsoft.Web.WebSockets;

namespace Server.Controllers
{
    public class PairWebSocketHandler : WebSocketHandler
    {
        private const string DesktopSuffix = "::desktop";
        private const string MobileSuffix = "::mobile";
        private const string ClientKeyQs = "clientKey";
        private readonly string _clientKey;

        private string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(_clientKey)) return null;

                var buffer = _clientKey.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
                if (buffer.Length == 0) return null;

                return buffer[0];
            }
        }

        private string CurrentChannel => _clientKey;

        private string OtherChannel
        {
            get
            {
                if (string.IsNullOrEmpty(_clientKey)) return null;

                if (_clientKey.Contains(MobileSuffix))
                {
                    return DesktopChannel;
                }

                if (_clientKey.Contains(DesktopSuffix))
                {
                    return MobileChannel;
                }

                return null;
            }
        }

        private string MobileChannel => $"{ClientId}{MobileSuffix}";
        private string DesktopChannel => $"{ClientId}{DesktopSuffix}";

        private static readonly WebSocketCollection Users = new WebSocketCollection();
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["pubsub"]));
        private static ConnectionMultiplexer Connection => LazyConnection.Value;

        public PairWebSocketHandler(string clientKey)
        {
            _clientKey = clientKey;
        }

        public override void OnOpen()
        {
            if (string.IsNullOrEmpty(CurrentChannel))
            {
                WebSocketContext
                    .WebSocket
                    .CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "No clientKey.", CancellationToken.None)
                    .Wait();

                return;
            }

            if (GetUser(_clientKey) != null)
            {
                WebSocketContext
                    .WebSocket
                    .CloseAsync(WebSocketCloseStatus.PolicyViolation, "Session has already started.", CancellationToken.None)
                    .Wait();

                return;
            }

            Users.Add(this);

            var sub = Connection.GetSubscriber();
            sub.Subscribe(CurrentChannel, OnChannelMessage);
        }

        public override void OnMessage(string message)
        {
            var sender = WebSocketContext.QueryString[ClientKeyQs];
            if (string.IsNullOrWhiteSpace(sender))
            {
                return;
            }

            var sub = Connection.GetSubscriber();
            sub.Publish(OtherChannel, message);
        }

        public override void OnClose()
        {
            Users.Remove(this);

            if (!string.IsNullOrWhiteSpace(CurrentChannel))
            {
                var sub = Connection.GetSubscriber();
                sub.Unsubscribe(CurrentChannel, OnChannelMessage);
            }
        }

        private void OnChannelMessage(RedisChannel channel, RedisValue message)
        {
            if (channel.IsNullOrEmpty) return;
            if (message.IsNullOrEmpty) return;

            var user = GetUser(channel);
            user?.Send((string) message);
        }

        private WebSocketHandler GetUser(string clientKey)
        {
            if (Users.Count == 0) return null;

            var user = Users.FirstOrDefault(u =>
            {
                var ctx = u as PairWebSocketHandler;
                if (ctx == null) return false;

                return ctx._clientKey == clientKey;
            });

            return user;
        }
    }
}
