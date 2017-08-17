using Microsoft.Web.WebSockets;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Server.Controllers
{
    public class PairController : ApiController
    {
        public HttpResponseMessage Get(string clientKey)
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                HttpContext.Current.AcceptWebSocketRequest(new PairWebSocketHandler(clientKey));
                return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}