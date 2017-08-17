using System;
using UISleuth.Messages;

namespace UISleuth.Networking
{
    /// <summary>
    /// Encapsulates a single client request and designer response.
    /// </summary>
    internal class UIMessageContext
    {
        public string Message { get; set; }
        public UIMessage Request { get; set; }
        public UIMessage Response { get; set; }
        internal bool ShouldQueue { get; set; }


        public UIMessageContext()
        {
            Request = null;
            Response = null;
        }


        public UIMessageContext(UIMessage request, UIMessage response)
        {
            Request = request;
            Response = response;
        }
    }


    internal static class UIMessageContextExtensions
    {
        public static T Get<T>(this UIMessageContext ctx) where T : UIMessage
        {
            var request = ctx.Request as T;

            if (request != null)
            {
                return request;
            }

            var response = ctx.Response as T;
            return response;
        }


        public static T SetResponse<T>(this UIMessageContext ctx, Action<T> action = null) where T : Response, new()
        {
            var response = UIMessage.Create<T>();
            ctx.Response = response;

            action?.Invoke(response);

            return (T) ctx.Response;
        }


        public static T SetRequest<T>(this UIMessageContext ctx, Action<T> action = null) where T : Request, new()
        {
            var request = UIMessage.Create<T>();
            ctx.Request = request;

            action?.Invoke(request);

            return (T) ctx.Request;
        }
    }
}