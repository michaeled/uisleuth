using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UISleuth.Networking
{
    internal struct OnlineResponse
    {
        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
    }

    internal class HeartbeatServerBehavior : WebSocketBehavior
    {
        private readonly WebSocketServer _server;
        private readonly IInspectorEnvironment _environment;


        public HeartbeatServerBehavior(WebSocketServer server)
        {
            _server = server;
            _environment = InspectorContainer.Current.Resolve<IInspectorEnvironment>();
        }


        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                var response = new OnlineResponse
                {
                    App = _environment.GetAppName(),
                    Port = _server.Port,
                    State = "online"
                };

                var json = JsonConvert.SerializeObject(response);

                Send(json);
            }
        }
    }
}